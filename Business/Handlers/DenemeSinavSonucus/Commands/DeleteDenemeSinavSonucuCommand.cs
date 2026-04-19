
using Business.Constants;
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;


namespace Business.Handlers.DenemeSinavSonucus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteDenemeSinavSonucuCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteDenemeSinavSonucuCommandHandler : IRequestHandler<DeleteDenemeSinavSonucuCommand, IResult>
        {
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;

            public DeleteDenemeSinavSonucuCommandHandler(
                IDenemeSinavSonucuRepository denemeSinavSonucuRepository,
                IDenemeSinaviRepository denemeSinaviRepository)
            {
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
                _denemeSinaviRepository = denemeSinaviRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDenemeSinavSonucuCommand request, CancellationToken cancellationToken)
            {
                var denemeSinavSonucuToDelete = await _denemeSinavSonucuRepository.GetAsync(p => p.Id == request.Id);
                if (denemeSinavSonucuToDelete == null)
                {
                    return new ErrorResult("Kayıt bulunamadı.");
                }

                var deneme = await _denemeSinaviRepository.GetAsync(d => d.Id == denemeSinavSonucuToDelete.DenemeSinaviId);
                var uid = UserInfoExtensions.GetUserId();
                if (uid == 0 || deneme == null || deneme.UserId != uid)
                {
                    return new ErrorResult("Bu işlem için yetkiniz yok.");
                }

                _denemeSinavSonucuRepository.Delete(denemeSinavSonucuToDelete);
                await _denemeSinavSonucuRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

