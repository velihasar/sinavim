
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


namespace Business.Handlers.DenemeSinavSonucus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteDenemeSinavSonucuCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteDenemeSinavSonucuCommandHandler : IRequestHandler<DeleteDenemeSinavSonucuCommand, IResult>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public DeleteDenemeSinavSonucuCommandHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var denemeSinavSonucuToDelete = _denemeSinavSonucuRepository.Get(p => p.Id == request.Id);

                _denemeSinavSonucuRepository.Delete(denemeSinavSonucuToDelete);
                await _denemeSinavSonucuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

