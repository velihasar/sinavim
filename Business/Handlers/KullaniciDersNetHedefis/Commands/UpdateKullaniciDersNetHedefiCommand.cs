
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciDersNetHedefis.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
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
    public class UpdateKullaniciDersNetHedefiCommand : UpdateKullaniciDersNetHedefiDto,
        IRequest<IDataResult<KullaniciDersNetHedefiDto>>
    {
        public class UpdateKullaniciDersNetHedefiCommandHandler
            : IRequestHandler<UpdateKullaniciDersNetHedefiCommand, IDataResult<KullaniciDersNetHedefiDto>>
        {
            private readonly IKullaniciDersNetHedefiRepository _kullaniciDersNetHedefiRepository;
            private readonly IMediator _mediator;

            public UpdateKullaniciDersNetHedefiCommandHandler(
                IKullaniciDersNetHedefiRepository kullaniciDersNetHedefiRepository,
                IMediator mediator)
            {
                _kullaniciDersNetHedefiRepository = kullaniciDersNetHedefiRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKullaniciDersNetHedefiValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciDersNetHedefiDto>> Handle(
                UpdateKullaniciDersNetHedefiCommand request,
                CancellationToken cancellationToken)
            {
                var entity = await _kullaniciDersNetHedefiRepository.GetAsync(u => u.Id == request.Id);

                if (entity == null)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(Messages.RecordNotFound);
                }

                var duplicateExists = _kullaniciDersNetHedefiRepository.Query().Any(u =>
                    u.UserId == request.UserId && u.DersId == request.DersId && u.Id != request.Id);

                if (duplicateExists)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(Messages.NameAlreadyExist);
                }

                entity.UserId = request.UserId;
                entity.DersId = request.DersId;
                entity.SinavBolumId = request.SinavBolumId;
                entity.HedefNet = request.HedefNet;
                entity.UpdatedBy = UserInfoExtensions.GetUserId();
                entity.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();

                _kullaniciDersNetHedefiRepository.Update(entity);
                await _kullaniciDersNetHedefiRepository.SaveChangesAsync();

                return new SuccessDataResult<KullaniciDersNetHedefiDto>(
                    KullaniciDersNetHedefiDto.FromEntity(entity),
                    Messages.Updated);
            }
        }
    }
}
