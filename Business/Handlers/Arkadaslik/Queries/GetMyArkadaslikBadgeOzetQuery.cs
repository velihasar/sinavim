using Business.BusinessAspects;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Queries
{
    /// <summary>Tab rozeti için bekleyen gelen/giden ve görülmemiş kabul sayıları.</summary>
    public class GetMyArkadaslikBadgeOzetQuery : IRequest<IDataResult<ArkadaslikBadgeOzetDto>>
    {
        public class GetMyArkadaslikBadgeOzetQueryHandler
            : IRequestHandler<GetMyArkadaslikBadgeOzetQuery, IDataResult<ArkadaslikBadgeOzetDto>>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;

            public GetMyArkadaslikBadgeOzetQueryHandler(IArkadaslikIstegiRepository istekRepository)
            {
                _istekRepository = istekRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ArkadaslikBadgeOzetDto>> Handle(
                GetMyArkadaslikBadgeOzetQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<ArkadaslikBadgeOzetDto>("Oturum bulunamadı.");
                }

                var bekleyen = await _istekRepository.Query()
                    .Where(x => x.Durum == ArkadaslikIstekDurumu.Beklemede
                                && (x.GonderenUserId == userId || x.HedefUserId == userId))
                    .Select(x => new { x.GonderenUserId, x.HedefUserId })
                    .ToListAsync(cancellationToken);

                var gelenBekleyen = bekleyen.Count(x => x.HedefUserId == userId);
                var gidenBekleyen = bekleyen.Count(x => x.GonderenUserId == userId);

                var kabulGorulmedi = await _istekRepository.Query().CountAsync(
                    x => x.GonderenUserId == userId
                         && x.Durum == ArkadaslikIstekDurumu.Kabul
                         && !x.GonderenKabulGordu,
                    cancellationToken);

                return new SuccessDataResult<ArkadaslikBadgeOzetDto>(
                    new ArkadaslikBadgeOzetDto
                    {
                        GelenBekleyen = gelenBekleyen,
                        GidenBekleyen = gidenBekleyen,
                        KabulGorulmedi = kabulGorulmedi,
                    });
            }
        }
    }
}
