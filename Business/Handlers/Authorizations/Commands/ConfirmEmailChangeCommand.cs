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
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Authorizations.Commands
{
    /// <summary>Yeni e-posta adresine gelen kod ile değişikliği tamamlar.</summary>
    public class ConfirmEmailChangeCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public string Code { get; set; }

        public class ConfirmEmailChangeCommandHandler : IRequestHandler<ConfirmEmailChangeCommand, IResult>
        {
            private readonly IUserRepository _userRepository;

            public ConfirmEmailChangeCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            [ValidationAspect(typeof(ConfirmEmailChangeValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(ConfirmEmailChangeCommand request, CancellationToken cancellationToken)
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

                if (string.IsNullOrWhiteSpace(user.PendingEmail))
                {
                    return new ErrorResult(Messages.EmailChangeInvalidOrExpired);
                }

                if (string.IsNullOrEmpty(user.EmailVerificationToken)
                    || user.EmailVerificationTokenExpiry == null
                    || user.EmailVerificationTokenExpiry < DateTimeExtensions.UtcNowForNpgsqlTimestampCompare())
                {
                    return new ErrorResult(Messages.EmailChangeInvalidOrExpired);
                }

                var entered = (request.Code ?? string.Empty).Trim();
                if (!string.Equals(user.EmailVerificationToken, entered, StringComparison.Ordinal))
                {
                    return new ErrorResult(Messages.EmailChangeInvalidOrExpired);
                }

                var pendingNorm = user.PendingEmail.Trim().ToLowerInvariant();
                var conflict = await _userRepository.GetAsync(u =>
                    u.UserId != user.UserId &&
                    u.Status &&
                    u.Email != null &&
                    u.Email.Trim().ToLowerInvariant() == pendingNorm);
                if (conflict != null)
                {
                    return new ErrorResult(Messages.EmailAlreadyRegistered);
                }

                user.Email = user.PendingEmail.Trim();
                user.PendingEmail = null;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpiry = null;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new SuccessResult(Messages.EmailChangeSuccess);
            }
        }
    }
}
