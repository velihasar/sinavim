
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciDersNetHedefis.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Commands
{
    public class CreateKullaniciDersNetHedefiCommand : CreateKullaniciDersNetHedefiDto,
        IRequest<IDataResult<KullaniciDersNetHedefiDto>>
    {
        public class CreateKullaniciDersNetHedefiCommandHandler
            : IRequestHandler<CreateKullaniciDersNetHedefiCommand, IDataResult<KullaniciDersNetHedefiDto>>
        {
            private readonly IKullaniciDersNetHedefiRepository _kullaniciDersNetHedefiRepository;
            private readonly IMediator _mediator;

            public CreateKullaniciDersNetHedefiCommandHandler(
                IKullaniciDersNetHedefiRepository kullaniciDersNetHedefiRepository,
                IMediator mediator)
            {
                _kullaniciDersNetHedefiRepository = kullaniciDersNetHedefiRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKullaniciDersNetHedefiValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciDersNetHedefiDto>> Handle(
                CreateKullaniciDersNetHedefiCommand request,
                CancellationToken cancellationToken)
            {
                var duplicateExists = _kullaniciDersNetHedefiRepository.Query().Any(u =>
                    u.UserId == request.UserId && u.DersId == request.DersId);

                if (duplicateExists)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(Messages.NameAlreadyExist);
                }

                var addedKullaniciDersNetHedefi = new KullaniciDersNetHedefi
                {
                    UserId = request.UserId,
                    DersId = request.DersId,
                    SinavBolumId = request.SinavBolumId,
                    HedefNet = request.HedefNet,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = DateTimeExtensions.NowForNpgsqlTimestamp(),
                    IsActive = true,
                };

                _kullaniciDersNetHedefiRepository.Add(addedKullaniciDersNetHedefi);
                await _kullaniciDersNetHedefiRepository.SaveChangesAsync();

                return new SuccessDataResult<KullaniciDersNetHedefiDto>(
                    KullaniciDersNetHedefiDto.FromEntity(addedKullaniciDersNetHedefi),
                    Messages.Added);
            }
        }
    }
}
