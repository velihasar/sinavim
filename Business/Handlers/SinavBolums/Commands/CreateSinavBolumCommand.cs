
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.SinavBolums.ValidationRules;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.SinavBolumDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.SinavBolums.Commands
{
    public class CreateSinavBolumCommand : IRequest<IDataResult<CreateSinavBolumDto>>
    {
        public int SinavId { get; set; }
        public string Isim { get; set; }

        public class CreateSinavBolumCommandHandler : IRequestHandler<CreateSinavBolumCommand, IDataResult<CreateSinavBolumDto>>
        {
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly ISinavRepository _sinavRepository;
            private readonly IMediator _mediator;

            public CreateSinavBolumCommandHandler(ISinavBolumRepository sinavBolumRepository, ISinavRepository sinavRepository, IMediator mediator)
            {
                _sinavBolumRepository = sinavBolumRepository;
                _sinavRepository = sinavRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateSinavBolumValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateSinavBolumDto>> Handle(CreateSinavBolumCommand request, CancellationToken cancellationToken)
            {
                var sinav = await _sinavRepository.GetAsync(s => s.Id == request.SinavId);
                if (sinav == null)
                    return new ErrorDataResult<CreateSinavBolumDto>("Sınav bulunamadı.");

                var duplicate = _sinavBolumRepository.Query().Any(b =>
                    b.SinavId == request.SinavId
                    && b.Isim == request.Isim);
                if (duplicate)
                    return new ErrorDataResult<CreateSinavBolumDto>(Messages.NameAlreadyExist);

                var entity = new SinavBolum
                {
                    SinavId = request.SinavId,
                    Isim = request.Isim
                };

                _sinavBolumRepository.Add(entity);
                await _sinavBolumRepository.SaveChangesAsync();

                var dto = new CreateSinavBolumDto
                {
                    Id = entity.Id,
                    SinavId = (int)entity.SinavId,
                    Isim = entity.Isim
                };

                return new SuccessDataResult<CreateSinavBolumDto>(dto, Messages.Added);
            }
        }
    }
}
