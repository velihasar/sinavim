
using Business.Handlers.DenemeSinavis.Commands;
using Business.Handlers.DenemeSinavis.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Entities.Concrete;
using System.Collections.Generic;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DenemeSinaviDtos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// DenemeSinavis If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DenemeSinavisController : BaseApiController
    {
        ///<summary>
        ///List DenemeSinavis
        ///</summary>
        ///<remarks>DenemeSinavis</remarks>
        ///<return>List DenemeSinavis</return>
        ///<response code="200"></response>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DenemeSinaviListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetDenemeSinavisQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Oturumdaki kullanıcının seçili sınav için denemeleri (yeniden eskiye, toplam net ile).
        /// take: örn. 3 ile son N kayıt; verilmezse tümü (sayfalı değilken).
        /// includeSonuclar: ders bazlı D/Y/B ve net (tam liste / detay ekranı için).
        /// paged: true ise skip/take ile sayfalama; yanıt { totalCount, items }.
        /// sinavBolumId: isteğe bağlı bölüm filtresi (sayfalı + filtre chip).
        /// </summary>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DenemeSinaviOzetListDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("formesinav")]
        public async Task<IActionResult> GetMyForSinav(
            [FromQuery] int sinavId,
            [FromQuery] int? take = null,
            [FromQuery] int? skip = null,
            [FromQuery] bool includeSonuclar = false,
            [FromQuery] bool paged = false,
            [FromQuery] int? sinavBolumId = null)
        {
            var result = await Mediator.Send(new GetMyDenemeSinavisForSinavQuery
            {
                SinavId = sinavId,
                Take = take,
                Skip = skip,
                IncludeSonuclar = includeSonuclar,
                Paged = paged,
                SinavBolumId = sinavBolumId,
            });
            if (result.Success)
            {
                if (paged)
                {
                    return Ok(new
                    {
                        totalCount = result.Data.TotalCount,
                        items = result.Data.Items,
                    });
                }

                return Ok(result.Data.Items);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>DenemeSinavis</remarks>
        ///<return>DenemeSinavis List</return>
        ///<response code="200"></response>  
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DenemeSinaviDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetDenemeSinaviQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add DenemeSinavi.
        /// </summary>
        /// <param name="createDenemeSinavi"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateDenemeSinaviDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateDenemeSinaviCommand createDenemeSinavi)
        {
            var result = await Mediator.Send(createDenemeSinavi);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update DenemeSinavi.
        /// </summary>
        /// <param name="updateDenemeSinavi"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateDenemeSinaviDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateDenemeSinaviCommand updateDenemeSinavi)
        {
            var result = await Mediator.Send(updateDenemeSinavi);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete DenemeSinavi.
        /// </summary>
        /// <param name="deleteDenemeSinavi"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteDenemeSinaviCommand deleteDenemeSinavi)
        {
            var result = await Mediator.Send(deleteDenemeSinavi);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
