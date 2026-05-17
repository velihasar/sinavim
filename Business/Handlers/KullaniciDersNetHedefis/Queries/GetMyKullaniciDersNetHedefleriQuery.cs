using Business.BusinessAspects;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Queries
{
    /// <summary>Oturumdaki kullanıcının ders bazlı deneme net hedefleri.</summary>
    public class GetMyKullaniciDersNetHedefleriQuery
        : IRequest<IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>>
    {
        public class GetMyKullaniciDersNetHedefleriQueryHandler
            : IRequestHandler<GetMyKullaniciDersNetHedefleriQuery,
                IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>>
        {
            private readonly IKullaniciDersNetHedefiRepository _repo;

            public GetMyKullaniciDersNetHedefleriQueryHandler(IKullaniciDersNetHedefiRepository repo)
            {
                _repo = repo;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>> Handle(
                GetMyKullaniciDersNetHedefleriQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<IEnumerable<KullaniciDersNetHedefiDto>>(
                        "Oturum bulunamadı.");
                }

                var entities = await _repo.Query()
                    .Where(x => x.UserId == userId)
                    .OrderBy(x => x.DersId)
                    .ToListAsync(cancellationToken);

                var list = entities.Select(KullaniciDersNetHedefiDto.FromEntity).ToList();
                return new SuccessDataResult<IEnumerable<KullaniciDersNetHedefiDto>>(list);
            }
        }
    }
}
