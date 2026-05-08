
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciSinavs.ValidationRules;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciSinavs.Commands
{
    /// <summary>
    /// Oturumdaki kullanıcının hedef sınavını değiştirir; önceki sınavdaki deneme ve konu takip verilerini siler.
    /// DenemeSinavSonucu satırları DenemeSinavi FK (ON DELETE CASCADE) ile birlikte silinir.
    /// </summary>
    public class ChangeMyKullaniciSinavCommand : IRequest<IDataResult<CreateKullaniciSinavDto>>
    {
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }

        public class ChangeMyKullaniciSinavCommandHandler
            : IRequestHandler<ChangeMyKullaniciSinavCommand, IDataResult<CreateKullaniciSinavDto>>
        {
            private readonly ProjectDbContext _db;
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly ISinavRepository _sinavRepository;

            public ChangeMyKullaniciSinavCommandHandler(
                ProjectDbContext db,
                IKullaniciSinavRepository kullaniciSinavRepository,
                ISinavRepository sinavRepository)
            {
                _db = db;
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _sinavRepository = sinavRepository;
            }

            [ValidationAspect(typeof(ChangeMyKullaniciSinavValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateKullaniciSinavDto>> Handle(
                ChangeMyKullaniciSinavCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Oturum bulunamadı.");
                }

                var ks = await _kullaniciSinavRepository.Query()
                    .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

                if (ks == null)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Sınav seçiminiz bulunmuyor.");
                }

                if (ks.SinavId == request.SinavId)
                {
                    var unchanged = new CreateKullaniciSinavDto
                    {
                        Id = ks.Id,
                        UserId = ks.UserId,
                        SinavId = ks.SinavId,
                        HedefPuan = ks.HedefPuan,
                    };
                    return new SuccessDataResult<CreateKullaniciSinavDto>(unchanged, Messages.Updated);
                }

                var yeniSinav = await _sinavRepository.GetAsync(s => s.Id == request.SinavId);
                if (yeniSinav == null)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Sınav bulunamadı.");
                }

                var eskiSinavId = ks.SinavId;

                await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await _db.Set<DenemeSinavi>()
                        .Where(d => d.UserId == userId && d.SinavId == eskiSinavId)
                        .ExecuteDeleteAsync(cancellationToken);

                    await _db.Set<KullaniciKonuIlerleme>()
                        .Where(p => p.UserId == userId &&
                            _db.Set<Konu>().Any(k =>
                                k.Id == p.KonuId &&
                                _db.Set<Ders>().Any(d => d.SinavId == eskiSinavId && d.Id == k.DersId)))
                        .ExecuteDeleteAsync(cancellationToken);

                    ks.SinavId = request.SinavId;
                    ks.HedefPuan = request.HedefPuan;
                    ks.UpdatedBy = userId;
                    ks.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();

                    _kullaniciSinavRepository.Update(ks);
                    await _kullaniciSinavRepository.SaveChangesAsync();

                    await tx.CommitAsync(cancellationToken);
                }
                catch
                {
                    await tx.RollbackAsync(cancellationToken);
                    throw;
                }

                var dto = new CreateKullaniciSinavDto
                {
                    Id = ks.Id,
                    UserId = ks.UserId,
                    SinavId = ks.SinavId,
                    HedefPuan = ks.HedefPuan,
                };

                return new SuccessDataResult<CreateKullaniciSinavDto>(dto, Messages.Updated);
            }
        }
    }
}
