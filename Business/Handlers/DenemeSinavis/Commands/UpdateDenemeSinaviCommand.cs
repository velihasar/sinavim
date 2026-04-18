
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
using Core.Entities.Dtos.Project.DenemeSinaviDtos;
using Core.Extensions;

namespace Business.Handlers.DenemeSinavis.Commands
{


    public class UpdateDenemeSinaviCommand : IRequest<IDataResult<UpdateDenemeSinaviDto>>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }
        public System.DateTime Tarih { get; set; }

        public class UpdateDenemeSinaviCommandHandler : IRequestHandler<UpdateDenemeSinaviCommand, IDataResult<UpdateDenemeSinaviDto>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IMediator _mediator;

            public UpdateDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository, IMediator mediator)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDenemeSinaviValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateDenemeSinaviDto>> Handle(UpdateDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinaviRecord = await _denemeSinaviRepository.GetAsync(u => u.Id == request.Id);

                if(isThereDenemeSinaviRecord == null)
                {
                    return new ErrorDataResult<UpdateDenemeSinaviDto>("Kayıt bulunamadı");
                }

                isThereDenemeSinaviRecord.Ad = request.Ad;
                isThereDenemeSinaviRecord.Aciklama = request.Aciklama;
                isThereDenemeSinaviRecord.UserId = request.UserId;
                isThereDenemeSinaviRecord.SinavId = request.SinavId;
                isThereDenemeSinaviRecord.SinavBolumId = request.SinavBolumId;
                isThereDenemeSinaviRecord.Tarih = request.Tarih;
                isThereDenemeSinaviRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereDenemeSinaviRecord.UpdatedDate = System.DateTime.Now;


                _denemeSinaviRepository.Update(isThereDenemeSinaviRecord);
                await _denemeSinaviRepository.SaveChangesAsync();

                var dto = new UpdateDenemeSinaviDto
                {
                    Id = isThereDenemeSinaviRecord.Id,
                    Ad = isThereDenemeSinaviRecord.Ad,
                    Aciklama = isThereDenemeSinaviRecord.Aciklama,
                    UserId = isThereDenemeSinaviRecord.UserId,
                    SinavId = isThereDenemeSinaviRecord.SinavId,
                    SinavBolumId = isThereDenemeSinaviRecord.SinavBolumId,
                    Tarih = isThereDenemeSinaviRecord.Tarih
                };

                return new SuccessDataResult<UpdateDenemeSinaviDto>(dto, Messages.Updated);
            }
        }
    }
}

