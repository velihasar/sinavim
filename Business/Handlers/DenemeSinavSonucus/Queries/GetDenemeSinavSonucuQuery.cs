
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


namespace Business.Handlers.DenemeSinavSonucus.Queries
{
    public class GetDenemeSinavSonucuQuery : IRequest<IDataResult<DenemeSinavSonucu>>
    {
        public int Id { get; set; }

        public class GetDenemeSinavSonucuQueryHandler : IRequestHandler<GetDenemeSinavSonucuQuery, IDataResult<DenemeSinavSonucu>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinavSonucuQueryHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinavSonucu>> Handle(GetDenemeSinavSonucuQuery request, CancellationToken cancellationToken)
            {
                var denemeSinavSonucu = await _denemeSinavSonucuRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<DenemeSinavSonucu>(denemeSinavSonucu);
            }
        }
    }
}
