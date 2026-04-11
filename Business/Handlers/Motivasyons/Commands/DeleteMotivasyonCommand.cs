
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


namespace Business.Handlers.Motivasyons.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteMotivasyonCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteMotivasyonCommandHandler : IRequestHandler<DeleteMotivasyonCommand, IResult>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public DeleteMotivasyonCommandHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteMotivasyonCommand request, CancellationToken cancellationToken)
            {
                var motivasyonToDelete = _motivasyonRepository.Get(p => p.Id == request.Id);

                _motivasyonRepository.Delete(motivasyonToDelete);
                await _motivasyonRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

