
using Business.Handlers.KullaniciGunlukSoruCozumus.Commands;
using Business.Handlers.KullaniciGunlukSoruCozumus.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Core.Entities.Dtos.Project.KullaniciGunlukSoruCozumuDtos;

namespace WebAPI.Controllers
{
    /// <summary>
    /// KullaniciGunlukSoruCozumus If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KullaniciGunlukSoruCozumusController : BaseApiController
    {
        ///<summary>
        ///List KullaniciGunlukSoruCozumus
        ///</summary>
        ///<remarks>KullaniciGunlukSoruCozumus</remarks>
        ///<return>List KullaniciGunlukSoruCozumus</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<KullaniciGunlukSoruCozumuDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetKullaniciGunlukSoruCozumusQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        ///<summary>
        ///It brings the details according to its id.
        ///</summary>
        ///<remarks>KullaniciGunlukSoruCozumus</remarks>
        ///<return>KullaniciGunlukSoruCozumus List</return>
        ///<response code="200"></response>  
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KullaniciGunlukSoruCozumuDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetKullaniciGunlukSoruCozumuQuery { Id = id });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>Oturumdaki kullanıcı: anasayfa için 8 gün, Türkiye takvimi (en eski gün yalnızca trend kıyası); grafikte son 7 gün gösterilir.</summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GunlukSoruGunOzetDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("ben/son-7-gun")]
        public async Task<IActionResult> GetSon7GunForMe()
        {
            var result = await Mediator.Send(new GetGunlukSoruCozumuSon7ForMeQuery());
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>Oturumdaki kullanıcı: günlük çözüm sayfalama (bugünden geçmişe, eksik gün 0). En fazla son 90 gün.</summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GunlukSoruCozumuPageForMeDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("ben/gunler")]
        public async Task<IActionResult> GetGunlerForMe([FromQuery] int offsetDays = 0, [FromQuery] int take = 30)
        {
            var result = await Mediator.Send(new GetGunlukSoruCozumuPageForMeQuery
            {
                OffsetDays = offsetDays,
                Take = take,
            });
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        /// <summary>Oturumdaki kullanıcı için seçilen güne çözülen soru sayısı (varsa günceller, yoksa ekler).</summary>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("ben")]
        public async Task<IActionResult> UpsertMy([FromBody] UpsertMyKullaniciGunlukSoruCozumuCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Add KullaniciGunlukSoruCozumu.
        /// </summary>
        /// <param name="createKullaniciGunlukSoruCozumu"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateKullaniciGunlukSoruCozumuCommand createKullaniciGunlukSoruCozumu)
        {
            var result = await Mediator.Send(createKullaniciGunlukSoruCozumu);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Update KullaniciGunlukSoruCozumu.
        /// </summary>
        /// <param name="updateKullaniciGunlukSoruCozumu"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateKullaniciGunlukSoruCozumuCommand updateKullaniciGunlukSoruCozumu)
        {
            var result = await Mediator.Send(updateKullaniciGunlukSoruCozumu);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Delete KullaniciGunlukSoruCozumu.
        /// </summary>
        /// <param name="deleteKullaniciGunlukSoruCozumu"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteKullaniciGunlukSoruCozumuCommand deleteKullaniciGunlukSoruCozumu)
        {
            var result = await Mediator.Send(deleteKullaniciGunlukSoruCozumu);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
