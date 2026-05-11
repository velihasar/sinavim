
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Queries
{
    public class GetKullaniciDersNetHedefisQuery : IRequest<IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>>
    {
        public class GetKullaniciDersNetHedefisQueryHandler
            : IRequestHandler<GetKullaniciDersNetHedefisQuery, IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>>
        {
            private readonly IKullaniciDersNetHedefiRepository _kullaniciDersNetHedefiRepository;
            private readonly IMediator _mediator;

            public GetKullaniciDersNetHedefisQueryHandler(
                IKullaniciDersNetHedefiRepository kullaniciDersNetHedefiRepository,
                IMediator mediator)
            {
                _kullaniciDersNetHedefiRepository = kullaniciDersNetHedefiRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KullaniciDersNetHedefiDto>>> Handle(
                GetKullaniciDersNetHedefisQuery request,
                CancellationToken cancellationToken)
            {
                var list = await _kullaniciDersNetHedefiRepository.GetListAsync();
                var dtos = list.Select(KullaniciDersNetHedefiDto.FromEntity).ToList();
                return new SuccessDataResult<IEnumerable<KullaniciDersNetHedefiDto>>(dtos);
            }
        }
    }
}
