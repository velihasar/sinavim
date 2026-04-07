
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


namespace Business.Handlers.DenemeSinavis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteDenemeSinaviCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteDenemeSinaviCommandHandler : IRequestHandler<DeleteDenemeSinaviCommand, IResult>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public DeleteDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

          // [CacheRemoveAspect("Get")]
          //  [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var denemeSinaviToDelete = _denemeSinaviRepository.Get(p => p.Id == request.Id);

                _denemeSinaviRepository.Delete(denemeSinaviToDelete);
                await _denemeSinaviRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

