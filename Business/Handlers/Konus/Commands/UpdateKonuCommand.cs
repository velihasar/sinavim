
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
using Business.Handlers.Konus.ValidationRules;


namespace Business.Handlers.Konus.Commands
{


    public class UpdateKonuCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }

        public class UpdateKonuCommandHandler : IRequestHandler<UpdateKonuCommand, IResult>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public UpdateKonuCommandHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKonuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateKonuCommand request, CancellationToken cancellationToken)
            {
                var isThereKonuRecord = await _konuRepository.GetAsync(u => u.Id == request.Id);


                isThereKonuRecord.Ad = request.Ad;
                isThereKonuRecord.SiraNo = request.SiraNo;
                isThereKonuRecord.DersId = request.DersId;


                _konuRepository.Update(isThereKonuRecord);
                await _konuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

