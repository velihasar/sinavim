
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
using Business.Handlers.Konus.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.Konus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKonuCommand : IRequest<IResult>
    {

        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }


        public class CreateKonuCommandHandler : IRequestHandler<CreateKonuCommand, IResult>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;
            public CreateKonuCommandHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKonuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateKonuCommand request, CancellationToken cancellationToken)
            {
                var isThereKonuRecord = _konuRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereKonuRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedKonu = new Konu
                {
                    Ad = request.Ad,
                    SiraNo = request.SiraNo,
                    DersId = request.DersId,

                };

                _konuRepository.Add(addedKonu);
                await _konuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}