
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
using Business.Handlers.Derses.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.Derses.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDersCommand : IRequest<IResult>
    {

        public string Ad { get; set; }
        public int SinavId { get; set; }


        public class CreateDersCommandHandler : IRequestHandler<CreateDersCommand, IResult>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;
            public CreateDersCommandHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDersValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateDersCommand request, CancellationToken cancellationToken)
            {
                var isThereDersRecord = _dersRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereDersRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedDers = new Ders
                {
                    Ad = request.Ad,
                    SinavId = request.SinavId,

                };

                _dersRepository.Add(addedDers);
                await _dersRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}