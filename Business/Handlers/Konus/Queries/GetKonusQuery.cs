
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
using Core.Entities.Dtos.Project.KonuDtos;

namespace Business.Handlers.Konus.Queries
{

    public class GetKonusQuery : IRequest<IDataResult<IEnumerable<KonuListDto>>>
    {
        public class GetKonusQueryHandler : IRequestHandler<GetKonusQuery, IDataResult<IEnumerable<KonuListDto>>>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public GetKonusQueryHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            //[PerformanceAspect(5)]
            //[CacheAspect(10)]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<KonuListDto>>> Handle(GetKonusQuery request, CancellationToken cancellationToken)
            {
                var list = await _konuRepository.GetListAsync();
                var dtoList = list.Select(k => new KonuListDto
                {
                    Id = k.Id,
                    Ad = k.Ad,
                    SiraNo = k.SiraNo,
                    DersId = k.DersId
                });

                return new SuccessDataResult<IEnumerable<KonuListDto>>(dtoList);
            }
        }
    }
}