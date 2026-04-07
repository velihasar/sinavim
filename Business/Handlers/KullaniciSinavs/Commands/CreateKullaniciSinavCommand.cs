
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
using Business.Handlers.KullaniciSinavs.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Extensions;

namespace Business.Handlers.KullaniciSinavs.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKullaniciSinavCommand : IRequest<IDataResult<CreateKullaniciSinavDto>>
    {

        public int UserId { get; set; }
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }


        public class CreateKullaniciSinavCommandHandler : IRequestHandler<CreateKullaniciSinavCommand, IDataResult<CreateKullaniciSinavDto>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;
            public CreateKullaniciSinavCommandHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciSinavValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateKullaniciSinavDto>> Handle(CreateKullaniciSinavCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciSinavRecord = _kullaniciSinavRepository.Query().Any(u => u.UserId == request.UserId && u.SinavId == request.SinavId);

                if (isThereKullaniciSinavRecord == true)
                    return new ErrorDataResult<CreateKullaniciSinavDto>(Messages.NameAlreadyExist);

                var addedKullaniciSinav = new KullaniciSinav
                {
                    UserId = request.UserId,
                    SinavId = request.SinavId,
                    HedefPuan = request.HedefPuan,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _kullaniciSinavRepository.Add(addedKullaniciSinav);
                await _kullaniciSinavRepository.SaveChangesAsync();
                
                var dto = new CreateKullaniciSinavDto
                {
                    Id = addedKullaniciSinav.Id,
                    UserId = addedKullaniciSinav.UserId,
                    SinavId = addedKullaniciSinav.SinavId,
                    HedefPuan = addedKullaniciSinav.HedefPuan
                };

                return new SuccessDataResult<CreateKullaniciSinavDto>(dto, Messages.Added);
            }
        }
    }
}