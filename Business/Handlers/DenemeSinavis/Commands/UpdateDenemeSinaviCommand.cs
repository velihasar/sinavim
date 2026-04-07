
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
using Business.Handlers.DenemeSinavis.ValidationRules;


namespace Business.Handlers.DenemeSinavis.Commands
{


    public class UpdateDenemeSinaviCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public System.DateTime Tarih { get; set; }

        public class UpdateDenemeSinaviCommandHandler : IRequestHandler<UpdateDenemeSinaviCommand, IResult>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public UpdateDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDenemeSinaviValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinaviRecord = await _denemeSinaviRepository.GetAsync(u => u.Id == request.Id);


                isThereDenemeSinaviRecord.Ad = request.Ad;
                isThereDenemeSinaviRecord.Aciklama = request.Aciklama;
                isThereDenemeSinaviRecord.UserId = request.UserId;
                isThereDenemeSinaviRecord.SinavId = request.SinavId;
                isThereDenemeSinaviRecord.Tarih = request.Tarih;


                _denemeSinaviRepository.Update(isThereDenemeSinaviRecord);
                await _denemeSinaviRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

