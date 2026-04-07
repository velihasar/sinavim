
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Business.Handlers.Derses.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteDersCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteDersCommandHandler : IRequestHandler<DeleteDersCommand, IResult>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public DeleteDersCommandHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDersCommand request, CancellationToken cancellationToken)
            {
                var dersToDelete = _dersRepository.Get(p => p.Id == request.Id);

                _dersRepository.Delete(dersToDelete);
                await _dersRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

