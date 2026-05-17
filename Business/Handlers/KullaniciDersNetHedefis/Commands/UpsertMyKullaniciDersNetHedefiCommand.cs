using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciDersNetHedefis.ValidationRules;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Commands
{
    /// <summary>
    /// Oturumdaki kullanıcı için tek bir ders net hedefi ekler/günceller/siler (HedefNet ≤ 0 ise kayıt silinir).
    /// Sinav/UserId istemciden alınmaz; <see cref="Ders"/> kaydı doğrulanır.
    /// </summary>
    public class UpsertMyKullaniciDersNetHedefiCommand : IRequest<IDataResult<KullaniciDersNetHedefiDto>>
    {
        public int DersId { get; set; }
        public decimal HedefNet { get; set; }

        public class UpsertMyKullaniciDersNetHedefiCommandHandler
            : IRequestHandler<UpsertMyKullaniciDersNetHedefiCommand,
                IDataResult<KullaniciDersNetHedefiDto>>
        {
            private readonly IKullaniciDersNetHedefiRepository _repo;
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IDersRepository _dersRepository;

            public UpsertMyKullaniciDersNetHedefiCommandHandler(
                IKullaniciDersNetHedefiRepository repo,
                IKullaniciSinavRepository kullaniciSinavRepository,
                IDersRepository dersRepository)
            {
                _repo = repo;
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _dersRepository = dersRepository;
            }

            [ValidationAspect(typeof(UpsertMyKullaniciDersNetHedefiValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciDersNetHedefiDto>> Handle(
                UpsertMyKullaniciDersNetHedefiCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>("Oturum bulunamadı.");
                }

                var ks = await _kullaniciSinavRepository.Query()
                    .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

                if (ks == null)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(
                        "Önce bir sınav seçmelisiniz.");
                }

                var ders = await _dersRepository.GetAsync(d => d.Id == request.DersId);
                if (ders == null)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>("Ders bulunamadı.");
                }

                if (ders.SinavId != ks.SinavId)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(
                        "Bu ders seçili sınavınıza ait değil.");
                }

                var existing = await _repo.Query().FirstOrDefaultAsync(
                    x => x.UserId == userId && x.DersId == request.DersId,
                    cancellationToken);

                if (request.HedefNet <= 0)
                {
                    if (existing != null)
                    {
                        _repo.Delete(existing);
                        await _repo.SaveChangesAsync();
                    }

                    var cleared = new KullaniciDersNetHedefiDto
                    {
                        Id = 0,
                        UserId = userId,
                        DersId = request.DersId,
                        SinavBolumId = ders.SinavBolumId,
                        HedefNet = 0,
                    };
                    return new SuccessDataResult<KullaniciDersNetHedefiDto>(cleared, Messages.Updated);
                }

                if (existing != null)
                {
                    existing.HedefNet = request.HedefNet;
                    existing.SinavBolumId = ders.SinavBolumId;
                    existing.UpdatedBy = userId;
                    existing.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();
                    _repo.Update(existing);
                    await _repo.SaveChangesAsync();
                    return new SuccessDataResult<KullaniciDersNetHedefiDto>(
                        KullaniciDersNetHedefiDto.FromEntity(existing),
                        Messages.Updated);
                }

                var entity = new KullaniciDersNetHedefi
                {
                    UserId = userId,
                    DersId = request.DersId,
                    SinavBolumId = ders.SinavBolumId,
                    HedefNet = request.HedefNet,
                    CreatedBy = userId,
                    CreatedDate = DateTimeExtensions.NowForNpgsqlTimestamp(),
                    IsActive = true,
                };

                _repo.Add(entity);
                await _repo.SaveChangesAsync();

                return new SuccessDataResult<KullaniciDersNetHedefiDto>(
                    KullaniciDersNetHedefiDto.FromEntity(entity),
                    Messages.Added);
            }
        }
    }
}
