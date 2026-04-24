using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public User()
        {
            if(UserId==0){
              RecordDate = DateTime.Now;
            }
            UpdateContactDate = DateTime.Now;
            Status = true;
        }

        public int UserId { get; set; }
        //public long CitizenId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public string MobilePhones { get; set; }
        /// <summary>Cüzdanım benzeri: false iken hesap e-posta doğrulanmamış (giriş yok).</summary>
        public bool Status { get; set; }
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
        public DateTime RecordDate { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public DateTime UpdateContactDate { get; set; }

        /// <summary>
        /// This is required when encoding token. Not in db. The default is Person.
        /// </summary>
        [NotMapped]
        public string AuthenticationProviderType { get; set; } = "Person";

        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }

        /// <summary>Şifre sıfırlama için gönderilen kod (düz metin, Cüzdanım ile aynı yaklaşım).</summary>
        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>Kayıt e-posta doğrulama kodu.</summary>
        public string EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }

        /// <summary>Doğrulanmayı bekleyen yeni e-posta (mevcut <see cref="Email"/> giriş için kullanılmaya devam eder).</summary>
        public string PendingEmail { get; set; }

        public bool UpdateMobilePhone(string mobilePhone)
        {
            if (MobilePhones == mobilePhone)
            {
                return false;
            }

            MobilePhones = mobilePhone;
            return true;
        }
    }
}
