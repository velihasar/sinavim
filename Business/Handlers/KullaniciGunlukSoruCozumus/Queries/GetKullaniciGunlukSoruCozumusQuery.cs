using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Queries
{
    public class GetKullaniciGunlukSoruCozumusQuery : IRequest<IDataResult<IEnumerable<KullaniciGunlukSoruCozumuDto>>>
    {
        public class GetKullaniciGunlukSoruCozumusQueryHandler
            : IRequestHandler<GetKullaniciGunlukSoruCozumusQuery, IDataResult<IEnumerable<KullaniciGunlukSoruCozumuDto>>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public GetKullaniciGunlukSoruCozumusQueryHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository,
                IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciGunlukSoruCozumuDto>>> Handle(
                GetKullaniciGunlukSoruCozumusQuery request,
                CancellationToken cancellationToken)
            {
                var list = await _kullaniciGunlukSoruCozumuRepository.GetListAsync();
                var dtos = list.Select(ToDto).ToList();
                return new SuccessDataResult<IEnumerable<KullaniciGunlukSoruCozumuDto>>(dtos);
            }

            private static KullaniciGunlukSoruCozumuDto ToDto(KullaniciGunlukSoruCozumu e)
            {
                return new KullaniciGunlukSoruCozumuDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Tarih = e.Tarih,
                    CozulenSoruSayisi = e.CozulenSoruSayisi,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    IsActive = e.IsActive,
                };
            }
        }
    }
}
