using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Property.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Property.Queries.GetByCode;
using RealStateApp.Core.Application.Features.Property.Queries.GetById;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// API Propiedades V 1.0
    /// Controlador de consulta de Propiedades
    /// </summary>
    public class PropertiesController : BaseApiController
    {
        /// <summary>
        /// Obtiene todas las propiedades del sistema
        /// </summary>
        /// <returns>Lista de todas las propiedades con sus relaciones (PropertyType, SaleType, Improvements)</returns>
        /// <response code="200">Retorna el listado de todas las propiedades en formato JSON</response>
        /// <response code="204">No existen propiedades en el sistema</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var response = await Mediator.Send(new GetAllPropertiesWithIncludeQuery());
            
            if (response == null || response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una propiedad por su Id
        /// </summary>
        /// <param name="id">Id de la propiedad</param>
        /// <returns>Datos de la propiedad en formato JSON</returns>
        /// <response code="200">Retorna los datos de la propiedad</response>
        /// <response code="404">No existe la propiedad con ese Id</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetPropertyByIdQuery { Id = id });
            
            if (response == null)
            {
                return NotFound(new { message = $"No existe la propiedad con el Id {id}" });
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una propiedad por su Código único
        /// </summary>
        /// <param name="code">Código de la propiedad (6 caracteres alfanuméricos)</param>
        /// <returns>Datos de la propiedad en formato JSON</returns>
        /// <response code="200">Retorna los datos de la propiedad</response>
        /// <response code="404">No existe la propiedad con ese código</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("code/{code}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            var response = await Mediator.Send(new GetPropertyByCodeQuery { Code = code });
            
            if (response == null)
            {
                return NotFound(new { message = $"No existe la propiedad con el código {code}" });
            }

            return Ok(response);
        }
    }
}
