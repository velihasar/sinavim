using Business.BusinessAspects;
using Business.Handlers.ArkadaslikApp.ValidationRules;
using Business.Helpers;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Core.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.ArkadaslikApp.Commands
{
    /// <summary>Davet kodu ile arkadaşlık isteği gönderir (gönderen oturumdaki kullanıcıdır).</summary>
    public class SendMyArkadaslikIstegiByDavetKoduCommand
        : IRequest<IDataResult<ArkadaslikIstegiListItemDto>>
    {
        public string DavetKodu { get; set; }

        public class SendMyArkadaslikIstegiByDavetKoduCommandHandler
            : IRequestHandler<SendMyArkadaslikIstegiByDavetKoduCommand,
                IDataResult<ArkadaslikIstegiListItemDto>>
        {
            private readonly IArkadaslikIstegiRepository _istekRepository;
            private readonly IKullaniciDavetKoduRepository _davetKoduRepository;
            private readonly IArkadaslikRepository _arkadaslikRepository;
            private readonly IUserRepository _userRepository;

            public SendMyArkadaslikIstegiByDavetKoduCommandHandler(
                IArkadaslikIstegiRepository istekRepository,
                IKullaniciDavetKoduRepository davetKoduRepository,
                IArkadaslikRepository arkadaslikRepository,
                IUserRepository userRepository)
            {
                _istekRepository = istekRepository;
                _davetKoduRepository = davetKoduRepository;
                _arkadaslikRepository = arkadaslikRepository;
                _userRepository = userRepository;
            }

            [ValidationAspect(typeof(SendMyArkadaslikIstegiByDavetKoduValidator), Priority = 1)]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ArkadaslikIstegiListItemDto>> Handle(
                SendMyArkadaslikIstegiByDavetKoduCommand request,
                CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserId();
                if (userId == 0)
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>("Oturum bulunamadı.");
                }

                var kod = request.DavetKodu?.Trim().ToUpperInvariant();
                if (string.IsNullOrEmpty(kod))
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>("Davet kodu gerekli.");
                }

                var davet = await _davetKoduRepository.Query()
                    .FirstOrDefaultAsync(x => x.Kod == kod, cancellationToken);

                if (davet == null)
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>("Davet kodu geçersiz.");
                }

                var hedefUserId = davet.UserId;
                if (hedefUserId == userId)
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>(
                        "Kendi davet kodunuzla istek gönderemezsiniz.");
                }

                if (await ArkadaslikDomainHelper.IsAlreadyFriendsAsync(
                        _arkadaslikRepository, userId, hedefUserId, cancellationToken))
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>(
                        "Bu kullanıcı zaten arkadaşınız.");
                }

                if (await ArkadaslikDomainHelper.HasPendingRequestBetweenAsync(
                        _istekRepository, userId, hedefUserId, cancellationToken))
                {
                    return new ErrorDataResult<ArkadaslikIstegiListItemDto>(
                        "Bu kullanıcıyla zaten bekleyen bir istek var.");
                }

                var now = DateTimeExtensions.NowForNpgsqlTimestamp();
                var entity = new ArkadaslikIstegi
                {
                    GonderenUserId = userId,
                    HedefUserId = hedefUserId,
                    Durum = ArkadaslikIstekDurumu.Beklemede,
                    KullanilanDavetKodu = kod,
                    OlusturulmaTarihi = now,
                    CreatedBy = userId,
                    CreatedDate = now,
                    IsActive = true,
                };

                _istekRepository.Add(entity);
                await _istekRepository.SaveChangesAsync();

                var gonderen = await _userRepository.GetAsync(u => u.UserId == userId);
                var hedef = await _userRepository.GetAsync(u => u.UserId == hedefUserId);

                var dto = new ArkadaslikIstegiListItemDto
                {
                    Id = entity.Id,
                    GonderenUserId = entity.GonderenUserId,
                    GonderenFullName = gonderen?.FullName,
                    HedefUserId = entity.HedefUserId,
                    HedefFullName = ArkadasDisplayNameHelper.MaskToInitials(hedef?.FullName),
                    Durum = entity.Durum,
                    OlusturulmaTarihi = entity.OlusturulmaTarihi,
                    YanitTarihi = entity.YanitTarihi,
                };

                return new SuccessDataResult<ArkadaslikIstegiListItemDto>(
                    dto,
                    "Arkadaşlık isteği gönderildi.");
            }
        }
    }
}
