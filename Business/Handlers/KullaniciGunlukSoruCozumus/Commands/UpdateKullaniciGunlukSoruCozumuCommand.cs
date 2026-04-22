using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{
    public class UpdateKullaniciGunlukSoruCozumuCommand : UpdateKullaniciGunlukSoruCozumuDto, IRequest<IResult>
    {
        public class UpdateKullaniciGunlukSoruCozumuCommandHandler : IRequestHandler<UpdateKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciGunlukSoruCozumuCommandHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository,
                IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciGunlukSoruCozumuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateKullaniciGunlukSoruCozumuCommand request, CancellationToken cancellationToken)
            {
                var entity = await _kullaniciGunlukSoruCozumuRepository.GetAsync(u => u.Id == request.Id);
                if (entity == null)
                {
                    return new ErrorResult(Messages.RecordNotFound);
                }

                entity.UserId = request.UserId;
                entity.Tarih = request.Tarih.ToNpgsqlDateOnly();
                entity.CozulenSoruSayisi = request.CozulenSoruSayisi;

                _kullaniciGunlukSoruCozumuRepository.Update(entity);
                await _kullaniciGunlukSoruCozumuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}
