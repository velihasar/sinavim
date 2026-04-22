using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Queries
{
    public class GetKullaniciGunlukSoruCozumuQuery : IRequest<IDataResult<KullaniciGunlukSoruCozumuDto>>
    {
        public int Id { get; set; }

        public class GetKullaniciGunlukSoruCozumuQueryHandler
            : IRequestHandler<GetKullaniciGunlukSoruCozumuQuery, IDataResult<KullaniciGunlukSoruCozumuDto>>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public GetKullaniciGunlukSoruCozumuQueryHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository,
                IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciGunlukSoruCozumuDto>> Handle(
                GetKullaniciGunlukSoruCozumuQuery request,
                CancellationToken cancellationToken)
            {
                var entity = await _kullaniciGunlukSoruCozumuRepository.GetAsync(p => p.Id == request.Id);
                if (entity == null)
                {
                    return new ErrorDataResult<KullaniciGunlukSoruCozumuDto>(Messages.RecordNotFound);
                }

                return new SuccessDataResult<KullaniciGunlukSoruCozumuDto>(ToDto(entity));
            }

            private static KullaniciGunlukSoruCozumuDto ToDto(KullaniciGunlukSoruCozumu e)
            {
                return new KullaniciGunlukSoruCozumuDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Tarih = e.Tarih,
                    CozulenSoruSayisi = e.CozulenSoruSayisi,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    IsActive = e.IsActive,
                };
            }
        }
    }
}
