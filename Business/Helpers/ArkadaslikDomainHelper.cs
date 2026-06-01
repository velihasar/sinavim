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
    }
}
