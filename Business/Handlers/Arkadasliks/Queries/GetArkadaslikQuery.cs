
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using ArkadaslikEntity = Core.Entities.Concrete.Project.Arkadaslik;

namespace Business.Handlers.Arkadasliks.Queries
{
    public class GetArkadaslikQuery : IRequest<IDataResult<ArkadaslikEntity>>
    {
        public int Id { get; set; }

        public class GetArkadaslikQueryHandler : IRequestHandler<GetArkadaslikQuery, IDataResult<ArkadaslikEntity>>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IMediator _mediator;

            public GetArkadaslikQueryHandler(IArkadaslikRepository arkadaslikRepository, IMediator mediator)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ArkadaslikEntity>> Handle(GetArkadaslikQuery request, CancellationToken cancellationToken)
            {
                var arkadaslik = await _arkadaslikRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<ArkadaslikEntity>(arkadaslik);
            }
        }
    }
}
