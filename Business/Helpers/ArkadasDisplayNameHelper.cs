using System;
using System.Linq;

namespace Business.Helpers
{
    /// <summary>
    /// Giden (bekleyen) arkadaşlık isteklerinde hedef adını maskelemek için.
    /// Kabul sonrası tam ad arkadaş listesinde gösterilir.
    /// </summary>
    public static class ArkadasDisplayNameHelper
    {
        public static string MaskToInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "Kullanıcı";
            }

            var parts = fullName.Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                return "Kullanıcı";
            }

            return string.Join(
                " ",
                parts.Select(p =>
                {
                    var ch = p.Trim()[0];
                    return char.ToUpperInvariant(ch) + ".";
                }));
        }
    }
}
