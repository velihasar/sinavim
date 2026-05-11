
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Queries
{
    public class GetKullaniciDersNetHedefiQuery : IRequest<IDataResult<KullaniciDersNetHedefiDto>>
    {
        public int Id { get; set; }

        public class GetKullaniciDersNetHedefiQueryHandler
            : IRequestHandler<GetKullaniciDersNetHedefiQuery, IDataResult<KullaniciDersNetHedefiDto>>
        {
            private readonly IKullaniciDersNetHedefiRepository _kullaniciDersNetHedefiRepository;
            private readonly IMediator _mediator;

            public GetKullaniciDersNetHedefiQueryHandler(
                IKullaniciDersNetHedefiRepository kullaniciDersNetHedefiRepository,
                IMediator mediator)
            {
                _kullaniciDersNetHedefiRepository = kullaniciDersNetHedefiRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciDersNetHedefiDto>> Handle(
                GetKullaniciDersNetHedefiQuery request,
                CancellationToken cancellationToken)
            {
                var entity = await _kullaniciDersNetHedefiRepository.GetAsync(p => p.Id == request.Id);
                if (entity == null)
                {
                    return new ErrorDataResult<KullaniciDersNetHedefiDto>(Messages.RecordNotFound);
                }

                return new SuccessDataResult<KullaniciDersNetHedefiDto>(KullaniciDersNetHedefiDto.FromEntity(entity));
            }
        }
    }
}
