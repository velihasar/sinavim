
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKullaniciGunlukSoruCozumuCommand : IRequest<IResult>
    {

        public int UserId { get; set; }
        public System.DateTime Tarih { get; set; }
        public int CozulenSoruSayisi { get; set; }


        public class CreateKullaniciGunlukSoruCozumuCommandHandler : IRequestHandler<CreateKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;
            public CreateKullaniciGunlukSoruCozumuCommandHandler(IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository, IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciGunlukSoruCozumuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateKullaniciGunlukSoruCozumuCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciGunlukSoruCozumuRecord = _kullaniciGunlukSoruCozumuRepository.Query().Any(u => u.UserId == request.UserId);

                if (isThereKullaniciGunlukSoruCozumuRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedKullaniciGunlukSoruCozumu = new KullaniciGunlukSoruCozumu
                {
                    UserId = request.UserId,
                    Tarih = request.Tarih,
                    CozulenSoruSayisi = request.CozulenSoruSayisi,

                };

                _kullaniciGunlukSoruCozumuRepository.Add(addedKullaniciGunlukSoruCozumu);
                await _kullaniciGunlukSoruCozumuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}