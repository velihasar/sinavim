using System;
using System.Globalization;

namespace Core.Extensions
{
    /// <summary>Takvim günü hesapları (günlük soru vb.): TR kullanıcıları için yerel gece yarısı UTC’den farklı olabilir.</summary>
    public static class DateTimeExtensions
    {
        private static readonly string[] TurkeyTimeZoneIds = new[] { "Turkey Standard Time", "Europe/Istanbul" };

        /// <summary>Windows / Linux’ta Türkiye saati dilimi.</summary>
        public static TimeZoneInfo GetTurkeyTimeZone()
        {
            foreach (var id in TurkeyTimeZoneIds)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                    /* try next */
                }
                catch (InvalidTimeZoneException)
                {
                    /* try next */
                }
            }

            return TimeZoneInfo.Utc;
        }

        /// <summary>Şu anki anın Türkiye takvim günü (00:00, <see cref="DateTimeKind.Unspecified"/>).</summary>
        public static DateTime TurkeyTodayToNpgsqlDateOnly()
        {
            var tz = GetTurkeyTimeZone();
            var nowTr = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            return new DateTime(nowTr.Year, nowTr.Month, nowTr.Day, 0, 0, 0, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// PostgreSQL <c>timestamp without time zone</c> kolonuna yazmak için: UTC anı, <see cref="DateTimeKind.Unspecified"/> ile (Npgsql kuralı).
        /// </summary>
        public static DateTime UtcNowPlusMinutesForNpgsqlTimestamp(int minutes)
        {
            return DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(minutes), DateTimeKind.Unspecified);
        }

        /// <summary><c>timestamp without time zone</c> kolonundan gelen süre ile aynı Kind ile şu anı karşılaştırmak için.</summary>
        public static DateTime UtcNowForNpgsqlTimestampCompare()
        {
            return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// PostgreSQL <c>timestamp without time zone</c> ile Npgsql: <see cref="DateTimeKind.Utc"/> / <see cref="DateTimeKind.Local"/>
        /// parametre veya kolon yazımında hata verir; günlük tarih için <see cref="DateTimeKind.Unspecified"/> üretir.
        /// </summary>
        public static DateTime ToNpgsqlDateOnly(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Unspecified);
        }

        public static string ToPrettyDate(this DateTime date, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            return date.ToString("yyyyMMdd", culture);
        }
    }
}