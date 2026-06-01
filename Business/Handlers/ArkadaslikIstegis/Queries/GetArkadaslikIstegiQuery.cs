
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


namespace Business.Handlers.ArkadaslikIstegis.Queries
{
    public class GetArkadaslikIstegiQuery : IRequest<IDataResult<ArkadaslikIstegi>>
    {
        public int Id { get; set; }

        public class GetArkadaslikIstegiQueryHandler : IRequestHandler<GetArkadaslikIstegiQuery, IDataResult<ArkadaslikIstegi>>
        {
            private readonly IArkadaslikIstegiRepository _arkadaslikIstegiRepository;
            private readonly IMediator _mediator;

            public GetArkadaslikIstegiQueryHandler(IArkadaslikIstegiRepository arkadaslikIstegiRepository, IMediator mediator)
            {
                _arkadaslikIstegiRepository = arkadaslikIstegiRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ArkadaslikIstegi>> Handle(GetArkadaslikIstegiQuery request, CancellationToken cancellationToken)
            {
                var arkadaslikIstegi = await _arkadaslikIstegiRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<ArkadaslikIstegi>(arkadaslikIstegi);
            }
        }
    }
}
