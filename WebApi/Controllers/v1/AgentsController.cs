using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealStateApp.Core.Application.Features.Agents.Commands.DeleteAgent;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties;
using RealStateApp.Core.Application.Features.Agents.Queries.GetAllList;
using RealStateApp.Core.Application.Features.Agents.Queries.GetById;
using Swashbuckle.AspNetCore.Annotations;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// Controlador para la gestión de agentes
    /// CRUD y gestión de propiedades asociadas a agentes
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerTag("Agentes CRUD y gestión de propiedades")]
    public class AgentsController : BaseApiController
    {
        /// <summary>
        /// Obtiene la lista completa de agentes
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var response = await Mediator.Send(new GetAllAgentsListQuery());

            if (response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un agente por su Id
        /// </summary>
        /// <param name="id">Id del agente</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var response = await Mediator.Send(new GetAgentByIdQuery { Id = id });

            if (response == null)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Obtiene las propiedades asignadas a un agente
        /// </summary>
        /// <param name="id">Id del agente</param>
        [HttpGet("{id}/properties")]
        [Authorize(Roles = "ADMIN,DEVELOPER")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAgentProperty([FromRoute] string id)
        {
            var response = await Mediator.Send(new GetAgentPropertiesQuery { AgentId = id });

            if (response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Cambia el estado de un agente (activo/inactivo)
        /// </summary>
        /// <param name="id">Id del agente</param>
        /// <param name="command">Objeto con el nuevo estado</param>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus([FromRoute] string id, [FromBody] ChangeAgentStatusCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Elimina un agente por Id
        /// </summary>
        /// <param name="id">Id del agente</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            await Mediator.Send(new DeleteAgentCommand { Id = id });
            return NoContent();
        }
    }
}
