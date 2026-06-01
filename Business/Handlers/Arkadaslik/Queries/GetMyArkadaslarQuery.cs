using Business.BusinessAspects;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Queries
{
    /// <summary>Oturumdaki kullanıcının arkadaş listesi.</summary>
    public class GetMyArkadaslarQuery : IRequest<IDataResult<IEnumerable<ArkadasOzetDto>>>
    {
        public class GetMyArkadaslarQueryHandler
            : IRequestHandler<GetMyArkadaslarQuery, IDataResult<IEnumerable<ArkadasOzetDto>>>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;

            public GetMyArkadaslarQueryHandler(IArkadaslikRepository arkadaslikRepository)
            {
                _arkadaslikRepository = arkadaslikRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ArkadasOzetDto>>> Handle(
                GetMyArkadaslarQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<IEnumerable<ArkadasOzetDto>>("Oturum bulunamadı.");
                }

                var list = await _arkadaslikRepository.Query()
                    .Include(x => x.UserKucuk)
                    .Include(x => x.UserBuyuk)
                    .Where(x => (x.UserIdKucuk == userId || x.UserIdBuyuk == userId)
                                && x.IsActive != false)
                    .OrderByDescending(x => x.OlusturulmaTarihi)
                    .ToListAsync(cancellationToken);

                var dtos = list.Select(x =>
                {
                    var arkadas = x.UserIdKucuk == userId ? x.UserBuyuk : x.UserKucuk;
                    var arkadasUserId = x.UserIdKucuk == userId ? x.UserIdBuyuk : x.UserIdKucuk;
                    return new ArkadasOzetDto
                    {
                        ArkadaslikId = x.Id,
                        ArkadasUserId = arkadasUserId,
                        ArkadasFullName = arkadas?.FullName,
                        OlusturulmaTarihi = x.OlusturulmaTarihi,
                    };
                }).ToList();

                return new SuccessDataResult<IEnumerable<ArkadasOzetDto>>(dtos);
            }
        }
    }
}
