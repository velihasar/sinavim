
using Business.BusinessAspects;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Concrete.EntityFramework.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Users.Commands
{
    /// <summary>
    /// Oturumdaki kullanıcıyı ve ona bağlı tüm verileri kalıcı olarak siler.
    /// </summary>
    public class DeleteMyAccountCommand : IRequest<IResult>
    {
        public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand, IResult>
        {
            private readonly ProjectDbContext _db;

            public DeleteMyAccountCommandHandler(ProjectDbContext db)
            {
                _db = db;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var exists = await _db.Set<User>().AnyAsync(u => u.UserId == userId, cancellationToken);
                if (!exists)
                {
                    return new ErrorResult(Messages.UserNotFound);
                }

                await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await _db.Set<UserClaim>().Where(x => x.UserId == userId).ExecuteDeleteAsync(cancellationToken);
                    await _db.Set<UserGroup>().Where(x => x.UserId == userId).ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<User>().Where(u => u.UserId == userId).ExecuteDeleteAsync(cancellationToken);

                    await tx.CommitAsync(cancellationToken);
                }
                catch
                {
                    await tx.RollbackAsync(cancellationToken);
                    throw;
                }

                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
