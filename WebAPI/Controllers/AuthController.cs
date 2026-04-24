using System.Threading.Tasks;
using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Business.Handlers.Users.Commands;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Entities.Dtos;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Make it Authorization operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {

        /// <summary>
        /// Make it User Login operations
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AccessToken>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserQuery loginModel)
        {
            var result = await Mediator.Send(loginModel);
            return result.Success ? Ok(result) : Unauthorized(result.Message);
        }

        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AccessToken>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> LoginWithRefreshToken([FromBody] LoginWithRefreshTokenQuery command)
        {
            var result = await Mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        ///  Make it User Register operations
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand createUser)
        {
            return GetResponseOnlyResult(await Mediator.Send(createUser));
        }

        /// <summary>
        /// Kayıt sonrası e-posta doğrulama: 6 haneli kod ile onaylama.
        /// </summary>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("verify-email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Doğrulanmamış hesaba yeni e-posta doğrulama kodu gönderir.
        /// </summary>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("verify-email/request")]
        public async Task<IActionResult> RequestEmailVerification([FromBody] RequestEmailVerificationCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Giriş yapmış kullanıcı: yeni e-posta adresine doğrulama kodu gönderir.
        /// </summary>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("verify-email/change/request")]
        public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Yeni e-posta adresine gelen kod ile adres değişikliğini onaylar.
        /// </summary>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("verify-email/change/confirm")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Şifre sıfırlama: e-posta ile 6 haneli kod talebi (kayıtlı adreslere gönderilir).
        /// </summary>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("forgot-password/request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Şifre sıfırlama: kod ve yeni şifre ile onaylama.
        /// </summary>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("forgot-password/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetCommand command)
        {
            return GetResponseOnlyResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Make it Change Password operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPut("user-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] UserChangePasswordCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }

        /// <summary>
        /// Mobile Login
        /// </summary>
        /// <param name="verifyCid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpPost("verify")]
        public async Task<IActionResult> Verification([FromBody] VerifyCidQuery verifyCid)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(verifyCid));
        }

        /// <summary>
        /// Token decode test
        /// </summary>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [HttpPost("test")]
        public IActionResult LoginTest()
        {
            var auth = Request.Headers["Authorization"];
            var token = JwtHelper.DecodeToken(auth);

            return Ok(token);
        }
    }
}