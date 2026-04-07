
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
using Business.Handlers.Sinavs.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.Sinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateSinavCommand : IRequest<IResult>
    {

        public string Ad { get; set; }
        public System.DateTime Tarih { get; set; }


        public class CreateSinavCommandHandler : IRequestHandler<CreateSinavCommand, IResult>
        {
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;
            public CreateSinavCommandHandler(ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateSinavValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereSinavRecord = _sinavRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereSinavRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedSinav = new Sinav
                {
                    Ad = request.Ad,
                    Tarih = request.Tarih,

                };

                _sinavRepository.Add(addedSinav);
                await _sinavRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}