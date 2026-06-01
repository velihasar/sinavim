
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


namespace Business.Handlers.ArkadaslikIstegis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteArkadaslikIstegiCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteArkadaslikIstegiCommandHandler : IRequestHandler<DeleteArkadaslikIstegiCommand, IResult>
        {
            private readonly IArkadaslikIstegiRepository _arkadaslikIstegiRepository;
            private readonly IMediator _mediator;

            public DeleteArkadaslikIstegiCommandHandler(IArkadaslikIstegiRepository arkadaslikIstegiRepository, IMediator mediator)
            {
                _arkadaslikIstegiRepository = arkadaslikIstegiRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteArkadaslikIstegiCommand request, CancellationToken cancellationToken)
            {
                var arkadaslikIstegiToDelete = _arkadaslikIstegiRepository.Get(p => p.Id == request.Id);

                _arkadaslikIstegiRepository.Delete(arkadaslikIstegiToDelete);
                await _arkadaslikIstegiRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

