using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType;
using RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType;
using RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetById;
using Swashbuckle.AspNetCore.Annotations;

namespace RealStateWebApi.Controllers.v1
{
    /// <summary>
    /// Controlador para la gestión de tipos de venta (SaleTypes)
    /// CRUD completo para tipos de venta
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerTag("Gestión de SaleTypes (crear, editar, eliminar, consultar)")]
    [Authorize(Roles = "ADMIN,DEVELOPER")]
    public class SaleTypesController : BaseApiController
    {
        /// <summary>
        /// Crea un nuevo tipo de venta
        /// </summary>
        /// <param name="command">Datos del tipo de venta</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Crea un nuevo tipo de venta",
            Description = "Crea un nuevo tipo de venta y devuelve su Id")]
        public async Task<IActionResult> Create([FromBody] CreateSaleTypeCommand command)
        {
            var response = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response }, response);
        }

        /// <summary>
        /// Actualiza un tipo de venta existente
        /// </summary>
        /// <param name="id">Id del tipo de venta</param>
        /// <param name="command">Datos actualizados</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Actualiza un tipo de venta existente",
            Description = "Actualiza un tipo de venta existente con los datos proporcionados")]
        public async Task<IActionResult> Update(int id, [FromBody] EditSaleTypeCommand command)
        {
            if (id != command.Id)
                return BadRequest("El ID en la URL no coincide con el ID del cuerpo de la solicitud.");

            await Mediator.Send(command);
            return Ok(new { message = "Tipo de venta actualizado correctamente" });
        }

        /// <summary>
        /// Obtiene todos los tipos de venta
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtiene todos los tipos de venta",
            Description = "Devuelve una lista de todos los tipos de venta disponibles")]
        public async Task<IActionResult> GetAllList()
        {
            var response = await Mediator.Send(new GetAllSaleTypeWithIncludeQuery());
            if (response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un tipo de venta por Id
        /// </summary>
        /// <param name="id">Id del tipo de venta</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Obtiene un tipo de venta por Id",
            Description = "Devuelve los detalles de un tipo de venta específico según su Id")]  
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetSaleTypeByIdQuery { Id = id });
            if (response == null)
                return NoContent();

            return Ok(response);
        }

        /// <summary>
        /// Elimina un tipo de venta
        /// </summary>
        /// <param name="id">Id del tipo de venta</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Elimina un tipo de venta",
            Description = "Elimina un tipo de venta específico según su Id")]       
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await Mediator.Send(new DeleteSaleTypeCommand { Id = id });
            return NoContent();
        }
    }
}
