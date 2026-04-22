using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;

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


            public RegisterUserCommandHandler(
                IUserRepository userRepository,
                IGroupRepository groupRepository,
                IUserGroupRepository userGroupRepository)
            {
                _userRepository = userRepository;
                _groupRepository = groupRepository;
                _userGroupRepository = userGroupRepository;
            }


            /** Kayıt herkese açık; SecuredOperation anonim istekleri reddeder. */
            [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
            [CacheRemoveAspect()]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                var isThereAnyUser = await _userRepository.GetAsync(u => u.Email == request.Email);

                if (isThereAnyUser != null)
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }

                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);
                var user = new User
                {
                    Email = request.Email,

                    FullName = request.FullName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
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

                return new SuccessResult(Messages.Added);
            }
        }
    }
}