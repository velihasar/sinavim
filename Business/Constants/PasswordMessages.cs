namespace Business.Constants
{
    public static partial class Messages
    {
        public static string PasswordEmpty => "Şifre boş olamaz.";
        public static string PasswordLength => "Şifre en az 8 karakter olmalıdır.";
        public static string PasswordUppercaseLetter => "Şifre en az bir büyük harf (A–Z) içermelidir.";
        public static string PasswordLowercaseLetter => "Şifre en az bir küçük harf (a–z) içermelidir.";
        public static string PasswordDigit => "Şifre en az bir rakam (0–9) içermelidir.";
        public static string PasswordSpecialCharacter => "Şifre en az bir özel karakter içermelidir (ör. ! @ # ? *).";
        public static string SendPassword => "SendPassword";
        public static string NewPassword => "NewPassword";

        /// <summary>Şifre sıfırlama ve e-posta doğrulama kodu talebinde ortak (adres sızıntısını azaltır).</summary>
        public static string PasswordResetRequestSent =>
            "Bu e-posta adresi kayıtlıysa, doğrulama kodu gönderildi. Gelen kutunuzu ve spam klasörünü kontrol edin.";

        public static string PasswordResetInvalidOrExpired =>
            "Kod hatalı veya süresi dolmuş. Yeni kod talep edebilirsiniz.";

        public static string PasswordResetCompleted => "Şifreniz güncellendi. Giriş yapabilirsiniz.";

        public static string PasswordResetMailFailed =>
            "E-posta gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin veya yöneticiye bildirin.";

        public static string EmailVerificationMailFailed =>
            "Doğrulama e-postası gönderilemedi. Lütfen daha sonra tekrar deneyin veya yöneticiye bildirin.";

        public static string RegistrationPendingVerification =>
            "Kaydınız alındı. E-posta adresinize gönderilen doğrulama kodunu girin.";

        public static string EmailVerificationInvalidOrExpired =>
            "Doğrulama kodu hatalı veya süresi dolmuş. Yeni kod talep edebilirsiniz.";

        public static string EmailVerifiedSuccess =>
            "E-posta adresiniz doğrulandı. Giriş yapabilirsiniz.";

        public static string EmailAlreadyConfirmed =>
            "Bu e-posta adresi zaten doğrulanmış.";

        public static string CurrentPasswordIncorrect => "Mevcut şifre hatalı.";

        public static string EmailChangeRequiresVerification =>
            "E-posta değişikliği doğrulama kodu ile yapılır; doğrudan güncellenemez.";

        public static string EmailChangeCodeSent =>
            "Yeni e-posta adresinize doğrulama kodu gönderildi.";

        public static string EmailChangeSuccess => "E-posta adresiniz güncellendi.";

        public static string EmailChangeSameAsCurrent =>
            "Yeni adres mevcut e-posta adresinizle aynı.";

        public static string EmailChangeInvalidOrExpired =>
            "E-posta doğrulama kodu hatalı veya süresi dolmuş. Yeni kod talep edebilirsiniz.";
    }
}