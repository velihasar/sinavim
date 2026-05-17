
using Business.Handlers.KullaniciDersNetHedefis.Commands;
using Business.Handlers.KullaniciDersNetHedefis.Queries;
using Core.Entities.Dtos.Project.KullaniciDersNetHedefiDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebAPI.Controllers
{
    /// <summary>
    /// KullaniciDersNetHedefis If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciDersNetHedefisController : BaseApiController
    {
        ///<summary>
        ///List KullaniciDersNetHedefis
        ///</summary>
        ///<remarks>KullaniciDersNetHedefis</remarks>
        ///<return>List KullaniciDersNetHedefis</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciDersNetHedefiDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetKullaniciDersNetHedefisQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>KullaniciDersNetHedefis</remarks>
        ///<return>KullaniciDersNetHedefis List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciDersNetHedefiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetKullaniciDersNetHedefiQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add KullaniciDersNetHedefi.
        /// </summary>
        /// <param name="createKullaniciDersNetHedefi"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciDersNetHedefiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateKullaniciDersNetHedefiCommand createKullaniciDersNetHedefi)
        {
            var result = await Mediator.Send(createKullaniciDersNetHedefi);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update KullaniciDersNetHedefi.
        /// </summary>
        /// <param name="updateKullaniciDersNetHedefi"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciDersNetHedefiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateKullaniciDersNetHedefiCommand updateKullaniciDersNetHedefi)
        {
            var result = await Mediator.Send(updateKullaniciDersNetHedefi);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete KullaniciDersNetHedefi.
        /// </summary>
        /// <param name="deleteKullaniciDersNetHedefi"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteKullaniciDersNetHedefiCommand deleteKullaniciDersNetHedefi)
        {
            var result = await Mediator.Send(deleteKullaniciDersNetHedefi);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>Oturumdaki kullanıcının ders bazlı deneme net hedefleri.</summary>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciDersNetHedefiDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getmy")]
        public async Task<IActionResult> GetMy()
        {
            var result = await Mediator.Send(new GetMyKullaniciDersNetHedefleriQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        /// <summary>Tek ders için hedef ekler/günceller; HedefNet 0 veya daha küçükse kaydı siler.</summary>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciDersNetHedefiDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("upsertmy")]
        public async Task<IActionResult> UpsertMy([FromBody] UpsertMyKullaniciDersNetHedefiCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}
