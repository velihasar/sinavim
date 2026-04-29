using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Utilities.Security.Google
{
    public static class GoogleTokenValidator
    {
        public static async Task<GoogleUserInfo> ValidateTokenAsync(string idToken, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(idToken))
            {
                throw new ArgumentException("ID token boş olamaz.");
            }

            var clientId = configuration["Google:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException(
                    "Google ClientId tanımlı değil. appsettings içinde 'Google:ClientId' (Web application OAuth Client ID) ayarlayın.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(idToken);

                var audClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                if (string.IsNullOrWhiteSpace(audClaim) || audClaim != clientId)
                {
                    throw new UnauthorizedAccessException(
                        $"Google token 'aud' claim'i beklenen Client ID ile eşleşmiyor. Beklenen: {clientId}, Token'da: {audClaim}");
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GoogleUserInfo
                {
                    Email = payload.Email,
                    Name = payload.Name,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    Picture = payload.Picture,
                    GoogleId = payload.Subject,
                    EmailVerified = payload.EmailVerified
                };
            }
            catch (Exception ex)
            {
                string errorMessage;
                if (ex.Message.Contains("audience", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("Audience", StringComparison.Ordinal))
                {
                    errorMessage =
                        "Google Client ID uyumsuzluğu. Frontend'deki Web Client ID ile backend Google:ClientId aynı olmalı.";
                }
                else if (ex.Message.Contains("expired", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Google giriş token'ı süresi dolmuş. Lütfen tekrar deneyin.";
                }
                else
                {
                    errorMessage = $"Geçersiz Google token: {ex.Message}";
                }

                throw new UnauthorizedAccessException(errorMessage, ex);
            }
        }
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Picture { get; set; }
        public string GoogleId { get; set; }
        public bool EmailVerified { get; set; }
    }
}
