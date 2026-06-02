
using Business.BusinessAspects;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
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
    /// Oturumdaki kullanıcıyı ve ona bağlı verileri kalıcı olarak siler
    /// (arkadaşlık, istekler, deneme, konu takibi, günlük soru, sınav seçimi, hedefler, davet kodu).
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
                    // Restrict FK: önce arkadaşlık verileri
                    await _db.Set<Arkadaslik>()
                        .Where(x => x.UserIdKucuk == userId || x.UserIdBuyuk == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<ArkadaslikIstegi>()
                        .Where(x => x.GonderenUserId == userId || x.HedefUserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    // DenemeSinavSonucu, DenemeSinavi FK ile birlikte silinir
                    await _db.Set<DenemeSinavi>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciKonuIlerleme>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciGunlukSoruCozumu>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciSinav>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciDersNetHedefi>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciDavetKodu>()
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync(cancellationToken);

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
