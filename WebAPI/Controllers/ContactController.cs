using System.Threading.Tasks;
using Business.Handlers.Contact.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>İletişim formu (SMTP ile support@masavtech.com).</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : BaseApiController
    {
        [Authorize]
        [Consumes("application/json")]
        [HttpPost("send")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendContactMessage([FromBody] SendContactMessageCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }
    }
}
