
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;


namespace Business.Handlers.KullaniciKonuIlerlemes.Queries
{
    public class GetKullaniciKonuIlerlemeQuery : IRequest<IDataResult<KullaniciKonuIlerleme>>
    {
        public int Id { get; set; }

        public class GetKullaniciKonuIlerlemeQueryHandler : IRequestHandler<GetKullaniciKonuIlerlemeQuery, IDataResult<KullaniciKonuIlerleme>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public GetKullaniciKonuIlerlemeQueryHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciKonuIlerleme>> Handle(GetKullaniciKonuIlerlemeQuery request, CancellationToken cancellationToken)
            {
                var kullaniciKonuIlerleme = await _kullaniciKonuIlerlemeRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<KullaniciKonuIlerleme>(kullaniciKonuIlerleme);
            }
        }
    }
}
