
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
using Business.Handlers.KullaniciSinavs.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.KullaniciSinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKullaniciSinavCommand : IRequest<IResult>
    {

        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }


        public class CreateKullaniciSinavCommandHandler : IRequestHandler<CreateKullaniciSinavCommand, IResult>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;
            public CreateKullaniciSinavCommandHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciSinavValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateKullaniciSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciSinavRecord = _kullaniciSinavRepository.Query().Any(u => u.UserId == request.UserId);

                if (isThereKullaniciSinavRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedKullaniciSinav = new KullaniciSinav
                {
                    UserId = request.UserId,
                    SinavId = request.SinavId,
                    HedefPuan = request.HedefPuan,

                };

                _kullaniciSinavRepository.Add(addedKullaniciSinav);
                await _kullaniciSinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}