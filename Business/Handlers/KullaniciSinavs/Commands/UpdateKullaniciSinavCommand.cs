
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
using Business.Handlers.KullaniciSinavs.ValidationRules;


namespace Business.Handlers.KullaniciSinavs.Commands
{


    public class UpdateKullaniciSinavCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }

        public class UpdateKullaniciSinavCommandHandler : IRequestHandler<UpdateKullaniciSinavCommand, IResult>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciSinavCommandHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciSinavValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateKullaniciSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciSinavRecord = await _kullaniciSinavRepository.GetAsync(u => u.Id == request.Id);


                isThereKullaniciSinavRecord.UserId = request.UserId;
                isThereKullaniciSinavRecord.SinavId = request.SinavId;
                isThereKullaniciSinavRecord.HedefPuan = request.HedefPuan;


                _kullaniciSinavRepository.Update(isThereKullaniciSinavRecord);
                await _kullaniciSinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

