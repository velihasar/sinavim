
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
using Business.Handlers.Sinavs.ValidationRules;


namespace Business.Handlers.Sinavs.Commands
{


    public class UpdateSinavCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public System.DateTime Tarih { get; set; }

        public class UpdateSinavCommandHandler : IRequestHandler<UpdateSinavCommand, IResult>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public UpdateSinavCommandHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateSinavValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereSinavRecord = await _sinavRepository.GetAsync(u => u.Id == request.Id);


                isThereSinavRecord.Ad = request.Ad;
                isThereSinavRecord.Tarih = request.Tarih;


                _sinavRepository.Update(isThereSinavRecord);
                await _sinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

