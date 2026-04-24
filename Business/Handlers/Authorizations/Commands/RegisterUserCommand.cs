using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.Helpers;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterUserCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }


        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IGroupRepository _groupRepository;
            private readonly IUserGroupRepository _userGroupRepository;
            private readonly IMailService _mailService;
            private readonly IConfiguration _configuration;
            private readonly ILogger<RegisterUserCommandHandler> _logger;

            public RegisterUserCommandHandler(
                IUserRepository userRepository,
                IGroupRepository groupRepository,
                IUserGroupRepository userGroupRepository,
                IMailService mailService,
                IConfiguration configuration,
                ILogger<RegisterUserCommandHandler> logger)
            {
                _userRepository = userRepository;
                _groupRepository = groupRepository;
                _userGroupRepository = userGroupRepository;
                _mailService = mailService;
                _configuration = configuration;
                _logger = logger;
            }


            /** Kayıt herkese açık; SecuredOperation anonim istekleri reddeder. */
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var existing = await _userRepository.GetAsync(u =>
                    u.Email != null &&
                    u.Email.Trim().ToLower() == normalizedEmail);

                if (existing != null && existing.Status)
                {
                    return new ErrorResult(Messages.EmailAlreadyRegistered);
                }

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);
                var code = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString(CultureInfo.InvariantCulture);
                var validityMinutes = Math.Clamp(
                    _configuration.GetValue("PasswordReset:CodeValidityMinutes", 15), 5, 120);
                var tokenExpiry = DateTimeExtensions.UtcNowPlusMinutesForNpgsqlTimestamp(validityMinutes);

                User user;
                if (existing != null && !existing.Status)
                {
                    existing.FullName = (request.FullName ?? string.Empty).Trim();
                    existing.PasswordHash = passwordHash;
                    existing.PasswordSalt = passwordSalt;
                    existing.EmailVerificationToken = code;
                    existing.EmailVerificationTokenExpiry = tokenExpiry;
                    existing.PasswordResetToken = null;
                    existing.PasswordResetTokenExpiry = null;
                    existing.PendingEmail = null;
                    _userRepository.Update(existing);
                    await _userRepository.SaveChangesAsync();
                    user = existing;
                }
                else
                {
                    user = new User
                    {
                        Email = request.Email.Trim(),
                        FullName = (request.FullName ?? string.Empty).Trim(),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Status = false,
                        EmailVerificationToken = code,
                        EmailVerificationTokenExpiry = tokenExpiry,
                        PendingEmail = null,
                    };

                    _userRepository.Add(user);
                    await _userRepository.SaveChangesAsync();

                    var ogrenci = _groupRepository.Get(g => g.GroupName == StudentGroupConstants.GroupName);
                    if (ogrenci != null && user.UserId > 0)
                    {
                        var already = await _userGroupRepository.GetAsync(
                            ug => ug.UserId == user.UserId && ug.GroupId == ogrenci.Id);
                        if (already == null)
                        {
                            _userGroupRepository.Add(new UserGroup
                            {
                                UserId = user.UserId,
                                GroupId = ogrenci.Id,
                            });
                            await _userGroupRepository.SaveChangesAsync();
                        }
                    }
                }

                var mailOk = await VerificationCodeMailHelper.TrySendCodeEmailAsync(
                    _mailService,
                    _configuration,
                    _logger,
                    user,
                    code,
                    validityMinutes,
                    VerificationCodeMailHelper.MailPurpose.EmailAddressVerification,
                    cancellationToken).ConfigureAwait(false);
                if (!mailOk)
                    return new ErrorResult(Messages.EmailVerificationMailFailed);

                return new SuccessResult(Messages.RegistrationPendingVerification);
            }
        }
    }
}
