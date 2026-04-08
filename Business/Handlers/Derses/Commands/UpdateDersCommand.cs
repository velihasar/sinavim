
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
using Core.Entities.Dtos.Project.DersDtos;
using Core.Extensions;

namespace Business.Handlers.Derses.Commands
{


    public class UpdateDersCommand : IRequest<IDataResult<UpdateDersDto>>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }

        public class UpdateDersCommandHandler : IRequestHandler<UpdateDersCommand, IDataResult<UpdateDersDto>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IMediator _mediator;

            public UpdateDersCommandHandler(IDersRepository dersRepository, ISinavBolumRepository sinavBolumRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _sinavBolumRepository = sinavBolumRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDersValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateDersDto>> Handle(UpdateDersCommand request, CancellationToken cancellationToken)
            {
                var isThereDersRecord = await _dersRepository.GetAsync(u => u.Id == request.Id);

                if(isThereDersRecord == null)
                {
                    return new ErrorDataResult<UpdateDersDto>("Kayıt bulunamadı");
                }

                if (request.SinavBolumId.HasValue)
                {
                    var bolum = await _sinavBolumRepository.GetAsync(b => b.Id == request.SinavBolumId.Value);
                    if (bolum == null || bolum.SinavId != request.SinavId)
                        return new ErrorDataResult<UpdateDersDto>("Sınav bölümü bulunamadı veya seçilen sınava ait değil.");
                }

                var duplicate = _dersRepository.Query().Any(u =>
                    u.Id != request.Id
                    && u.Ad == request.Ad
                    && u.SinavId == request.SinavId
                    && u.SinavBolumId == request.SinavBolumId);
                if (duplicate)
                    return new ErrorDataResult<UpdateDersDto>(Messages.NameAlreadyExist);

                isThereDersRecord.Ad = request.Ad;
                isThereDersRecord.SinavId = request.SinavId;
                isThereDersRecord.SinavBolumId = request.SinavBolumId;
                isThereDersRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereDersRecord.UpdatedDate = System.DateTime.Now;


                _dersRepository.Update(isThereDersRecord);
                await _dersRepository.SaveChangesAsync();

                var dto = new UpdateDersDto
                {
                    Id = isThereDersRecord.Id,
                    Ad = isThereDersRecord.Ad,
                    SinavId = isThereDersRecord.SinavId,
                    SinavBolumId = isThereDersRecord.SinavBolumId
                };

                return new SuccessDataResult<UpdateDersDto>(dto, Messages.Updated);
            }
        }
    }
}

