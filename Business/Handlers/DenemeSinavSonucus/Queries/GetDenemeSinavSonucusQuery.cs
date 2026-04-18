
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Concrete.Project;
using System.Linq;
using Core.Entities.Dtos.Project.DenemeSinavSonucuDtos;

namespace Business.Handlers.DenemeSinavSonucus.Queries
{

    public class GetDenemeSinavSonucusQuery : IRequest<IDataResult<IEnumerable<DenemeSinavSonucuListDto>>>
    {
        public class GetDenemeSinavSonucusQueryHandler : IRequestHandler<GetDenemeSinavSonucusQuery, IDataResult<IEnumerable<DenemeSinavSonucuListDto>>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinavSonucusQueryHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            //[PerformanceAspect(5)]
            //[CacheAspect(10)]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<DenemeSinavSonucuListDto>>> Handle(GetDenemeSinavSonucusQuery request, CancellationToken cancellationToken)
            {
                var list = await _denemeSinavSonucuRepository.GetListAsync();
                var dtoList = list.Select(d => new DenemeSinavSonucuListDto
                {
                    Id = d.Id,
                    DenemeSinaviId = d.DenemeSinaviId,
                    DersId = d.DersId,
                    DogruSayisi = d.DogruSayisi,
                    YanlisSayisi = d.YanlisSayisi,
                    BosSayisi = d.BosSayisi,
                    ToplamNet = d.ToplamNet
                });

                return new SuccessDataResult<IEnumerable<DenemeSinavSonucuListDto>>(dtoList);
            }
        }
    }
}