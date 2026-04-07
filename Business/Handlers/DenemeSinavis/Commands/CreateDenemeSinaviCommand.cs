
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
using Business.Handlers.DenemeSinavis.ValidationRules;
using Core.Entities.Concrete.Project;

namespace Business.Handlers.DenemeSinavis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDenemeSinaviCommand : IRequest<IResult>
    {

        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public System.DateTime Tarih { get; set; }


        public class CreateDenemeSinaviCommandHandler : IRequestHandler<CreateDenemeSinaviCommand, IResult>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;
            public CreateDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDenemeSinaviValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinaviRecord = _denemeSinaviRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereDenemeSinaviRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedDenemeSinavi = new DenemeSinavi
                {
                    Ad = request.Ad,
                    Aciklama = request.Aciklama,
                    UserId = request.UserId,
                    SinavId = request.SinavId,
                    Tarih = request.Tarih,

                };

                _denemeSinaviRepository.Add(addedDenemeSinavi);
                await _denemeSinaviRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}