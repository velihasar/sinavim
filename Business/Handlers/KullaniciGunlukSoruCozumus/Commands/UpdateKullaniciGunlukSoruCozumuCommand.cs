
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules;


namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{


    public class UpdateKullaniciGunlukSoruCozumuCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public System.DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }

        public class UpdateKullaniciGunlukSoruCozumuCommandHandler : IRequestHandler<UpdateKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciGunlukSoruCozumuCommandHandler(IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository, IMediator mediator)
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
                var isThereKullaniciGunlukSoruCozumuRecord = await _kullaniciGunlukSoruCozumuRepository.GetAsync(u => u.Id == request.Id);


                isThereKullaniciGunlukSoruCozumuRecord.UserId = request.UserId;
                isThereKullaniciGunlukSoruCozumuRecord.Tarih = request.Tarih;
                isThereKullaniciGunlukSoruCozumuRecord.CozulenSoruSayisi = request.CozulenSoruSayisi;


                _kullaniciGunlukSoruCozumuRepository.Update(isThereKullaniciGunlukSoruCozumuRecord);
                await _kullaniciGunlukSoruCozumuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

