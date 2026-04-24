using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Users.Commands
{
    public class UserChangePasswordCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }

        public class UserChangePasswordCommandHandler : IRequestHandler<UserChangePasswordCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMediator _mediator;

            public UserChangePasswordCommandHandler(IUserRepository userRepository, IMediator mediator)
            {
                _userRepository = userRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [ValidationAspect(typeof(UserChangePasswordValidator), Priority = 2)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var isThereAnyUser = await _userRepository.GetAsync(u => u.UserId == request.UserId);
                if (isThereAnyUser == null)
                {
                    return new ErrorResult(Messages.UserNotFound);
                }

                if (!HashingHelper.VerifyPasswordHash(
                        request.OldPassword ?? string.Empty,
                        isThereAnyUser.PasswordSalt,
                        isThereAnyUser.PasswordHash))
                {
                    return new ErrorResult(Messages.CurrentPasswordIncorrect);
                }

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                isThereAnyUser.PasswordHash = passwordHash;
                isThereAnyUser.PasswordSalt = passwordSalt;

                _userRepository.Update(isThereAnyUser);
                await _userRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}