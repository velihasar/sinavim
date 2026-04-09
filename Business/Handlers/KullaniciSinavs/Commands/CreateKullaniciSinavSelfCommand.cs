
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciSinavs.ValidationRules;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciSinavs.Commands
{
    /// <summary>
    /// Oturumdaki kullanıcı için sınav seçimi (UserId istemciden alınmaz).
    /// </summary>
    public class CreateKullaniciSinavSelfCommand : IRequest<IDataResult<CreateKullaniciSinavDto>>
    {
        public int SinavId { get; set; }
        public int HedefPuan { get; set; }

        public class CreateKullaniciSinavSelfCommandHandler
            : IRequestHandler<CreateKullaniciSinavSelfCommand, IDataResult<CreateKullaniciSinavDto>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly ISinavRepository _sinavRepository;

            public CreateKullaniciSinavSelfCommandHandler(
                IKullaniciSinavRepository kullaniciSinavRepository,
                ISinavRepository sinavRepository)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _sinavRepository = sinavRepository;
            }

            [ValidationAspect(typeof(CreateKullaniciSinavSelfValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateKullaniciSinavDto>> Handle(
                CreateKullaniciSinavSelfCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Oturum bulunamadı.");
                }

                var hasAny = _kullaniciSinavRepository.Query().Any(k => k.UserId == userId);
                if (hasAny)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Zaten bir sınav seçiminiz kayıtlı.");
                }

                var sinav = await _sinavRepository.GetAsync(s => s.Id == request.SinavId);
                if (sinav == null)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>("Sınav bulunamadı.");
                }

                var duplicate = _kullaniciSinavRepository.Query()
                    .Any(k => k.UserId == userId && k.SinavId == request.SinavId);
                if (duplicate)
                {
                    return new ErrorDataResult<CreateKullaniciSinavDto>(Messages.NameAlreadyExist);
                }

                var entity = new KullaniciSinav
                {
                    UserId = userId,
                    SinavId = request.SinavId,
                    HedefPuan = request.HedefPuan,
                    CreatedBy = userId,
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _kullaniciSinavRepository.Add(entity);
                await _kullaniciSinavRepository.SaveChangesAsync();

                var dto = new CreateKullaniciSinavDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    SinavId = entity.SinavId,
                    HedefPuan = entity.HedefPuan
                };

                return new SuccessDataResult<CreateKullaniciSinavDto>(dto, Messages.Added);
            }
        }
    }
}
