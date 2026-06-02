using Core.Entities.Concrete.Project;
using Core.Enums;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Helpers
{
    internal static class ArkadaslikDomainHelper
    {
        public static async Task<bool> IsAlreadyFriendsAsync(
            IArkadaslikRepository repo,
            int userIdA,
            int userIdB,
            CancellationToken cancellationToken)
        {
            var (kucuk, buyuk) = ArkadaslikPairHelper.Order(userIdA, userIdB);
            return await repo.Query().AnyAsync(
                x => x.UserIdKucuk == kucuk
                     && x.UserIdBuyuk == buyuk
                     && x.IsActive != false,
                cancellationToken);
        }

        public static async Task<bool> HasPendingRequestBetweenAsync(
            IArkadaslikIstegiRepository repo,
            int userIdA,
            int userIdB,
            CancellationToken cancellationToken)
        {
            return await repo.Query().AnyAsync(
                x => x.Durum == ArkadaslikIstekDurumu.Beklemede
                     && ((x.GonderenUserId == userIdA && x.HedefUserId == userIdB)
                         || (x.GonderenUserId == userIdB && x.HedefUserId == userIdA)),
                cancellationToken);
        }

        /// <summary>
        /// İki kullanıcının hedef sınavı (<see cref="KullaniciSinav"/>) aynı mı; arkadaşlık öncesi kontrol.
        /// </summary>
        public static async Task<(bool Ok, string Message)> CanBecomeFriendsBySinavAsync(
            IKullaniciSinavRepository repo,
            int userIdA,
            int userIdB,
            CancellationToken cancellationToken)
        {
            var rows = await repo.Query()
                .Where(k => k.UserId == userIdA || k.UserId == userIdB)
                .ToListAsync(cancellationToken);

            var sinavA = rows.FirstOrDefault(k => k.UserId == userIdA);
            var sinavB = rows.FirstOrDefault(k => k.UserId == userIdB);

            if (sinavA == null)
            {
                return (false, "Arkadaş eklemek için önce sınav seçmelisiniz.");
            }

            if (sinavB == null)
            {
                return (false, "Bu kullanıcının henüz sınav seçimi yok.");
            }

            if (sinavA.SinavId != sinavB.SinavId)
            {
                return (false, "Yalnızca aynı sınava hazırlanan öğrenciler arkadaş olabilir.");
            }

            return (true, null);
        }
    }
}
