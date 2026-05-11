
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciDersNetHedefis.Commands
{
    public class DeleteKullaniciDersNetHedefiCommand : DeleteKullaniciDersNetHedefiDto, IRequest<IResult>
    {
        public class DeleteKullaniciDersNetHedefiCommandHandler
            : IRequestHandler<DeleteKullaniciDersNetHedefiCommand, IResult>
        {
            private readonly IKullaniciDersNetHedefiRepository _kullaniciDersNetHedefiRepository;
            private readonly IMediator _mediator;

            public DeleteKullaniciDersNetHedefiCommandHandler(
                IKullaniciDersNetHedefiRepository kullaniciDersNetHedefiRepository,
                IMediator mediator)
            {
                _kullaniciDersNetHedefiRepository = kullaniciDersNetHedefiRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                DeleteKullaniciDersNetHedefiCommand request,
                CancellationToken cancellationToken)
            {
                var kullaniciDersNetHedefiToDelete =
                    await _kullaniciDersNetHedefiRepository.GetAsync(p => p.Id == request.Id);

                if (kullaniciDersNetHedefiToDelete == null)
                {
                    return new ErrorResult(Messages.RecordNotFound);
                }

                _kullaniciDersNetHedefiRepository.Delete(kullaniciDersNetHedefiToDelete);
                await _kullaniciDersNetHedefiRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
