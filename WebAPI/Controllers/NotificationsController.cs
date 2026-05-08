using Business.Handlers.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>Push bildirim kaydı ve gönderimi (FCM).</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseApiController
    {
        /// <summary>Oturumdaki kullanıcı için FCM token kaydı (JWT'den kullanıcı id).</summary>
        [Authorize]
        [Consumes("application/json")]
        [HttpPost("register-token")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterFcmTokenCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }

        /// <summary>
        /// Push gönder: <c>targetUserIds</c> ile çoklu, <c>userId</c> ile tek, ikisi yoksa herkese (token'ı olanlar).
        /// </summary>
        [Authorize]
        [Consumes("application/json")]
        [HttpPost("send")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendNotification([FromBody] SendPushNotificationCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }
    }
}
