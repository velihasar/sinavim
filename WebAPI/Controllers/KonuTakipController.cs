using Business.Handlers.KonuTakip.Commands;
using Business.Handlers.KonuTakip.Queries;
using Core.Entities.Dtos.Project.KonuTakipDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KonuTakipController : BaseApiController
    {
        /// <summary>
        /// Seçilen sınav ve isteğe bağlı sınav bölümü için ders / konu özeti (oturumdaki kullanıcı).
        /// </summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KonuTakipForMeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("forme")]
        public async Task<IActionResult> GetForMe([FromQuery] int? sinavBolumId = null)
        {
            var result = await Mediator.Send(new GetKonuTakipForMeQuery { SinavBolumId = sinavBolumId });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>Eksik veya yanlış konu bildirimi (SMTP ile support@masavtech.com).</summary>
        [Consumes("application/json")]
        [HttpPost("report-issue")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> ReportIssue([FromBody] ReportKonuIssueCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }
    }
}
