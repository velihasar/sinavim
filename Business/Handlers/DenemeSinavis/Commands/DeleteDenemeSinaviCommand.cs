
using Business.Constants;
using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            private readonly IDenemeSinavSonucuRepository _denemeSinavSonucuRepository;

            public DeleteDenemeSinaviCommandHandler(
                IDenemeSinaviRepository denemeSinaviRepository,
                IDenemeSinavSonucuRepository denemeSinavSonucuRepository)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _denemeSinavSonucuRepository = denemeSinavSonucuRepository;
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

                var sonuclar = await _denemeSinavSonucuRepository.Query()
                    .Where(s => s.DenemeSinaviId == request.Id)
                    .ToListAsync(cancellationToken);
                foreach (var s in sonuclar)
                {
                    _denemeSinavSonucuRepository.Delete(s);
                }

                _denemeSinaviRepository.Delete(denemeSinaviToDelete);
                await _denemeSinaviRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

