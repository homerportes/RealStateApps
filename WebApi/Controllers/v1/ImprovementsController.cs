using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement;
using RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.Improvement.Queries.GetById;
using Swashbuckle.AspNetCore.Annotations;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// Controlador para la gestión de mejoras (Improvements)
    /// CRUD completo de mejoras
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerTag("Gestión de Improvements (crear, editar, eliminar, consultar)")]
    public class ImprovementsController : BaseApiController
    {
        /// <summary>
        /// Crea una nueva mejora
        /// </summary>
        /// <param name="command">Datos de la mejora</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Crea una nueva mejora",
            Description = "Crear una mejoras con todos sus componentes")]
        public async Task<IActionResult> Create([FromBody] CreateImprovementCommand command)
        {
            var response = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response }, response);
        }

        /// <summary>
        /// Edita una mejora existente
        /// </summary>
        /// <param name="id">Id de la mejora a editar</param>
        /// <param name="command">Datos actualizados de la mejora</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Edita una mejora existente",
            Description = "Actualizar los datos de una mejora por su Id")]
        public async Task<IActionResult> Update(int id, [FromBody] EditImprovementCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("El ID en la URL no coincide con el ID del cuerpo de la solicitud.");
            }

            await Mediator.Send(command);
            return Ok(new { message = "Mejora actualizada correctamente" });
        }

        /// <summary>
        /// Obtiene todas las mejoras
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtiene todas las mejoras",
            Description = "Recupera una lista de todas las mejoras con sus detalles")]  
        public async Task<IActionResult> GetAllList()
        {
            var response = await Mediator.Send(new GetAllImprovementWithIncludeQuery());
            if (response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Obtiene una mejora por Id
        /// </summary>
        /// <param name="id">Id de la mejora</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtiene una mejora por Id",
            Description = "Recupera los detalles de una mejora específica utilizando su Id")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetImprovementByIdQuery { Id = id });
            if (response == null)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Elimina una mejora por Id
        /// </summary>
        /// <param name="id">Id de la mejora</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Elimina una mejora por Id",
            Description = "Eliminar una mejora específica utilizando su Id")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await Mediator.Send(new DeleteImprovementCommand { Id = id });
            return NoContent();
        }
    }
}
