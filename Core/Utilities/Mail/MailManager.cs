using System;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Core.Utilities.Mail
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Cüzdanım ile aynı: <c>${VAR}</c> değerlerini ortam değişkeninden çözümler.
        /// </summary>
        public static string ResolveConfigurationValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.StartsWith("${", StringComparison.Ordinal) && value.EndsWith("}", StringComparison.Ordinal)
                && value.Length > 3)
            {
                var envVarName = value.Substring(2, value.Length - 3);
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                if (!string.IsNullOrEmpty(envValue))
                    return envValue;
            }

            return value;
        }

        public void Send(EmailMessage emailMessage)
        {
            var section = _configuration.GetSection("EmailConfiguration");
            var smtpServer = ResolveConfigurationValue(section["SmtpServer"]);
            var smtpPortRaw = ResolveConfigurationValue(section["SmtpPort"]);
            var userName = ResolveConfigurationValue(section["UserName"] ?? section["SmtpUsername"]);
            var password = ResolveConfigurationValue(section["Password"] ?? section["SmtpPassword"]);

            if (!int.TryParse(smtpPortRaw, out var smtpPort))
                smtpPort = 587;

            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            if (emailMessage.ReplyToAddresses != null && emailMessage.ReplyToAddresses.Count > 0)
            {
                message.ReplyTo.AddRange(
                    emailMessage.ReplyToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            }

            message.Subject = emailMessage.Subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content ?? string.Empty
            };

            using var emailClient = new SmtpClient();
            emailClient.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);

            if (!string.IsNullOrWhiteSpace(userName))
                emailClient.Authenticate(userName, password ?? string.Empty);

            emailClient.Send(message);
            emailClient.Disconnect(true);
        }
    }
}
