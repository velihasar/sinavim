using Business.BusinessAspects;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinaviDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.DenemeSinavis.Queries
{
    /// <summary>
    /// Oturumdaki kullanıcının tek bir denemesi (kart/detay ile aynı şekilde sonuçlar dahil).
    /// </summary>
    public class GetMyDenemeSinaviByIdQuery : IRequest<IDataResult<DenemeSinaviOzetListDto>>
    {
        public int Id { get; set; }

        public class GetMyDenemeSinaviByIdQueryHandler
            : IRequestHandler<GetMyDenemeSinaviByIdQuery, IDataResult<DenemeSinaviOzetListDto>>
        {
            private readonly IDenemeSinaviRepository _denemeSinaviRepository;
            private readonly IDersRepository _dersRepository;

            public GetMyDenemeSinaviByIdQueryHandler(
                IDenemeSinaviRepository denemeSinaviRepository,
                IDersRepository dersRepository)
            {
                _denemeSinaviRepository = denemeSinaviRepository;
                _dersRepository = dersRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<DenemeSinaviOzetListDto>> Handle(
                GetMyDenemeSinaviByIdQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<DenemeSinaviOzetListDto>("Oturum bulunamadı.");
                }

                if (request.Id <= 0)
                {
                    return new ErrorDataResult<DenemeSinaviOzetListDto>("Geçersiz deneme.");
                }

                var d = await _denemeSinaviRepository.Query()
                    .AsNoTracking()
                    .Include(x => x.SinavBolum)
                    .Include(x => x.Sonuclar)
                    .Where(x => x.Id == request.Id && x.UserId == userId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (d == null)
                {
                    return new ErrorDataResult<DenemeSinaviOzetListDto>("Kayıt bulunamadı.");
                }

                var dersTemplateCache = new Dictionary<(int, int?), List<Ders>>();
                var key = (d.SinavId, d.SinavBolumId);
                var q = _dersRepository.Query()
                    .AsNoTracking()
                    .Where(x => x.SinavId == key.Item1);
                if (key.Item2.HasValue && key.Item2.Value > 0)
                {
                    var bid = key.Item2.Value;
                    q = q.Where(x => x.SinavBolumId == bid);
                }

                var template = await q
                    .OrderBy(x => x.SiraNo)
                    .ThenBy(x => x.Id)
                    .ToListAsync(cancellationToken);
                dersTemplateCache[key] = template;

                var dto = new DenemeSinaviOzetListDto
                {
                    Id = d.Id,
                    Ad = d.Ad,
                    SinavId = d.SinavId,
                    SinavBolumId = d.SinavBolumId,
                    SinavBolumIsim = d.SinavBolum != null ? d.SinavBolum.Isim : null,
                    Tarih = d.Tarih,
                    ToplamNet = d.Sonuclar != null && d.Sonuclar.Count > 0
                        ? d.Sonuclar.Sum(s => s.ToplamNet)
                        : 0m,
                    Sonuclar = DenemeSinaviSonucMapper.BuildSonuclarFullDers(d, dersTemplateCache),
                };

                return new SuccessDataResult<DenemeSinaviOzetListDto>(dto);
            }
        }
    }
}
