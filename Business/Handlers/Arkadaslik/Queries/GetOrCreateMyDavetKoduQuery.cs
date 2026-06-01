using Business.BusinessAspects;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Queries
{
    /// <summary>Oturumdaki kullanıcının davet kodunu döner; yoksa oluşturur.</summary>
    public class GetOrCreateMyDavetKoduQuery : IRequest<IDataResult<KullaniciDavetKoduDto>>
    {
        public class GetOrCreateMyDavetKoduQueryHandler
            : IRequestHandler<GetOrCreateMyDavetKoduQuery, IDataResult<KullaniciDavetKoduDto>>
        {
            private const string KodChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            private readonly IKullaniciDavetKoduRepository _davetKoduRepository;

            public GetOrCreateMyDavetKoduQueryHandler(IKullaniciDavetKoduRepository davetKoduRepository)
            {
                _davetKoduRepository = davetKoduRepository;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<KullaniciDavetKoduDto>> Handle(
                GetOrCreateMyDavetKoduQuery request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<KullaniciDavetKoduDto>("Oturum bulunamadı.");
                }

                var existing = await _davetKoduRepository.Query()
                    .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

                if (existing != null)
                {
                    return new SuccessDataResult<KullaniciDavetKoduDto>(Map(existing));
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                for (var attempt = 0; attempt < 10; attempt++)
                {
                    var kod = GenerateKod();
                    var kodVar = await _davetKoduRepository.Query()
                        .AnyAsync(x => x.Kod == kod, cancellationToken);
                    if (kodVar)
                    {
                        continue;
                    }

                    var entity = new Core.Entities.Concrete.Project.KullaniciDavetKodu
                    {
                        UserId = userId,
                        Kod = kod,
                        OlusturulmaTarihi = now,
                    };

                    _davetKoduRepository.Add(entity);
                    await _davetKoduRepository.SaveChangesAsync();
                    return new SuccessDataResult<KullaniciDavetKoduDto>(Map(entity));
                }

                return new ErrorDataResult<KullaniciDavetKoduDto>(
                    "Davet kodu oluşturulamadı. Lütfen tekrar deneyin.");
            }

            private static string GenerateKod()
            {
                var bytes = new byte[8];
                RandomNumberGenerator.Fill(bytes);
                var chars = new char[8];
                for (var i = 0; i < 8; i++)
                {
                    chars[i] = KodChars[bytes[i] % KodChars.Length];
                }

                return new string(chars);
            }

            private static KullaniciDavetKoduDto Map(Core.Entities.Concrete.Project.KullaniciDavetKodu e)
            {
                return new KullaniciDavetKoduDto
                {
                    Kod = e.Kod,
                    OlusturulmaTarihi = e.OlusturulmaTarihi,
                };
            }
        }
    }
}
