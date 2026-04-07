
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
using Core.Entities.Dtos.Project.DersDtos;


namespace Business.Handlers.Derses.Queries
{
    public class GetDersQuery : IRequest<IDataResult<DersDto>>
    {
        public int Id { get; set; }

        public class GetDersQueryHandler : IRequestHandler<GetDersQuery, IDataResult<DersDto>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public GetDersQueryHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DersDto>> Handle(GetDersQuery request, CancellationToken cancellationToken)
            {
                var ders = await _dersRepository.GetAsync(p => p.Id == request.Id);
                
                if(ders == null)
                {
                    return new ErrorDataResult<DersDto>(null, "Kayıt bulunamadı");
                }

                var dto = new DersDto
                {
                    Id = ders.Id,
                    Ad = ders.Ad,
                    SinavId = ders.SinavId
                };

                return new SuccessDataResult<DersDto>(dto);
            }
        }
    }
}
