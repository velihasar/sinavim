
using Business.Handlers.KullaniciKonuIlerlemes.Commands;
using Business.Handlers.KullaniciKonuIlerlemes.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;

namespace WebAPI.Controllers
{
    /// <summary>
    /// KullaniciKonuIlerlemes If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciKonuIlerlemesController : BaseApiController
    {
        ///<summary>
        ///List KullaniciKonuIlerlemes
        ///</summary>
        ///<remarks>KullaniciKonuIlerlemes</remarks>
        ///<return>List KullaniciKonuIlerlemes</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciKonuIlerleme>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetKullaniciKonuIlerlemesQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>KullaniciKonuIlerlemes</remarks>
        ///<return>KullaniciKonuIlerlemes List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciKonuIlerleme))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetKullaniciKonuIlerlemeQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add KullaniciKonuIlerleme.
        /// </summary>
        /// <param name="createKullaniciKonuIlerleme"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateKullaniciKonuIlerlemeCommand createKullaniciKonuIlerleme)
        {
            var result = await Mediator.Send(createKullaniciKonuIlerleme);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update KullaniciKonuIlerleme.
        /// </summary>
        /// <param name="updateKullaniciKonuIlerleme"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateKullaniciKonuIlerlemeCommand updateKullaniciKonuIlerleme)
        {
            var result = await Mediator.Send(updateKullaniciKonuIlerleme);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete KullaniciKonuIlerleme.
        /// </summary>
        /// <param name="deleteKullaniciKonuIlerleme"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteKullaniciKonuIlerlemeCommand deleteKullaniciKonuIlerleme)
        {
            var result = await Mediator.Send(deleteKullaniciKonuIlerleme);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
