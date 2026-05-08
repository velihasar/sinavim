using Business.BusinessAspects;
using Business.Constants;
using Business.Services;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Users.Commands
{
    /// <summary>
    /// Push bildirim gönderir.
    /// - <see cref="TargetUserIds"/> doluysa: sadece bu kullanıcı id'lerine (token varsa).
    /// - <see cref="UserId"/> doluysa: tek kullanıcı.
    /// - İkisi de boşsa: FCM token'ı olan tüm aktif kullanıcılar.
    /// </summary>
    public class SendPushNotificationCommand : IRequest<IResult>
    {
        /// <summary>Belirli kullanıcı id listesi (öncelikli).</summary>
        public List<int> TargetUserIds { get; set; }

        /// <summary>Tek kullanıcı (TargetUserIds boşsa).</summary>
        public int? UserId { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }

        public class SendPushNotificationCommandHandler : IRequestHandler<SendPushNotificationCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IFirebaseNotificationService _firebaseNotificationService;

            public SendPushNotificationCommandHandler(
                IUserRepository userRepository,
                IFirebaseNotificationService firebaseNotificationService)
            {
                _userRepository = userRepository;
                _firebaseNotificationService = firebaseNotificationService;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
                {
                    return new ErrorResult("Başlık ve mesaj gereklidir.");
                }

                if (request.TargetUserIds != null && request.TargetUserIds.Count > 0)
                {
                    var ids = request.TargetUserIds.Where(id => id > 0).Distinct().ToList();
                    var users = await _userRepository.GetListAsync(u =>
                        ids.Contains(u.UserId) && u.Status && !string.IsNullOrWhiteSpace(u.FcmToken));

                    var tokens = users.Select(u => u.FcmToken).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
                    if (tokens.Length == 0)
                    {
                        return new ErrorResult("Seçilen kullanıcılar için kayıtlı FCM token bulunamadı.");
                    }

                    var ok = await _firebaseNotificationService.SendNotificationToMultipleAsync(
                        tokens,
                        request.Title,
                        request.Body,
                        request.Data);

                    return ok
                        ? new SuccessResult($"{tokens.Length} kullanıcıya push notification gönderildi.")
                        : new ErrorResult("Push notification gönderilemedi.");
                }

                if (request.UserId.HasValue)
                {
                    var user = await _userRepository.GetAsync(u => u.UserId == request.UserId.Value);
                    if (user == null)
                    {
                        return new ErrorResult(Messages.UserNotFound);
                    }

                    if (string.IsNullOrWhiteSpace(user.FcmToken))
                    {
                        return new ErrorResult("Kullanıcının FCM token'ı kayıtlı değil.");
                    }

                    var success = await _firebaseNotificationService.SendNotificationAsync(
                        user.FcmToken,
                        request.Title,
                        request.Body,
                        request.Data);

                    return success
                        ? new SuccessResult("Push notification başarıyla gönderildi.")
                        : new ErrorResult("Push notification gönderilemedi.");
                }

                var allUsers = await _userRepository.GetListAsync(u =>
                    !string.IsNullOrWhiteSpace(u.FcmToken) && u.Status);
                var allTokens = allUsers.Select(u => u.FcmToken).ToArray();

                if (allTokens.Length == 0)
                {
                    return new ErrorResult("FCM token'ı olan kullanıcı bulunamadı.");
                }

                var broadcastOk = await _firebaseNotificationService.SendNotificationToMultipleAsync(
                    allTokens,
                    request.Title,
                    request.Body,
                    request.Data);

                return broadcastOk
                    ? new SuccessResult($"{allTokens.Length} kullanıcıya push notification gönderildi.")
                    : new ErrorResult("Push notification gönderilemedi.");
            }
        }
    }
}
