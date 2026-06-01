using Business.BusinessAspects;
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
    /// <summary>Gönderdiğiniz bekleyen arkadaşlık isteğini iptal eder.</summary>
    public class CancelMyArkadaslikIstegiCommand : IRequest<IResult>
    {
        public int IstekId { get; set; }

        public class CancelMyArkadaslikIstegiCommandHandler
            : IRequestHandler<CancelMyArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;

            public CancelMyArkadaslikIstegiCommandHandler(IArkadaslikIstegiRepository istekRepository)
            {
                _istekRepository = istekRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                CancelMyArkadaslikIstegiCommand request,
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

                if (istek.GonderenUserId != userId)
                {
                    return new ErrorResult("Bu isteği iptal etme yetkiniz yok.");
                }

                if (istek.Durum != ArkadaslikIstekDurumu.Beklemede)
                {
                    return new ErrorResult("İstek artık bekleyen durumda değil.");
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                istek.Durum = ArkadaslikIstekDurumu.Iptal;
                istek.YanitTarihi = now;
                istek.UpdatedBy = userId;
                istek.UpdatedDate = now;

                _istekRepository.Update(istek);
                await _istekRepository.SaveChangesAsync();

                return new SuccessResult("Arkadaşlık isteği iptal edildi.");
            }
        }
    }
}
