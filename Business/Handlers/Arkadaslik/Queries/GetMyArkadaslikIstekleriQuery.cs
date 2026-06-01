using Business.BusinessAspects;
using Business.Helpers;
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
    /// <summary>Bekleyen gelen ve giden arkadaşlık istekleri.</summary>
    public class GetMyArkadaslikIstekleriQuery : IRequest<IDataResult<MyArkadaslikIstekleriDto>>
    {
        public class GetMyArkadaslikIstekleriQueryHandler
            : IRequestHandler<GetMyArkadaslikIstekleriQuery, IDataResult<MyArkadaslikIstekleriDto>>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;

            public GetMyArkadaslikIstekleriQueryHandler(IArkadaslikIstegiRepository istekRepository)
            {
                _istekRepository = istekRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<MyArkadaslikIstekleriDto>> Handle(
                GetMyArkadaslikIstekleriQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<MyArkadaslikIstekleriDto>("Oturum bulunamadı.");
                }

                var list = await _istekRepository.Query()
                    .Include(x => x.GonderenUser)
                    .Include(x => x.HedefUser)
                    .Where(x => x.Durum == ArkadaslikIstekDurumu.Beklemede
                                && (x.GonderenUserId == userId || x.HedefUserId == userId))
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(cancellationToken);

                var dto = new MyArkadaslikIstekleriDto
                {
                    Gelen = list
                        .Where(x => x.HedefUserId == userId)
                        .Select(Map)
                        .ToList(),
                    Giden = list
                        .Where(x => x.GonderenUserId == userId)
                        .Select(MapGiden)
                        .ToList(),
                };

                return new SuccessDataResult<MyArkadaslikIstekleriDto>(dto);
            }

            private static ArkadaslikIstegiListItemDto Map(Core.Entities.Concrete.Project.ArkadaslikIstegi x)
            {
                return new ArkadaslikIstegiListItemDto
                {
                    Id = x.Id,
                    GonderenUserId = x.GonderenUserId,
                    GonderenFullName = x.GonderenUser?.FullName,
                    HedefUserId = x.HedefUserId,
                    HedefFullName = x.HedefUser?.FullName,
                    Durum = x.Durum,
                    OlusturulmaTarihi = x.OlusturulmaTarihi,
                    YanitTarihi = x.YanitTarihi,
                };
            }

            private static ArkadaslikIstegiListItemDto MapGiden(Core.Entities.Concrete.Project.ArkadaslikIstegi x)
            {
                var dto = Map(x);
                dto.HedefFullName = ArkadasDisplayNameHelper.MaskToInitials(dto.HedefFullName);
                return dto;
            }
        }
    }
}
