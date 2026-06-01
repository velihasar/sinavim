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
    /// <summary>Gelen bekleyen arkadaşlık isteğini reddeder.</summary>
    public class RejectMyArkadaslikIstegiCommand : IRequest<IResult>
    {
        public int IstekId { get; set; }

        public class RejectMyArkadaslikIstegiCommandHandler
            : IRequestHandler<RejectMyArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;

            public RejectMyArkadaslikIstegiCommandHandler(IArkadaslikIstegiRepository istekRepository)
            {
                _istekRepository = istekRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                RejectMyArkadaslikIstegiCommand request,
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
                    return new ErrorResult("Bu isteği reddetme yetkiniz yok.");
                }

                if (istek.Durum != ArkadaslikIstekDurumu.Beklemede)
                {
                    return new ErrorResult("İstek artık bekleyen durumda değil.");
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                istek.Durum = ArkadaslikIstekDurumu.Red;
                istek.YanitTarihi = now;
                istek.UpdatedBy = userId;
                istek.UpdatedDate = now;

                _istekRepository.Update(istek);
                await _istekRepository.SaveChangesAsync();

                return new SuccessResult("Arkadaşlık isteği reddedildi.");
            }
        }
    }
}
