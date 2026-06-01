
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


namespace Business.Handlers.Arkadasliks.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteArkadaslikCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteArkadaslikCommandHandler : IRequestHandler<DeleteArkadaslikCommand, IResult>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IMediator _mediator;

            public DeleteArkadaslikCommandHandler(IArkadaslikRepository arkadaslikRepository, IMediator mediator)
            {
                _arkadaslikRepository = arkadaslikRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteArkadaslikCommand request, CancellationToken cancellationToken)
            {
                var arkadaslikToDelete = _arkadaslikRepository.Get(p => p.Id == request.Id);

                _arkadaslikRepository.Delete(arkadaslikToDelete);
                await _arkadaslikRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

