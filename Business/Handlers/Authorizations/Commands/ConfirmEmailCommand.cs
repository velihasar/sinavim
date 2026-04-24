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
    public class ConfirmEmailCommand : IRequest<IResult>
    {
        public string Email { get; set; }
        public string Code { get; set; }

        public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, IResult>
        {
            private readonly IUserRepository _userRepository;

            public ConfirmEmailCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            [ValidationAspect(typeof(ConfirmEmailValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
            {
                var normalizedEmail = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
                var user = await _userRepository.GetAsync(u =>
                    u.Email != null && u.Email.Trim().ToLower() == normalizedEmail);

                if (user == null)
                {
                    return new ErrorResult(Messages.EmailVerificationInvalidOrExpired);
                }

                if (user.Status)
                {
                    return new ErrorResult(Messages.EmailAlreadyConfirmed);
                }

                if (string.IsNullOrEmpty(user.EmailVerificationToken)
                    || user.EmailVerificationTokenExpiry == null
                    || user.EmailVerificationTokenExpiry < DateTimeExtensions.UtcNowForNpgsqlTimestampCompare())
                {
                    return new ErrorResult(Messages.EmailVerificationInvalidOrExpired);
                }

                var entered = (request.Code ?? string.Empty).Trim();
                if (!string.Equals(user.EmailVerificationToken, entered, StringComparison.Ordinal))
                {
                    return new ErrorResult(Messages.EmailVerificationInvalidOrExpired);
                }

                user.Status = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpiry = null;

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new SuccessResult(Messages.EmailVerifiedSuccess);
            }
        }
    }
}
