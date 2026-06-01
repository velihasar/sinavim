namespace Business.Helpers
{
    /// <summary>Arkadaşlık tablosunda çift kayıt önlemek için kullanıcı id sıralaması.</summary>
    public static class ArkadaslikPairHelper
    {
        public static (int UserIdKucuk, int UserIdBuyuk) Order(int userIdA, int userIdB)
        {
            return userIdA < userIdB ? (userIdA, userIdB) : (userIdB, userIdA);
        }
    }
}
