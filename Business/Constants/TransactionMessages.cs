namespace Business.Constants
{
    /// <summary>
    /// This class was created to get rid of magic strings and write more readable code.
    /// </summary>
    public static partial class Messages
    {
        public static string OperationClaimExists => "OperationClaimExists";
        public static string AuthorizationsDenied => "AuthorizationsDenied";
        public static string Added => "Added";
        public static string Deleted => "Deleted";
        public static string Updated => "Updated";
        public static string NameAlreadyExist => "NameAlreadyExist";

        /// <summary>Kayıt / kullanıcı oluşturma: aynı e-posta zaten var.</summary>
        public static string EmailAlreadyRegistered => "Bu e-posta adresi zaten kullanılıyor.";
        public static string TokenProviderException => "TokenProviderException";
        public static string Unknown => "Unknown";
        public static string RecordNotFound => "RecordNotFound";
    }
}