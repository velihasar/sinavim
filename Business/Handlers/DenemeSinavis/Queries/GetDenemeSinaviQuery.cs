
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


namespace Business.Handlers.DenemeSinavis.Queries
{
    public class GetDenemeSinaviQuery : IRequest<IDataResult<DenemeSinavi>>
    {
        public int Id { get; set; }

        public class GetDenemeSinaviQueryHandler : IRequestHandler<GetDenemeSinaviQuery, IDataResult<DenemeSinavi>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public GetDenemeSinaviQueryHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinavi>> Handle(GetDenemeSinaviQuery request, CancellationToken cancellationToken)
            {
                var denemeSinavi = await _denemeSinaviRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<DenemeSinavi>(denemeSinavi);
            }
        }
    }
}
