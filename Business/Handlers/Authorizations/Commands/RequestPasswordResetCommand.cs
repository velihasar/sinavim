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
using Core.Extensions;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Handlers.Authorizations.Commands
{
    public class RequestPasswordResetCommand : IRequest<IResult>
    {
        public string Email { get; set; }

        public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMailService _mailService;
            private readonly IConfiguration _configuration;
            private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

            public RequestPasswordResetCommandHandler(
                IUserRepository userRepository,
                IMailService mailService,
                IConfiguration configuration,
                ILogger<RequestPasswordResetCommandHandler> logger)
            {
                _userRepository = userRepository;
                _mailService = mailService;
                _configuration = configuration;
                _logger = logger;
            }

            [ValidationAspect(typeof(RequestPasswordResetValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
            {
                var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var user = await _userRepository.GetAsync(u =>
                    u.Email != null && u.Email.Trim().ToLower() == normalizedEmail);

                if (user == null)
                {
                    return new SuccessResult(Messages.PasswordResetRequestSent);
                }

                var code = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString(CultureInfo.InvariantCulture);
                var validityMinutes = Math.Clamp(
                    _configuration.GetValue("PasswordReset:CodeValidityMinutes", 15), 5, 120);
                user.PasswordResetToken = code;
                user.PasswordResetTokenExpiry = DateTimeExtensions.UtcNowPlusMinutesForNpgsqlTimestamp(validityMinutes);

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                var mailOk = await VerificationCodeMailHelper.TrySendCodeEmailAsync(
                    _mailService,
                    _configuration,
                    _logger,
                    user,
                    code,
                    validityMinutes,
                    VerificationCodeMailHelper.MailPurpose.PasswordReset,
                    cancellationToken).ConfigureAwait(false);
                if (!mailOk)
                    return new ErrorResult(Messages.PasswordResetMailFailed);

                return new SuccessResult(Messages.PasswordResetRequestSent);
            }
        }
    }
}
