using Business.BusinessAspects;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Commands
{
    /// <summary>Gönderen olarak kabul edilmiş ve henüz görülmemiş istekleri okundu işaretler.</summary>
    public class MarkMyKabulBildirimleriGorulduCommand : IRequest<IResult>
    {
        public class MarkMyKabulBildirimleriGorulduCommandHandler
            : IRequestHandler<MarkMyKabulBildirimleriGorulduCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;

            public MarkMyKabulBildirimleriGorulduCommandHandler(
                IArkadaslikIstegiRepository istekRepository)
            {
                _istekRepository = istekRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                MarkMyKabulBildirimleriGorulduCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                var list = await _istekRepository.Query()
                    .Where(x => x.GonderenUserId == userId
                                && x.Durum == ArkadaslikIstekDurumu.Kabul
                                && !x.GonderenKabulGordu)
                    .ToListAsync(cancellationToken);

                foreach (var istek in list)
                {
                    istek.GonderenKabulGordu = true;
                    istek.UpdatedBy = userId;
                    istek.UpdatedDate = now;
                    _istekRepository.Update(istek);
                }

                if (list.Count > 0)
                {
                    await _istekRepository.SaveChangesAsync();
                }

                return new SuccessResult("Okundu olarak işaretlendi.");
            }
        }
    }
}
