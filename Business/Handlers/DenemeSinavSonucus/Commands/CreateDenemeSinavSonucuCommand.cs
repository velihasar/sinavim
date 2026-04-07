
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
using Business.Handlers.DenemeSinavSonucus.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinavSonucuDtos;
using Core.Extensions;

namespace Business.Handlers.DenemeSinavSonucus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDenemeSinavSonucuCommand : IRequest<IDataResult<CreateDenemeSinavSonucuDto>>
    {

        public int DersId { get; set; }
        public int DogruSayisi { get; set; }
        public int YanlisSayisi { get; set; }
        public int BosSayisi { get; set; }
        public decimal ToplamNet { get; set; }


        public class CreateDenemeSinavSonucuCommandHandler : IRequestHandler<CreateDenemeSinavSonucuCommand, IDataResult<CreateDenemeSinavSonucuDto>>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IMediator _mediator;
            public CreateDenemeSinavSonucuCommandHandler(IDenemeSinavSonucuRepository denemeSinavSonucuRepository, IMediator mediator)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDenemeSinavSonucuValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateDenemeSinavSonucuDto>> Handle(CreateDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var isThereDenemeSinavSonucuRecord = _denemeSinavSonucuRepository.Query().Any(u => u.DersId == request.DersId);

                if (isThereDenemeSinavSonucuRecord == true)
                    return new ErrorDataResult<CreateDenemeSinavSonucuDto>(Messages.NameAlreadyExist);

                var addedDenemeSinavSonucu = new DenemeSinavSonucu
                {
                    DersId = request.DersId,
                    DogruSayisi = request.DogruSayisi,
                    YanlisSayisi = request.YanlisSayisi,
                    BosSayisi = request.BosSayisi,
                    ToplamNet = request.ToplamNet,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _denemeSinavSonucuRepository.Add(addedDenemeSinavSonucu);
                await _denemeSinavSonucuRepository.SaveChangesAsync();
                
                var dto = new CreateDenemeSinavSonucuDto
                {
                    Id = addedDenemeSinavSonucu.Id,
                    DersId = addedDenemeSinavSonucu.DersId,
                    DogruSayisi = addedDenemeSinavSonucu.DogruSayisi,
                    YanlisSayisi = addedDenemeSinavSonucu.YanlisSayisi,
                    BosSayisi = addedDenemeSinavSonucu.BosSayisi,
                    ToplamNet = addedDenemeSinavSonucu.ToplamNet
                };

                return new SuccessDataResult<CreateDenemeSinavSonucuDto>(dto, Messages.Added);
            }
        }
    }
}