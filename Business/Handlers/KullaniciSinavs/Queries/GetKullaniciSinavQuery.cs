
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


namespace Business.Handlers.KullaniciSinavs.Queries
{
    public class GetKullaniciSinavQuery : IRequest<IDataResult<KullaniciSinav>>
    {
        public int Id { get; set; }

        public class GetKullaniciSinavQueryHandler : IRequestHandler<GetKullaniciSinavQuery, IDataResult<KullaniciSinav>>
        {
            private readonly IKullaniciSinavRepository _kullaniciSinavRepository;
            private readonly IMediator _mediator;

            public GetKullaniciSinavQueryHandler(IKullaniciSinavRepository kullaniciSinavRepository, IMediator mediator)
            {
                _kullaniciSinavRepository = kullaniciSinavRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciSinav>> Handle(GetKullaniciSinavQuery request, CancellationToken cancellationToken)
            {
                var kullaniciSinav = await _kullaniciSinavRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<KullaniciSinav>(kullaniciSinav);
            }
        }
    }
}
