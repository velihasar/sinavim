
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


namespace Business.Handlers.Sinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteSinavCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteSinavCommandHandler : IRequestHandler<DeleteSinavCommand, IResult>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public DeleteSinavCommandHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteSinavCommand request, CancellationToken cancellationToken)
            {
                var sinavToDelete = _sinavRepository.Get(p => p.Id == request.Id);

                _sinavRepository.Delete(sinavToDelete);
                await _sinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

