
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.MotivasyonDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Motivasyons.Queries
{
    public class GetMotivasyonQuery : IRequest<IDataResult<MotivasyonDto>>
    {
        public int Id { get; set; }

        public class GetMotivasyonQueryHandler : IRequestHandler<GetMotivasyonQuery, IDataResult<MotivasyonDto>>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public GetMotivasyonQueryHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<MotivasyonDto>> Handle(GetMotivasyonQuery request, CancellationToken cancellationToken)
            {
                var motivasyon = await _motivasyonRepository.GetAsync(p => p.Id == request.Id);

                if (motivasyon == null)
                {
                    return new ErrorDataResult<MotivasyonDto>(null, "Kayıt bulunamadı");
                }

                var dto = new MotivasyonDto
                {
                    Id = motivasyon.Id,
                    Kelime = motivasyon.Kelime,
                };

                return new SuccessDataResult<MotivasyonDto>(dto);
            }
        }
    }
}
