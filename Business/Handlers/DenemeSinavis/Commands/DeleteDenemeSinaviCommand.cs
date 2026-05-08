
using Business.Constants;
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;


namespace Business.Handlers.DenemeSinavis.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteDenemeSinaviCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteDenemeSinaviCommandHandler : IRequestHandler<DeleteDenemeSinaviCommand, IResult>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;

            public DeleteDenemeSinaviCommandHandler(IDenemeSinaviRepository denemeSinaviRepository)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteDenemeSinaviCommand request, CancellationToken cancellationToken)
            {
                var uid = UserInfoExtensions.GetUserId();
                if (uid == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var denemeSinaviToDelete = await _denemeSinaviRepository.GetAsync(p => p.Id == request.Id);
                if (denemeSinaviToDelete == null)
                {
                    return new ErrorResult("Kayıt bulunamadı.");
                }

                if (denemeSinaviToDelete.UserId != uid)
                {
                    return new ErrorResult("Bu işlem için yetkiniz yok.");
                }

                _denemeSinaviRepository.Delete(denemeSinaviToDelete);
                await _denemeSinaviRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

