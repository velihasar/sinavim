using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.KullaniciGunlukSoruCozumus.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.KullaniciGunlukSoruCozumus.Commands
{
    public class UpsertMyKullaniciGunlukSoruCozumuCommand : UpsertMyKullaniciGunlukSoruCozumuDto, IRequest<IResult>
    {
        public class UpsertMyKullaniciGunlukSoruCozumuCommandHandler
            : IRequestHandler<UpsertMyKullaniciGunlukSoruCozumuCommand, IResult>
        {
            private readonly IKullaniciGunlukSoruCozumuRepository _repo;
            private readonly IMediator _mediator;

            public UpsertMyKullaniciGunlukSoruCozumuCommandHandler(
                IKullaniciGunlukSoruCozumuRepository repo,
                IMediator mediator)
            {
                _repo = repo;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpsertMyKullaniciGunlukSoruCozumuValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(
                UpsertMyKullaniciGunlukSoruCozumuCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorResult("Oturum bulunamadı.");
                }

                var day = request.Tarih.ToNpgsqlDateOnly();
                var dayEnd = day.AddDays(1);

                var existing = await _repo.Query()
                    .Where(x => x.UserId == userId && x.Tarih >= day && x.Tarih < dayEnd)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existing != null)
                {
                    existing.CozulenSoruSayisi = request.CozulenSoruSayisi;
                    _repo.Update(existing);
                }
                else
                {
                    _repo.Add(new KullaniciGunlukSoruCozumu
                    {
                        UserId = userId,
                        Tarih = day,
                        CozulenSoruSayisi = request.CozulenSoruSayisi,
                    });
                }

                await _repo.SaveChangesAsync();
                return new SuccessResult(existing != null ? Messages.Updated : Messages.Added);
            }
        }
    }
}
