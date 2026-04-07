
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


namespace Business.Handlers.Derses.Queries
{
    public class GetDersQuery : IRequest<IDataResult<Ders>>
    {
        public int Id { get; set; }

        public class GetDersQueryHandler : IRequestHandler<GetDersQuery, IDataResult<Ders>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public GetDersQueryHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Ders>> Handle(GetDersQuery request, CancellationToken cancellationToken)
            {
                var ders = await _dersRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Ders>(ders);
            }
        }
    }
}
