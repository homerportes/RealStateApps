using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Login.Commands;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// Controlador para el login de usuarios y generación de JWT
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LoginController : BaseApiController
    {
        /// <summary>
        /// Inicia sesión con usuario y contraseña y retorna un JWT
        /// </summary>
        /// <param name="loginCommand">Datos de usuario y contraseña</param>
        /// <returns>Token JWT si las credenciales son correctas</returns>
        [AllowAnonymous]
        [HttpPost("login", Name = "IniciarSesion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(loginCommand);

            if (result.HasError)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            return Ok(new { token = result.AccessToken });
        }
    }
}
