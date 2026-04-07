
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
using Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos;


namespace Business.Handlers.KullaniciKonuIlerlemes.Queries
{
    public class GetKullaniciKonuIlerlemeQuery : IRequest<IDataResult<KullaniciKonuIlerlemeDto>>
    {
        public int Id { get; set; }

        public class GetKullaniciKonuIlerlemeQueryHandler : IRequestHandler<GetKullaniciKonuIlerlemeQuery, IDataResult<KullaniciKonuIlerlemeDto>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;

            public GetKullaniciKonuIlerlemeQueryHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciKonuIlerlemeDto>> Handle(GetKullaniciKonuIlerlemeQuery request, CancellationToken cancellationToken)
            {
                var kullaniciKonuIlerleme = await _kullaniciKonuIlerlemeRepository.GetAsync(p => p.Id == request.Id);
                
                if(kullaniciKonuIlerleme == null)
                {
                    return new ErrorDataResult<KullaniciKonuIlerlemeDto>(null, "Kayıt bulunamadı");
                }

                var dto = new KullaniciKonuIlerlemeDto
                {
                    Id = kullaniciKonuIlerleme.Id,
                    UserId = kullaniciKonuIlerleme.UserId,
                    KonuId = kullaniciKonuIlerleme.KonuId,
                    Durum = kullaniciKonuIlerleme.Durum
                };

                return new SuccessDataResult<KullaniciKonuIlerlemeDto>(dto);
            }
        }
    }
}
