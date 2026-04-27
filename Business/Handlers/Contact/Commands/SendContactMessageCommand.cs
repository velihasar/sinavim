using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Business.Handlers.Contact.ValidationRules;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Business.Handlers.Contact.Commands
{
    /// <summary>İletişim formu: gönderen yapılandırmadaki no-reply, alıcı support@masavtech.com.</summary>
    public class SendContactMessageCommand : IRequest<IResult>
    {
        public string FullName { get; set; }
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class SendContactMessageCommandHandler : IRequestHandler<SendContactMessageCommand, IResult>
    {
        private const string SupportEmail = "support@masavtech.com";

        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public SendContactMessageCommandHandler(IMailService mailService, IConfiguration configuration)
        {
            _mailService = mailService;
            _configuration = configuration;
        }

        [ValidationAspect(typeof(SendContactMessageValidator), Priority = 1)]
        [LogAspect(typeof(FileLogger))]
        public Task<IResult> Handle(SendContactMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = _configuration.GetSection("EmailConfiguration");
                var senderEmail = MailManager.ResolveConfigurationValue(section["SenderEmail"]);
                var senderName = MailManager.ResolveConfigurationValue(section["SenderName"]);

                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    senderEmail = "no-reply@masavtech.com";
                }

                if (string.IsNullOrWhiteSpace(senderName))
                {
                    senderName = "Sınavım";
                }

                var appName = senderName;
                var safeName = WebUtility.HtmlEncode((request.FullName ?? string.Empty).Trim());
                var safeMail = WebUtility.HtmlEncode((request.UserEmail ?? string.Empty).Trim());
                var safeSubject = WebUtility.HtmlEncode((request.Subject ?? string.Empty).Trim());
                var bodyText = request.Message ?? string.Empty;
                var safeBody = WebUtility.HtmlEncode(bodyText).Replace("\n", "<br>", StringComparison.Ordinal);

                var emailContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0a7ea4; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .info {{ background-color: #fff; padding: 15px; margin: 15px 0; border-left: 4px solid #0a7ea4; }}
        .message {{ background-color: #fff; padding: 20px; margin: 15px 0; border-radius: 5px; }}
        .app-info {{ background-color: #e3f2fd; padding: 10px; margin: 15px 0; border-radius: 5px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2 style='margin:0;font-size:20px'>Yeni İletişim Formu Mesajı</h2>
        </div>
        <div class='content'>
            <div class='app-info'>
                <strong>Uygulama:</strong> {WebUtility.HtmlEncode(appName)}
            </div>
            <div class='info'>
                <strong>Ad Soyad:</strong> {safeName}<br>
                <strong>E-posta:</strong> {safeMail}<br>
                <strong>Konu:</strong> {safeSubject}
            </div>
            <div class='message'>
                <strong>Mesaj:</strong><br>
                {safeBody}
            </div>
        </div>
    </div>
</body>
</html>";

                var emailMessage = new EmailMessage
                {
                    ToAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = "MasavTech Destek", Address = SupportEmail },
                    },
                    FromAddresses = new List<EmailAddress>
                    {
                        new EmailAddress { Name = senderName, Address = senderEmail },
                    },
                    Subject = $"{appName} - İletişim: {request.Subject.Trim()}",
                    Content = emailContent,
                };

                if (!string.IsNullOrWhiteSpace(request.UserEmail))
                {
                    emailMessage.ReplyToAddresses.Add(
                        new EmailAddress
                        {
                            Name = string.IsNullOrWhiteSpace(request.FullName)
                                ? "Kullanıcı"
                                : request.FullName.Trim(),
                            Address = request.UserEmail.Trim(),
                        });
                }

                _mailService.Send(emailMessage);
                return Task.FromResult<IResult>(new SuccessResult("Mesajınız başarıyla gönderildi."));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IResult>(
                    new ErrorResult("Mesaj gönderilirken bir hata oluştu: " + ex.Message));
            }
        }
    }
}
