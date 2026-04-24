using System;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Authorizations.Commands
{
    public class ConfirmPasswordResetCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }

        public class ConfirmPasswordResetCommandHandler : IRequestHandler<ConfirmPasswordResetCommand, IResult>
        {
            private readonly IUserRepository _userRepository;

            public ConfirmPasswordResetCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            [ValidationAspect(typeof(ConfirmPasswordResetValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
            {
                var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var user = await _userRepository.GetAsync(u =>
                    u.Email != null && u.Email.Trim().ToLower() == normalizedEmail);

                if (user == null
                    || string.IsNullOrEmpty(user.PasswordResetToken)
                    || user.PasswordResetTokenExpiry == null
                    || user.PasswordResetTokenExpiry < DateTimeExtensions.UtcNowForNpgsqlTimestampCompare())
                {
                    return new ErrorResult(Messages.PasswordResetInvalidOrExpired);
                }

                if (!string.Equals(user.PasswordResetToken, (request.Code ?? string.Empty).Trim(), StringComparison.Ordinal))
                {
                    return new ErrorResult(Messages.PasswordResetInvalidOrExpired);
                }

                HashingHelper.CreatePasswordHash(request.NewPassword, out var passwordSalt, out var passwordHash);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new SuccessResult(Messages.PasswordResetCompleted);
            }
        }
    }
}
