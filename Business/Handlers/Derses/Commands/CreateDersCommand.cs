
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
using Core.Entities.Dtos.Project.DersDtos;
using Core.Extensions;

namespace Business.Handlers.Derses.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDersCommand : IRequest<IDataResult<CreateDersDto>>
    {

        public string Ad { get; set; }
        public int SinavId { get; set; }
        public int? SinavBolumId { get; set; }


        public class CreateDersCommandHandler : IRequestHandler<CreateDersCommand, IDataResult<CreateDersDto>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IMediator _mediator;
            public CreateDersCommandHandler(IDersRepository dersRepository, ISinavBolumRepository sinavBolumRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _sinavBolumRepository = sinavBolumRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDersValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateDersDto>> Handle(CreateDersCommand request, CancellationToken cancellationToken)
            {
                if (request.SinavBolumId.HasValue)
                {
                    var bolum = await _sinavBolumRepository.GetAsync(b => b.Id == request.SinavBolumId.Value);
                    if (bolum == null || bolum.SinavId != request.SinavId)
                        return new ErrorDataResult<CreateDersDto>("Sınav bölümü bulunamadı veya seçilen sınava ait değil.");
                }

                var isThereDersRecord = _dersRepository.Query().Any(u =>
                    u.Ad == request.Ad
                    && u.SinavId == request.SinavId
                    && u.SinavBolumId == request.SinavBolumId);

                if (isThereDersRecord == true)
                    return new ErrorDataResult<CreateDersDto>(Messages.NameAlreadyExist);

                var addedDers = new Ders
                {
                    Ad = request.Ad,
                    SinavId = request.SinavId,
                    SinavBolumId = request.SinavBolumId,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _dersRepository.Add(addedDers);
                await _dersRepository.SaveChangesAsync();

                var dto = new CreateDersDto
                {
                    Id = addedDers.Id,
                    Ad = addedDers.Ad,
                    SinavId = addedDers.SinavId,
                    SinavBolumId = addedDers.SinavBolumId
                };

                return new SuccessDataResult<CreateDersDto>(dto, Messages.Added);
            }
        }
    }
}