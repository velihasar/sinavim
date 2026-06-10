using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.KullaniciGunlukSoruCozumus.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Business.BackgroundJobs
{
    /// <summary>
    /// 90 günlük pencere dışında kalan günlük soru çözüm kayıtlarını siler (Türkiye takvimi).
    /// </summary>
    public class GunlukSoruCozumuRetentionJob
    {
        public const string RecurringJobId = "purge-gunluk-soru-cozumu-90d";

        private readonly ProjectDbContext _db;

        public GunlukSoruCozumuRetentionJob(ProjectDbContext db)
        {
            _db = db;
        }

        public async Task<int> PurgeExpiredRecordsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTimeExtensions.TurkeyTodayToNpgsqlDateOnly();
            var oldestAllowed = today.AddDays(-(GetGunlukSoruCozumuPageForMeQuery.MaxGunlukPenceresi - 1));

            return await _db.Set<KullaniciGunlukSoruCozumu>()
                .Where(x => x.Tarih < oldestAllowed)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
