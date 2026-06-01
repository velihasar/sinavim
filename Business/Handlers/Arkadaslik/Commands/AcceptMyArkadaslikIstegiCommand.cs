using Business.BusinessAspects;
using Business.Helpers;
using Core.Entities.Concrete.Project;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Commands
{
    /// <summary>Gelen bekleyen arkadaşlık isteğini kabul eder ve Arkadaslik kaydı oluşturur.</summary>
    public class AcceptMyArkadaslikIstegiCommand : IRequest<IResult>
    {
        public int IstekId { get; set; }

        public class AcceptMyArkadaslikIstegiCommandHandler
            : IRequestHandler<AcceptMyArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;
            private readonly IArkadaslikRepository _arkadaslikRepository;

            public AcceptMyArkadaslikIstegiCommandHandler(
                IArkadaslikIstegiRepository istekRepository,
                IArkadaslikRepository arkadaslikRepository)
            {
                _istekRepository = istekRepository;
                _arkadaslikRepository = arkadaslikRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                AcceptMyArkadaslikIstegiCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var istek = await _istekRepository.Query()
                    .FirstOrDefaultAsync(x => x.Id == request.IstekId, cancellationToken);

                if (istek == null)
                {
                    return new ErrorResult("İstek bulunamadı.");
                }

                if (istek.HedefUserId != userId)
                {
                    return new ErrorResult("Bu isteği yanıtlama yetkiniz yok.");
                }

                if (istek.Durum != ArkadaslikIstekDurumu.Beklemede)
                {
                    return new ErrorResult("İstek artık bekleyen durumda değil.");
                }

                if (await ArkadaslikDomainHelper.IsAlreadyFriendsAsync(
                        _arkadaslikRepository,
                        istek.GonderenUserId,
                        istek.HedefUserId,
                        cancellationToken))
                {
                    return new ErrorResult("Bu kullanıcı zaten arkadaşınız.");
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                var (kucuk, buyuk) = ArkadaslikPairHelper.Order(
                    istek.GonderenUserId,
                    istek.HedefUserId);

                var arkadaslik = new Arkadaslik
                {
                    UserIdKucuk = kucuk,
                    UserIdBuyuk = buyuk,
                    ArkadaslikIstegiId = istek.Id,
                    OlusturulmaTarihi = now,
                    CreatedBy = userId,
                    CreatedDate = now,
                    IsActive = true,
                };

                istek.Durum = ArkadaslikIstekDurumu.Kabul;
                istek.YanitTarihi = now;
                istek.GonderenKabulGordu = false;
                istek.UpdatedBy = userId;
                istek.UpdatedDate = now;

                _arkadaslikRepository.Add(arkadaslik);
                _istekRepository.Update(istek);
                await _arkadaslikRepository.SaveChangesAsync();

                return new SuccessResult("Arkadaşlık isteği kabul edildi.");
            }
        }
    }
}
