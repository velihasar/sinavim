
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
using Business.Handlers.Derses.ValidationRules;


namespace Business.Handlers.Derses.Commands
{


    public class UpdateDersCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SinavId { get; set; }

        public class UpdateDersCommandHandler : IRequestHandler<UpdateDersCommand, IResult>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public UpdateDersCommandHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDersValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateDersCommand request, CancellationToken cancellationToken)
            {
                var isThereDersRecord = await _dersRepository.GetAsync(u => u.Id == request.Id);


                isThereDersRecord.Ad = request.Ad;
                isThereDersRecord.SinavId = request.SinavId;


                _dersRepository.Update(isThereDersRecord);
                await _dersRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

