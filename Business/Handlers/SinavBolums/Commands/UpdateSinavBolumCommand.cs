
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.SinavBolums.ValidationRules;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Dtos.Project.SinavBolumDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.SinavBolums.Commands
{
    public class UpdateSinavBolumCommand : IRequest<IDataResult<UpdateSinavBolumDto>>
    {
        public int Id { get; set; }
        public int SinavId { get; set; }
        public string Isim { get; set; }

        public class UpdateSinavBolumCommandHandler : IRequestHandler<UpdateSinavBolumCommand, IDataResult<UpdateSinavBolumDto>>
        {
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public UpdateSinavBolumCommandHandler(ISinavBolumRepository sinavBolumRepository, ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavBolumRepository = sinavBolumRepository;
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateSinavBolumValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateSinavBolumDto>> Handle(UpdateSinavBolumCommand request, CancellationToken cancellationToken)
            {
                var entity = await _sinavBolumRepository.GetAsync(b => b.Id == request.Id);
                if (entity == null)
                    return new ErrorDataResult<UpdateSinavBolumDto>("Kayıt bulunamadı");

                var sinav = await _sinavRepository.GetAsync(s => s.Id == request.SinavId);
                if (sinav == null)
                    return new ErrorDataResult<UpdateSinavBolumDto>("Sınav bulunamadı.");

                var duplicate = _sinavBolumRepository.Query().Any(b =>
                    b.Id != request.Id
                    && b.SinavId == request.SinavId
                    && b.Isim == request.Isim);
                if (duplicate)
                    return new ErrorDataResult<UpdateSinavBolumDto>(Messages.NameAlreadyExist);

                entity.SinavId = request.SinavId;
                entity.Isim = request.Isim;
                entity.UpdatedBy = UserInfoExtensions.GetUserId();
                entity.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();

                _sinavBolumRepository.Update(entity);
                await _sinavBolumRepository.SaveChangesAsync();

                var dto = new UpdateSinavBolumDto
                {
                    Id = entity.Id,
                    SinavId = entity.SinavId,
                    Isim = entity.Isim
                };

                return new SuccessDataResult<UpdateSinavBolumDto>(dto, Messages.Updated);
            }
        }
    }
}
