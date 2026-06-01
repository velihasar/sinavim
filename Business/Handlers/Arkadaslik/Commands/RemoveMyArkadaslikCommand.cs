using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Commands
{
    /// <summary>Oturumdaki kullanıcının bir arkadaşlık kaydını kaldırır.</summary>
    public class RemoveMyArkadaslikCommand : IRequest<IResult>
    {
        public int ArkadaslikId { get; set; }

        public class RemoveMyArkadaslikCommandHandler
            : IRequestHandler<RemoveMyArkadaslikCommand, IResult>
        {
            private readonly IArkadaslikRepository _arkadaslikRepository;

            public RemoveMyArkadaslikCommandHandler(IArkadaslikRepository arkadaslikRepository)
            {
                _arkadaslikRepository = arkadaslikRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                RemoveMyArkadaslikCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var arkadaslik = await _arkadaslikRepository.Query()
                    .FirstOrDefaultAsync(x => x.Id == request.ArkadaslikId, cancellationToken);

                if (arkadaslik == null)
                {
                    return new ErrorResult("Arkadaşlık bulunamadı.");
                }

                if (arkadaslik.UserIdKucuk != userId && arkadaslik.UserIdBuyuk != userId)
                {
                    return new ErrorResult("Bu arkadaşlığı kaldırma yetkiniz yok.");
                }

                _arkadaslikRepository.Delete(arkadaslik);
                await _arkadaslikRepository.SaveChangesAsync();

                return new SuccessResult("Arkadaşlık kaldırıldı.");
            }
        }
    }
}
