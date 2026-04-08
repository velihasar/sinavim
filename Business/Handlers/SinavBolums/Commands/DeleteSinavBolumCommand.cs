
using Business.BusinessAspects;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.SinavBolums.Commands
{
    public class DeleteSinavBolumCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteSinavBolumCommandHandler : IRequestHandler<DeleteSinavBolumCommand, IResult>
        {
            private readonly ISinavBolumRepository _sinavBolumRepository;
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public DeleteSinavBolumCommandHandler(ISinavBolumRepository sinavBolumRepository, IDersRepository dersRepository, IMediator mediator)
            {
                _sinavBolumRepository = sinavBolumRepository;
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteSinavBolumCommand request, CancellationToken cancellationToken)
            {
                var entity = await _sinavBolumRepository.GetAsync(b => b.Id == request.Id);
                if (entity == null)
                    return new ErrorResult("Kayıt bulunamadı");

                var hasDers = _dersRepository.Query().Any(d => d.SinavBolumId == request.Id);
                if (hasDers)
                    return new ErrorResult("Bu bölüme bağlı ders kayıtları var; önce dersleri güncelleyin.");

                _sinavBolumRepository.Delete(entity);
                await _sinavBolumRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
