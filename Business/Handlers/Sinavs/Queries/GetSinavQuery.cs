
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;


namespace Business.Handlers.Sinavs.Queries
{
    public class GetSinavQuery : IRequest<IDataResult<Sinav>>
    {
        public int Id { get; set; }

        public class GetSinavQueryHandler : IRequestHandler<GetSinavQuery, IDataResult<Sinav>>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public GetSinavQueryHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Sinav>> Handle(GetSinavQuery request, CancellationToken cancellationToken)
            {
                var sinav = await _sinavRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Sinav>(sinav);
            }
        }
    }
}
