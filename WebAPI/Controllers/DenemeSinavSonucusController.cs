
using Business.Handlers.DenemeSinavSonucus.Commands;
using Business.Handlers.DenemeSinavSonucus.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinavSonucuDtos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DenemeSinavSonucus If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DenemeSinavSonucusController : BaseApiController
    {
        ///<summary>
        ///List DenemeSinavSonucus
        ///</summary>
        ///<remarks>DenemeSinavSonucus</remarks>
        ///<return>List DenemeSinavSonucus</return>
        ///<response code="200"></response>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DenemeSinavSonucuListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetDenemeSinavSonucusQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>DenemeSinavSonucus</remarks>
        ///<return>DenemeSinavSonucus List</return>
        ///<response code="200"></response>  
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DenemeSinavSonucuDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetDenemeSinavSonucuQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add DenemeSinavSonucu.
        /// </summary>
        /// <param name="createDenemeSinavSonucu"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDenemeSinavSonucuDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateDenemeSinavSonucuCommand createDenemeSinavSonucu)
        {
            var result = await Mediator.Send(createDenemeSinavSonucu);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update DenemeSinavSonucu.
        /// </summary>
        /// <param name="updateDenemeSinavSonucu"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateDenemeSinavSonucuDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDenemeSinavSonucuCommand updateDenemeSinavSonucu)
        {
            var result = await Mediator.Send(updateDenemeSinavSonucu);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete DenemeSinavSonucu.
        /// </summary>
        /// <param name="deleteDenemeSinavSonucu"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteDenemeSinavSonucuCommand deleteDenemeSinavSonucu)
        {
            var result = await Mediator.Send(deleteDenemeSinavSonucu);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
