namespace Core.Enums
{
    /// <summary>Arkadaşlık isteği yaşam döngüsü.</summary>
    public enum ArkadaslikIstekDurumu
    {
        Beklemede = 0,
        Kabul = 1,
        Red = 2,
        /// <summary>Gönderen veya alan tarafından iptal.</summary>
        Iptal = 3,
    }
}
