
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
using Business.Handlers.DenemeSinavSonucus.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.DenemeSinavSonucus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDenemeSinavSonucuCommand : IRequest<IResult>
    {

        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }


        public class CreateDenemeSinavSonucuCommandHandler : IRequestHandler<CreateDenemeSinavSonucuCommand, IResult>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;
            public CreateDenemeSinavSonucuCommandHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDenemeSinavSonucuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinavSonucuRecord = _denemeSinavSonucuRepository.Query().Any(u => u.DersId == request.DersId);

                if (isThereDenemeSinavSonucuRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedDenemeSinavSonucu = new DenemeSinavSonucu
                {
                    DersId = request.DersId,
                    DogruSayisi = request.DogruSayisi,
                    YanlisSayisi = request.YanlisSayisi,
                    BosSayisi = request.BosSayisi,
                    ToplamNet = request.ToplamNet,

                };

                _denemeSinavSonucuRepository.Add(addedDenemeSinavSonucu);
                await _denemeSinavSonucuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}