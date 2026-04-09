
using Business.Handlers.KullaniciSinavs.Commands;
using Business.Handlers.KullaniciSinavs.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KullaniciSinavDtos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// KullaniciSinavs If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciSinavsController : BaseApiController
    {
        ///<summary>
        ///List KullaniciSinavs
        ///</summary>
        ///<remarks>KullaniciSinavs</remarks>
        ///<return>List KullaniciSinavs</return>
        ///<response code="200"></response>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciSinavListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetKullaniciSinavsQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Oturumdaki kullanıcının sınav seçimleri (boşsa henüz seçim yoktur).
        /// </summary>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciSinavListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getmy")]
        public async Task<IActionResult> GetMyList()
        {
            var result = await Mediator.Send(new GetMyKullaniciSinavsQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>KullaniciSinavs</remarks>
        ///<return>KullaniciSinavs List</return>
        ///<response code="200"></response>  
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciSinavDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetKullaniciSinavQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add KullaniciSinav.
        /// </summary>
        /// <param name="createKullaniciSinav"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateKullaniciSinavDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateKullaniciSinavCommand createKullaniciSinav)
        {
            var result = await Mediator.Send(createKullaniciSinav);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Giriş yapan kullanıcı için sınav seçimi (UserId token’dan alınır).
        /// </summary>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateKullaniciSinavDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("sec")]
        public async Task<IActionResult> SecForMe([FromBody] CreateKullaniciSinavSelfCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update KullaniciSinav.
        /// </summary>
        /// <param name="updateKullaniciSinav"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateKullaniciSinavDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateKullaniciSinavCommand updateKullaniciSinav)
        {
            var result = await Mediator.Send(updateKullaniciSinav);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }


        /// <summary>
        /// Delete KullaniciSinav.
        /// </summary>
        /// <param name="deleteKullaniciSinav"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteKullaniciSinavCommand deleteKullaniciSinav)
        {
            var result = await Mediator.Send(deleteKullaniciSinav);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
