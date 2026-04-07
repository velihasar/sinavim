
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
using Business.Handlers.DenemeSinavSonucus.ValidationRules;


namespace Business.Handlers.DenemeSinavSonucus.Commands
{


    public class UpdateDenemeSinavSonucuCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }

        public class UpdateDenemeSinavSonucuCommandHandler : IRequestHandler<UpdateDenemeSinavSonucuCommand, IResult>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;

            public UpdateDenemeSinavSonucuCommandHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDenemeSinavSonucuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinavSonucuRecord = await _denemeSinavSonucuRepository.GetAsync(u => u.Id == request.Id);


                isThereDenemeSinavSonucuRecord.DersId = request.DersId;
                isThereDenemeSinavSonucuRecord.DogruSayisi = request.DogruSayisi;
                isThereDenemeSinavSonucuRecord.YanlisSayisi = request.YanlisSayisi;
                isThereDenemeSinavSonucuRecord.BosSayisi = request.BosSayisi;
                isThereDenemeSinavSonucuRecord.ToplamNet = request.ToplamNet;


                _denemeSinavSonucuRepository.Update(isThereDenemeSinavSonucuRecord);
                await _denemeSinavSonucuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

