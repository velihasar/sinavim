using Business.Handlers.ArkadaslikApp.Commands;
using Business.Handlers.ArkadaslikApp.Queries;
using Core.Entities.Dtos.Project.ArkadaslikDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>Mobil uygulama için arkadaşlık ve davet kodu uçları.</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArkadaslikController : BaseApiController
    {
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciDavetKoduDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("davetkodum")]
        public async Task<IActionResult> GetMyDavetKodu()
        {
            var result = await Mediator.Send(new GetOrCreateMyDavetKoduQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArkadaslikIstegiListItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("istek-gonder")]
        public async Task<IActionResult> SendIstek([FromBody] SendMyArkadaslikIstegiByDavetKoduCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArkadaslikBadgeOzetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("rozet-ozet")]
        public async Task<IActionResult> GetBadgeOzet()
        {
            var result = await Mediator.Send(new GetMyArkadaslikBadgeOzetQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("kabul-bildirimleri/goruldu")]
        public async Task<IActionResult> MarkKabulBildirimleriGoruldu()
        {
            var result = await Mediator.Send(new MarkMyKabulBildirimleriGorulduCommand());
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MyArkadaslikIstekleriDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("isteklerim")]
        public async Task<IActionResult> GetMyIstekler()
        {
            var result = await Mediator.Send(new GetMyArkadaslikIstekleriQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("istek/{istekId:int}/kabul")]
        public async Task<IActionResult> AcceptIstek(int istekId)
        {
            var result = await Mediator.Send(new AcceptMyArkadaslikIstegiCommand { IstekId = istekId });
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("istek/{istekId:int}/red")]
        public async Task<IActionResult> RejectIstek(int istekId)
        {
            var result = await Mediator.Send(new RejectMyArkadaslikIstegiCommand { IstekId = istekId });
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("istek/{istekId:int}/iptal")]
        public async Task<IActionResult> CancelIstek(int istekId)
        {
            var result = await Mediator.Send(new CancelMyArkadaslikIstegiCommand { IstekId = istekId });
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArkadasOzetDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("arkadaslarim")]
        public async Task<IActionResult> GetMyArkadaslar()
        {
            var result = await Mediator.Send(new GetMyArkadaslarQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArkadasRekabetDashboardOzetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("rekabet-dashboard-ozet")]
        public async Task<IActionResult> GetRekabetDashboardOzet()
        {
            var result = await Mediator.Send(new GetArkadasRekabetDashboardOzetQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArkadasRekabetDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("rekabet")]
        public async Task<IActionResult> GetRekabet([FromQuery] int? gunSayisi)
        {
            var result = await Mediator.Send(new GetArkadasRekabetQuery
            {
                GunSayisi = gunSayisi ?? 7,
            });
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete("{arkadaslikId:int}")]
        public async Task<IActionResult> RemoveArkadaslik(int arkadaslikId)
        {
            var result = await Mediator.Send(new RemoveMyArkadaslikCommand { ArkadaslikId = arkadaslikId });
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }
    }
}
