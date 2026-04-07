
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


namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteKullaniciKonuIlerlemeCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteKullaniciKonuIlerlemeCommandHandler : IRequestHandler<DeleteKullaniciKonuIlerlemeCommand, IResult>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public DeleteKullaniciKonuIlerlemeCommandHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteKullaniciKonuIlerlemeCommand request, CancellationToken cancellationToken)
            {
                var kullaniciKonuIlerlemeToDelete = _kullaniciKonuIlerlemeRepository.Get(p => p.Id == request.Id);

                _kullaniciKonuIlerlemeRepository.Delete(kullaniciKonuIlerlemeToDelete);
                await _kullaniciKonuIlerlemeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

