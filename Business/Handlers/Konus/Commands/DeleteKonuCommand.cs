
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


namespace Business.Handlers.Konus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteKonuCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteKonuCommandHandler : IRequestHandler<DeleteKonuCommand, IResult>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public DeleteKonuCommandHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteKonuCommand request, CancellationToken cancellationToken)
            {
                var konuToDelete = _konuRepository.Get(p => p.Id == request.Id);

                _konuRepository.Delete(konuToDelete);
                await _konuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

