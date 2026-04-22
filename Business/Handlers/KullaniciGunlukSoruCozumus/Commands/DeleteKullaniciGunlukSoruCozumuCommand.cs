using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{
    public class DeleteKullaniciGunlukSoruCozumuCommand : DeleteKullaniciGunlukSoruCozumuDto, IRequest<IResult>
    {
        public class DeleteKullaniciGunlukSoruCozumuCommandHandler : IRequestHandler<DeleteKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _kullaniciGunlukSoruCozumuRepository;
            private readonly IMediator _mediator;

            public DeleteKullaniciGunlukSoruCozumuCommandHandler(
                IKullaniciGunlukSoruCozumuRepository kullaniciGunlukSoruCozumuRepository,
                IMediator mediator)
            {
                _kullaniciGunlukSoruCozumuRepository = kullaniciGunlukSoruCozumuRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteKullaniciGunlukSoruCozumuCommand request, CancellationToken cancellationToken)
            {
                var kullaniciGunlukSoruCozumuToDelete = _kullaniciGunlukSoruCozumuRepository.Get(p => p.Id == request.Id);
                if (kullaniciGunlukSoruCozumuToDelete == null)
                {
                    return new ErrorResult(Messages.RecordNotFound);
                }

                _kullaniciGunlukSoruCozumuRepository.Delete(kullaniciGunlukSoruCozumuToDelete);
                await _kullaniciGunlukSoruCozumuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
