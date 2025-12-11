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

namespace RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement
{
    /// <summary>
    /// Comando para eliminar una mejora (Improvement) de propiedad por su Id.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// 
    /// DELETE /api/v1/improvements/5
    /// 
    /// Donde 5 es el Id de la mejora que se desea eliminar.
    /// </remarks>
    public class DeleteImprovementCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id de la mejora que se desea eliminar.
        /// </summary>
        /// <example>5</example>
        [SwaggerParameter(Description = "Id de la mejora que se desea eliminar")]
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler encargado de procesar la eliminación de una mejora.
    /// </summary>
    public class DeleteImprovementCommandHandler : IRequestHandler<DeleteImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        /// <summary>
        /// Constructor del handler.
        /// </summary>
        /// <param name="improvementRepository">Repositorio de mejoras para acceder a la base de datos.</param>
        public DeleteImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        /// <summary>
        /// Ejecuta la lógica para eliminar una mejora.
        /// </summary>
        /// <param name="request">Comando con el Id de la mejora a eliminar.</param>
        /// <param name="cancellationToken">Token para cancelación.</param>
        /// <returns>Unit si la eliminación fue exitosa.</returns>
        /// <exception cref="ApiException">Si la mejora no se encuentra.</exception>
        public async Task<Unit> Handle(DeleteImprovementCommand request, CancellationToken cancellationToken)
        {
            // Buscar entidad por Id
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ApiException("Entity  not found with this id",(int)HttpStatusCode.NotFound);

               
            // Eliminar la entidad
            await _improvementRepository.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
