using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Helpers
{
    public static class VerificationCodeMailHelper
    {
        private const string AccentColor = "#0a7ea4";
        private const string BrandProduct = "Sınavım";
        private const string BrandCompany = "MasavTech";

        public enum MailPurpose
        {
            PasswordReset,
            EmailAddressVerification,
            EmailAddressChange,
        }

        /// <summary>
        /// SMTP yoksa Development’ta kodu loglar ve true döner; yapılandırılmış SMTP’da gönderim hatasında false.
        /// </summary>
        /// <param name="sendToEmailOverride">Doluysa alıcı adresi (ör. yeni e-posta); yoksa <paramref name="user"/>.Email.</param>
        public static async Task<bool> TrySendCodeEmailAsync(
            IMailService mailService,
            IConfiguration configuration,
            ILogger logger,
            User user,
            string code,
            int validityMinutes,
            MailPurpose purpose,
            CancellationToken cancellationToken,
            string sendToEmailOverride = null)
        {
            var smtpServer = (configuration["EmailConfiguration:SmtpServer"] ?? string.Empty).Trim();
            var senderEmail = (configuration["EmailConfiguration:SenderEmail"] ?? string.Empty).Trim();
            var appMode = (configuration["AppSettings:Mode"] ?? string.Empty).Trim();
            var recipient = string.IsNullOrWhiteSpace(sendToEmailOverride)
                ? (user.Email ?? string.Empty).Trim()
                : sendToEmailOverride.Trim();

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail))
            {
                if (string.Equals(appMode, "Development", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning(
                        "{Purpose} (SMTP yapılandırılmadı, Development): {Email} kodu: {Code}",
                        purpose, recipient, code);
                }
                else
                {
                    logger.LogWarning(
                        "Doğrulama e-postası gönderilemedi: SMTP veya gönderen e-posta eksik. Alıcı: {Email}",
                        recipient);
                }

                return true;
            }

            try
            {
                var senderName = configuration["EmailConfiguration:SenderName"] ?? BrandProduct;
                var safeCode = WebUtility.HtmlEncode(code ?? string.Empty);
                var displayName = string.IsNullOrWhiteSpace(user.FullName)
                    ? null
                    : WebUtility.HtmlEncode(user.FullName.Trim());
                var greeting = displayName != null
                    ? $"<p>Merhaba {displayName},</p>"
                    : "<p>Merhaba,</p>";

                string subject;
                string html;
                if (purpose == MailPurpose.PasswordReset)
                {
                    subject = $"{BrandProduct} - Şifre Sıfırlama";
                    html = BuildCuzdanimStyleHtml(
                        headerTitle: "Şifre Sıfırlama",
                        greetingHtml: greeting,
                        introHtml: $"<p><strong>{BrandProduct}</strong> hesabınız için şifre sıfırlama talebinde bulunuldu. Aşağıdaki doğrulama kodunu uygulamaya giriniz:</p>",
                        code: safeCode,
                        stepsTitle: "Nasıl kullanılır?",
                        stepsHtml:
                            "<ol style='text-align: left; padding-left: 20px;'>" +
                            $"<li>Uygulamadaki <strong>\"Şifre Sıfırlama\"</strong> ekranını açın</li>" +
                            "<li>Yukarıdaki doğrulama kodunu giriniz</li>" +
                            "<li>Yeni şifrenizi belirleyiniz</li>" +
                            "</ol>",
                        importantHtml:
                            $"<p><strong>Önemli:</strong> Bu kod <strong>{validityMinutes}</strong> dakika geçerlidir. " +
                            "Bu isteği siz yapmadıysanız bu e-postayı görmezden gelebilirsiniz; şifreniz değişmeyecektir.</p>");
                }
                else if (purpose == MailPurpose.EmailAddressChange)
                {
                    subject = $"{BrandProduct} - E-posta adresi değişikliği";
                    html = BuildCuzdanimStyleHtml(
                        headerTitle: "E-posta adresinizi doğrulayın",
                        greetingHtml: greeting,
                        introHtml:
                            $"<p><strong>{BrandProduct}</strong> hesabınız için yeni bir e-posta adresi talep edildi. " +
                            "Bu adresin size ait olduğunu doğrulamak için aşağıdaki kodu uygulamadaki <strong>Profili düzenle</strong> ekranına giriniz:</p>",
                        code: safeCode,
                        stepsTitle: "Nasıl kullanılır?",
                        stepsHtml:
                            "<ol style='text-align: left; padding-left: 20px;'>" +
                            "<li>Uygulamayı açın ve <strong>Ayarlar → Profili düzenle</strong> bölümüne gidin</li>" +
                            "<li>Yukarıdaki doğrulama kodunu giriniz</li>" +
                            "<li><strong>E-postayı onayla</strong> ile işlemi tamamlayın</li>" +
                            "</ol>",
                        importantHtml:
                            $"<p><strong>Önemli:</strong> Bu kod <strong>{validityMinutes}</strong> dakika geçerlidir. " +
                            "Bu değişikliği siz talep etmediyseniz bu e-postayı yok sayabilirsiniz; mevcut e-posta adresiniz değişmez.</p>");
                }
                else
                {
                    subject = $"{BrandProduct} - E-posta Doğrulama";
                    html = BuildCuzdanimStyleHtml(
                        headerTitle: $"{BrandProduct}'a Hoş Geldiniz!",
                        greetingHtml: greeting,
                        introHtml:
                            $"<p>Hesabınızı oluşturduğunuz için teşekkür ederiz. <strong>{BrandProduct}</strong> hesabınızı aktifleştirmek için aşağıdaki doğrulama kodunu uygulamaya giriniz:</p>",
                        code: safeCode,
                        stepsTitle: "Nasıl kullanılır?",
                        stepsHtml:
                            "<ol style='text-align: left; padding-left: 20px;'>" +
                            $"<li>Uygulamadaki <strong>\"E-posta Doğrulama\"</strong> ekranını açın</li>" +
                            "<li>Yukarıdaki doğrulama kodunu giriniz</li>" +
                            "<li><strong>Doğrula</strong> ile işlemi tamamlayın</li>" +
                            "</ol>",
                        importantHtml:
                            $"<p><strong>Önemli:</strong> Bu kod <strong>{validityMinutes}</strong> dakika geçerlidir. " +
                            "Süre içinde doğrulama yapmazsanız hesabınız aktif olmayacaktır.</p>" +
                            "<p>Bu hesabı siz oluşturmadıysanız bu e-postayı görmezden gelebilirsiniz.</p>");
                }

                var email = new EmailMessage
                {
                    Subject = subject,
                    Content = html,
                };
                email.ToAddresses.Add(new EmailAddress { Name = user.FullName, Address = recipient });
                email.FromAddresses.Add(new EmailAddress { Name = senderName, Address = senderEmail });

                await Task.Run(() => mailService.Send(email), cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Doğrulama e-postası gönderilemedi ({Purpose}): {Email}", purpose, recipient);
                return false;
            }
        }

        /// <summary>Cüzdanım e-posta şablonu ile aynı yerleşim (header, code-box, liste, footer).</summary>
        private static string BuildCuzdanimStyleHtml(
            string headerTitle,
            string greetingHtml,
            string introHtml,
            string code,
            string stepsTitle,
            string stepsHtml,
            string importantHtml)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: {AccentColor}; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .header-sub {{ margin: 10px 0 0; font-size: 14px; opacity: 0.95; font-weight: normal; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .code-box {{ background-color: #fff; border: 2px solid {AccentColor}; border-radius: 8px; padding: 20px; margin: 20px 0; text-align: center; }}
        .code {{ font-family: 'Courier New', monospace; font-size: 18px; font-weight: bold; color: {AccentColor}; letter-spacing: 2px; word-break: break-all; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
        .brand-line {{ margin-top: 12px; font-size: 13px; color: #555; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin:0;font-size:22px'>{WebUtility.HtmlEncode(headerTitle)}</h1>
            <p class='header-sub'>{BrandProduct} · {BrandCompany}</p>
        </div>
        <div class='content'>
            {greetingHtml}
            {introHtml}
            <div class='code-box'>
                <p style='margin: 0 0 10px 0; font-weight: bold;'>Doğrulama Kodu:</p>
                <p class='code'>{code}</p>
            </div>
            <p><strong>{WebUtility.HtmlEncode(stepsTitle)}</strong></p>
            {stepsHtml}
            {importantHtml}
        </div>
        <div class='footer'>
            <p>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.</p>
            <p class='brand-line'><strong>{BrandProduct}</strong> — {BrandCompany}</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
