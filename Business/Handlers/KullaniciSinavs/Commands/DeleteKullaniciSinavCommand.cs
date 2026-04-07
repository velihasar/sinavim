
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


namespace Business.Handlers.KullaniciSinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteKullaniciSinavCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteKullaniciSinavCommandHandler : IRequestHandler<DeleteKullaniciSinavCommand, IResult>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public DeleteKullaniciSinavCommandHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteKullaniciSinavCommand request, CancellationToken cancellationToken)
            {
                var kullaniciSinavToDelete = _kullaniciSinavRepository.Get(p => p.Id == request.Id);

                _kullaniciSinavRepository.Delete(kullaniciSinavToDelete);
                await _kullaniciSinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

