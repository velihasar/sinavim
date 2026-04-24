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
using DataAccess.Abstract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Handlers.Authorizations.Commands
{
    /// <summary>Giriş yapmış kullanıcı: yeni e-postaya doğrulama kodu gönderir (UserId token ile eşleşmeli).</summary>
    public class RequestEmailChangeCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; }

        public class RequestEmailChangeCommandHandler : IRequestHandler<RequestEmailChangeCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMailService _mailService;
            private readonly IConfiguration _configuration;
            private readonly ILogger<RequestEmailChangeCommandHandler> _logger;

            public RequestEmailChangeCommandHandler(
                IUserRepository userRepository,
                IMailService mailService,
                IConfiguration configuration,
                ILogger<RequestEmailChangeCommandHandler> logger)
            {
                _userRepository = userRepository;
                _mailService = mailService;
                _configuration = configuration;
                _logger = logger;
            }

            [ValidationAspect(typeof(RequestEmailChangeValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(RequestEmailChangeCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetAsync(u => u.UserId == request.UserId);
                if (user == null)
                {
                    return new ErrorResult(Messages.UserNotFound);
                }

                if (!user.Status)
                {
                    return new ErrorResult(Messages.EmailNotVerified);
                }

                var normalizedNew = (request.NewEmail ?? string.Empty).Trim().ToLowerInvariant();
                var currentNorm = (user.Email ?? string.Empty).Trim().ToLowerInvariant();
                if (string.Equals(normalizedNew, currentNorm, StringComparison.Ordinal))
                {
                    return new ErrorResult(Messages.EmailChangeSameAsCurrent);
                }

                var takenAsEmail = await _userRepository.GetAsync(u =>
                    u.UserId != user.UserId &&
                    u.Email != null &&
                    u.Email.Trim().ToLowerInvariant() == normalizedNew);
                if (takenAsEmail != null)
                {
                    return new ErrorResult(Messages.EmailAlreadyRegistered);
                }

                var takenAsPending = await _userRepository.GetAsync(u =>
                    u.UserId != user.UserId &&
                    u.PendingEmail != null &&
                    u.PendingEmail.Trim().ToLowerInvariant() == normalizedNew);
                if (takenAsPending != null)
                {
                    return new ErrorResult(Messages.EmailAlreadyRegistered);
                }

                var code = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString(CultureInfo.InvariantCulture);
                var validityMinutes = Math.Clamp(
                    _configuration.GetValue("PasswordReset:CodeValidityMinutes", 15), 5, 120);
                var tokenExpiry = DateTimeExtensions.UtcNowPlusMinutesForNpgsqlTimestamp(validityMinutes);

                user.PendingEmail = request.NewEmail.Trim();
                user.EmailVerificationToken = code;
                user.EmailVerificationTokenExpiry = tokenExpiry;
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                var mailUser = new User
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.PendingEmail,
                };

                var mailOk = await VerificationCodeMailHelper.TrySendCodeEmailAsync(
                    _mailService,
                    _configuration,
                    _logger,
                    mailUser,
                    code,
                    validityMinutes,
                    VerificationCodeMailHelper.MailPurpose.EmailAddressChange,
                    cancellationToken,
                    sendToEmailOverride: user.PendingEmail).ConfigureAwait(false);
                if (!mailOk)
                {
                    return new ErrorResult(Messages.EmailVerificationMailFailed);
                }

                return new SuccessResult(Messages.EmailChangeCodeSent);
            }
        }
    }
}
