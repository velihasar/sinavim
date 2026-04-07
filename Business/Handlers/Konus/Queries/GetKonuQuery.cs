
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KonuDtos;


namespace Business.Handlers.Konus.Queries
{
    public class GetKonuQuery : IRequest<IDataResult<KonuDto>>
    {
        public int Id { get; set; }

        public class GetKonuQueryHandler : IRequestHandler<GetKonuQuery, IDataResult<KonuDto>>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public GetKonuQueryHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KonuDto>> Handle(GetKonuQuery request, CancellationToken cancellationToken)
            {
                var konu = await _konuRepository.GetAsync(p => p.Id == request.Id);
                
                if(konu == null)
                {
                    return new ErrorDataResult<KonuDto>(null, "Kayıt bulunamadı");
                }

                var dto = new KonuDto
                {
                    Id = konu.Id,
                    Ad = konu.Ad,
                    SiraNo = konu.SiraNo,
                    DersId = konu.DersId
                };

                return new SuccessDataResult<KonuDto>(dto);
            }
        }
    }
}
