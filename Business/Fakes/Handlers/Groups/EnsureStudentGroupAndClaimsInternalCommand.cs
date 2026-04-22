using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Fakes.Handlers.Groups
{
    /// <summary>
    /// Startup: "Öğrenci" grubunu yoksa oluşturur; veritabanındaki tüm operasyon taleplerini bu gruba senkronlar.
    /// </summary>
    public class EnsureStudentGroupAndClaimsInternalCommand : IRequest<IResult>
    {
        public class EnsureStudentGroupAndClaimsInternalCommandHandler
            : IRequestHandler<EnsureStudentGroupAndClaimsInternalCommand, IResult>
        {
            private readonly IGroupRepository _groupRepository;
            private readonly IGroupClaimRepository _groupClaimRepository;
            private readonly IOperationClaimRepository _operationClaimRepository;
            private readonly IUserRepository _userRepository;
            private readonly IUserGroupRepository _userGroupRepository;

            public EnsureStudentGroupAndClaimsInternalCommandHandler(
                IGroupRepository groupRepository,
                IGroupClaimRepository groupClaimRepository,
                IOperationClaimRepository operationClaimRepository,
                IUserRepository userRepository,
                IUserGroupRepository userGroupRepository)
            {
                _groupRepository = groupRepository;
                _groupClaimRepository = groupClaimRepository;
                _operationClaimRepository = operationClaimRepository;
                _userRepository = userRepository;
                _userGroupRepository = userGroupRepository;
            }

            public async Task<IResult> Handle(
                EnsureStudentGroupAndClaimsInternalCommand request,
                CancellationToken cancellationToken)
            {
                var name = StudentGroupConstants.GroupName;
                var group = _groupRepository.Get(g => g.GroupName == name);
                if (group == null)
                {
                    group = new Group { GroupName = name };
                    _groupRepository.Add(group);
                    await _groupRepository.SaveChangesAsync();
                    group = _groupRepository.Get(g => g.GroupName == name);
                }

                if (group == null)
                {
                    return new ErrorResult("Öğrenci grubu oluşturulamadı.");
                }

                var claims = (await _operationClaimRepository.GetListAsync()).ToList();
                if (claims.Count == 0)
                {
                    return new SuccessResult(Messages.Added);
                }

                var rows = claims.Select(c => new GroupClaim { GroupId = group.Id, ClaimId = c.Id });
                await _groupClaimRepository.BulkInsert(group.Id, rows);
                await _groupClaimRepository.SaveChangesAsync();

                var gid = group.Id;
                foreach (var u in await _userRepository.GetListAsync())
                {
                    var existing = await _userGroupRepository.GetAsync(
                        x => x.UserId == u.UserId && x.GroupId == gid);
                    if (existing == null)
                    {
                        _userGroupRepository.Add(new UserGroup { UserId = u.UserId, GroupId = gid });
                    }
                }

                await _userGroupRepository.SaveChangesAsync();

                return new SuccessResult(Messages.Added);
            }
        }
    }
}
