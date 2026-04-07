
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
using Business.Handlers.KullaniciKonuIlerlemes.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciKonuIlerlemeDtos;
using Core.Extensions;

namespace Business.Handlers.KullaniciKonuIlerlemes.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKullaniciKonuIlerlemeCommand : IRequest<IDataResult<CreateKullaniciKonuIlerlemeDto>>
    {

        public int UserId { get; set; }
        public int KonuId { get; set; }
        public Core.Enums.IlerlemeDurumu Durum { get; set; }


        public class CreateKullaniciKonuIlerlemeCommandHandler : IRequestHandler<CreateKullaniciKonuIlerlemeCommand, IDataResult<CreateKullaniciKonuIlerlemeDto>>
        {
            private readonly IKullaniciKonuIlerlemeRepository _kullaniciKonuIlerlemeRepository;
            private readonly IMediator _mediator;
            public CreateKullaniciKonuIlerlemeCommandHandler(IKullaniciKonuIlerlemeRepository kullaniciKonuIlerlemeRepository, IMediator mediator)
            {
                _kullaniciKonuIlerlemeRepository = kullaniciKonuIlerlemeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciKonuIlerlemeValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateKullaniciKonuIlerlemeDto>> Handle(CreateKullaniciKonuIlerlemeCommand request, CancellationToken cancellationToken)
            {
                var isThereKullaniciKonuIlerlemeRecord = _kullaniciKonuIlerlemeRepository.Query().Any(u => u.UserId == request.UserId && u.KonuId == request.KonuId);

                if (isThereKullaniciKonuIlerlemeRecord == true)
                    return new ErrorDataResult<CreateKullaniciKonuIlerlemeDto>(Messages.NameAlreadyExist);

                var addedKullaniciKonuIlerleme = new KullaniciKonuIlerleme
                {
                    UserId = request.UserId,
                    KonuId = request.KonuId,
                    Durum = request.Durum,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _kullaniciKonuIlerlemeRepository.Add(addedKullaniciKonuIlerleme);
                await _kullaniciKonuIlerlemeRepository.SaveChangesAsync();
                
                var dto = new CreateKullaniciKonuIlerlemeDto
                {
                    Id = addedKullaniciKonuIlerleme.Id,
                    UserId = addedKullaniciKonuIlerleme.UserId,
                    KonuId = addedKullaniciKonuIlerleme.KonuId,
                    Durum = addedKullaniciKonuIlerleme.Durum
                };

                return new SuccessDataResult<CreateKullaniciKonuIlerlemeDto>(dto, Messages.Added);
            }
        }
    }
}