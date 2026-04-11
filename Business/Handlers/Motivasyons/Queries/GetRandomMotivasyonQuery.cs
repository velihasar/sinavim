using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.MotivasyonDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Motivasyons.Queries
{
    /// <summary>
    /// Aktif motivasyonlar arasından rastgele bir kayıt döner (anasayfa günlük motivasyon).
    /// </summary>
    public class GetRandomMotivasyonQuery : IRequest<IDataResult<MotivasyonDto>>
    {
        public class GetRandomMotivasyonQueryHandler : IRequestHandler<GetRandomMotivasyonQuery, IDataResult<MotivasyonDto>>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public GetRandomMotivasyonQueryHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<MotivasyonDto>> Handle(
                GetRandomMotivasyonQuery request,
                CancellationToken cancellationToken)
            {
                var ids = await _motivasyonRepository
                    .Query()
                    .Where(m => m.IsActive != false)
                    .Select(m => m.Id)
                    .ToListAsync(cancellationToken);

                if (ids.Count == 0)
                {
                    return new ErrorDataResult<MotivasyonDto>("Motivasyon kaydı bulunamadı.");
                }

                var pickId = ids[Random.Shared.Next(ids.Count)];
                var motivasyon = await _motivasyonRepository.GetAsync(m => m.Id == pickId);

                if (motivasyon == null)
                {
                    return new ErrorDataResult<MotivasyonDto>("Motivasyon bulunamadı.");
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
