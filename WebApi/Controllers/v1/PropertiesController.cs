using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Property.Commands.EditProperty;
using RealStateApp.Core.Application.Features.Property.Commands.UpdateProperty;
using RealStateApp.Core.Application.Features.Property.Queries.GetAll;
using RealStateApp.Core.Application.Features.Property.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Property.Queries.GetByCode;
using RealStateApp.Core.Application.Features.Property.Queries.GetById;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
            Summary = "Obtiene todas las propiedades",
            Description = "Devuelve una lista de todas las propiedades registradas en el sistema."
        )]
        public async Task<IActionResult> GetAll()
        public async Task<IActionResult> List()
            Description = "Devuelve una lista de todas las propiedades registradas en el sistema."
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWithInclude()
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
        [SwaggerOperation(
            Summary = "Obtiene una propiedad por Id",
            Description = "Devuelve los datos de una propiedad específica utilizando su Id."
        )]
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
        [SwaggerOperation(
            Summary = "Obtiene una propiedad por Código",
            Description = "Devuelve los datos de una propiedad específica utilizando su código único de 6 caracteres."
        )]
        public async Task<IActionResult> GetByCode([FromRoute] string code)
        {
            var response = await Mediator.Send(new GetPropertyByCodeQuery { Code = code });

            if (response == null)
            {
                return NotFound(new { message = $"No existe la propiedad con el código {code}" });
            }
            return Ok(response);
        }

        /// <summary>
        /// Crea una nueva propiedad
        /// </summary>
        /// <param name="command">Datos de la propiedad a crear</param>
        /// <returns>Id de la propiedad creada</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command)
        {
            var propertyId = await Mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, propertyId);
        }

        /// <summary>
        /// Actualiza una propiedad existente por código
        /// </summary>
        /// <param name="code">Código de la propiedad</param>
        /// <param name="command">Datos actualizados de la propiedad</param>
        /// <returns>Confirmación de actualización</returns>
        [HttpPut("{code}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] string code, [FromBody] UpdatePropertyCommand command)
        {
            command.Code = code;
            await Mediator.Send(command);
            return Ok(new { message = "Propiedad actualizada exitosamente" });
        }

        /// <summary>
        /// Edita una propiedad existente por ID
        /// </summary>
        /// <param name="id">ID de la propiedad</param>
        /// <param name="command">Datos actualizados de la propiedad</param>
        /// <returns>Confirmación de actualización</returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] EditPropertyCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return Ok(new { message = "Propiedad editada exitosamente" });
        }

        /// <summary>
        /// Elimina una propiedad
        /// </summary>
        /// <param name="code">Código de la propiedad a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{code}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] string code)
        {
            await Mediator.Send(new DeletePropertyCommand { Code = code });
            return NoContent();
        }
    }
}
