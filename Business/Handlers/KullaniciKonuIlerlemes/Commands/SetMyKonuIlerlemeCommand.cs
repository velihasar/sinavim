using Business.BusinessAspects;
using Business.Constants;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{
    /// <summary>
    /// Oturumdaki kullanıcı için konu ilerlemesi oluşturur veya günceller.
    /// </summary>
    public class SetMyKonuIlerlemeCommand : IRequest<IDataResult<SetMyKonuIlerlemeDto>>
    {
        public int KonuId { get; set; }
        public IlerlemeDurumu Durum { get; set; }

        public class SetMyKonuIlerlemeCommandHandler : IRequestHandler<SetMyKonuIlerlemeCommand, IDataResult<SetMyKonuIlerlemeDto>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _ilerlemeRepository;
            private readonly IKonuRepository _konuRepository;

            public SetMyKonuIlerlemeCommandHandler(
                IKullaniciKonuIlerlemeRepository ilerlemeRepository,
                IKonuRepository konuRepository)
            {
                _ilerlemeRepository = ilerlemeRepository;
                _konuRepository = konuRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<SetMyKonuIlerlemeDto>> Handle(
                SetMyKonuIlerlemeCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<SetMyKonuIlerlemeDto>("Oturum bulunamadı.");
                }

                if (request.KonuId <= 0)
                {
                    return new ErrorDataResult<SetMyKonuIlerlemeDto>("Geçersiz konu.");
                }

                var konu = await _konuRepository.GetAsync(k => k.Id == request.KonuId);
                if (konu == null)
                {
                    return new ErrorDataResult<SetMyKonuIlerlemeDto>("Konu bulunamadı.");
                }

                var existing = await _ilerlemeRepository.Query()
                    .FirstOrDefaultAsync(
                        x => x.UserId == userId && x.KonuId == request.KonuId,
                        cancellationToken);

                if (existing != null)
                {
                    existing.Durum = request.Durum;
                    existing.UpdatedBy = userId;
                    existing.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();
                    _ilerlemeRepository.Update(existing);
                    await _ilerlemeRepository.SaveChangesAsync();

                    return new SuccessDataResult<SetMyKonuIlerlemeDto>(
                        new SetMyKonuIlerlemeDto
                        {
                            Id = existing.Id,
                            KonuId = existing.KonuId,
                            Durum = (short)existing.Durum,
                        },
                        Messages.Updated);
                }

                var entity = new KullaniciKonuIlerleme
                {
                    UserId = userId,
                    KonuId = request.KonuId,
                    Durum = request.Durum,
                    CreatedBy = userId,
                    CreatedDate = DateTimeExtensions.NowForNpgsqlTimestamp(),
                    IsActive = true,
                };
                _ilerlemeRepository.Add(entity);
                await _ilerlemeRepository.SaveChangesAsync();

                return new SuccessDataResult<SetMyKonuIlerlemeDto>(
                    new SetMyKonuIlerlemeDto
                    {
                        Id = entity.Id,
                        KonuId = entity.KonuId,
                        Durum = (short)entity.Durum,
                    },
                    Messages.Added);
            }
        }
    }
}
