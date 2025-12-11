using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading;

using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement
{
    /// <summary>
    /// Comando para editar una mejora (Improvement) de propiedad.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// 
    /// PUT /api/v1/improvements/5
    /// {
    ///     "name": "Cámaras de seguridad actualizadas",
    ///     "description": "Nueva descripción de la mejora"
    /// }
    /// 
    /// Donde 5 es el Id de la mejora que se desea editar.
    /// </remarks>
    public class EditImprovementCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id de la mejora que se desea editar.
        /// </summary>
        /// <example>5</example>
        [SwaggerParameter(Description = "Id de la mejora a editar")]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la mejora.
        /// </summary>
        /// <example>Cámaras de seguridad</example>
        [SwaggerParameter(Description = "Nombre actualizado de la mejora")]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción de la mejora.
        /// </summary>
        /// <example>Se instalan cámaras de última generación</example>
        [SwaggerParameter(Description = "Descripción actualizada de la mejora")]
        public required string Description { get; set; }
    }

    /// <summary>
    /// Handler encargado de procesar la edición de una mejora.
    /// </summary>
    public class EditImprovementCommandHandler : IRequestHandler<EditImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        /// <summary>
        /// Constructor del handler.
        /// </summary>
        /// <param name="improvementRepository">Repositorio de mejoras para acceder a la base de datos.</param>
        public EditImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        /// <summary>
        /// Ejecuta la lógica para actualizar una mejora.
        /// </summary>
        /// <param name="request">Comando con los datos actualizados de la mejora.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Unit si la actualización fue exitosa.</returns>
        /// <exception cref="ApiException">Si la mejora no existe.</exception>
        public async Task<Unit> Handle(EditImprovementCommand request, CancellationToken cancellationToken)
        {
            // Buscar entidad existente por Id
            var getEntity = await _improvementRepository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ApiException("Entity not found with Id",(int)HttpStatusCode.NotFound);



            // Crear la entidad actualizada
            var entity = new Domain.Entities.Improvement
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };

            // Actualizar en la base de datos
            await _improvementRepository.UpdateAsync(request.Id, entity);

            return Unit.Value;
        }
    }
}
