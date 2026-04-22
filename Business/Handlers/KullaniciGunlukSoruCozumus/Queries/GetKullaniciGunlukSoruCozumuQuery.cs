
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


namespace Business.Handlers.KullaniciGunlukSoruCozumus.Queries
{
    public class GetKullaniciGunlukSoruCozumuQuery : IRequest<IDataResult<KullaniciGunlukSoruCozumu>>
    {
        public int Id { get; set; }

        public class GetKullaniciGunlukSoruCozumuQueryHandler : IRequestHandler<GetKullaniciGunlukSoruCozumuQuery, IDataResult<KullaniciGunlukSoruCozumu>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public GetKullaniciGunlukSoruCozumuQueryHandler(IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository, IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciGunlukSoruCozumu>> Handle(GetKullaniciGunlukSoruCozumuQuery request, CancellationToken cancellationToken)
            {
                var kullaniciGunlukSoruCozumu = await _kullaniciGunlukSoruCozumuRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<KullaniciGunlukSoruCozumu>(kullaniciGunlukSoruCozumu);
            }
        }
    }
}
