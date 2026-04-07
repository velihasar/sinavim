
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
using Core.Entities.Dtos.Project.KullaniciSinavDtos;


namespace Business.Handlers.KullaniciSinavs.Queries
{
    public class GetKullaniciSinavQuery : IRequest<IDataResult<KullaniciSinavDto>>
    {
        public int Id { get; set; }

        public class GetKullaniciSinavQueryHandler : IRequestHandler<GetKullaniciSinavQuery, IDataResult<KullaniciSinavDto>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public GetKullaniciSinavQueryHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciSinavDto>> Handle(GetKullaniciSinavQuery request, CancellationToken cancellationToken)
            {
                var kullaniciSinav = await _kullaniciSinavRepository.GetAsync(p => p.Id == request.Id);
                
                if(kullaniciSinav == null)
                {
                    return new ErrorDataResult<KullaniciSinavDto>(null, "Kayıt bulunamadı");
                }

                var dto = new KullaniciSinavDto
                {
                    Id = kullaniciSinav.Id,
                    UserId = kullaniciSinav.UserId,
                    SinavId = kullaniciSinav.SinavId,
                    HedefPuan = kullaniciSinav.HedefPuan
                };

                return new SuccessDataResult<KullaniciSinavDto>(dto);
            }
        }
    }
}
