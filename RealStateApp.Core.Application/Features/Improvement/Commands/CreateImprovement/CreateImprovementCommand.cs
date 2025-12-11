using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement
{
    /// <summary>
    /// Comando para crear una nueva mejora (Improvement) de propiedad.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// 
    /// POST /api/v1/improvements
    /// {
    ///    "name": "Cámaras de seguridad",
    ///    "description": "Sistema de vigilancia con cámaras en todas las entradas"
    /// }
    /// </remarks>
    public class CreateImprovementCommand : IRequest<int>
    {
        /// <summary>
        /// Nombre de la mejora.
        /// </summary>
        /// <example>Cámaras de seguridad</example>
        [SwaggerParameter(Description = "Nombre de la mejora")]
        public  string? Name { get; set; }

        /// <summary>
        /// Descripción detallada de la mejora.
        /// </summary>
        /// <example>Sistema de vigilancia con cámaras en todas las entradas</example>
        [SwaggerParameter(Description = "Descripción de la mejora")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Handler encargado de procesar la creación de una mejora.
    /// </summary>
    public class CreateImprovementCommandHandler : IRequestHandler<CreateImprovementCommand, int>
    {
        private readonly IImprovementRepository _improvementRepository;

        /// <summary>
        /// Constructor del handler.
        /// </summary>
        /// <param name="improvementRepository">Repositorio para manejar las mejoras en la base de datos.</param>
        public CreateImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        /// <summary>
        /// Ejecuta la lógica para crear una mejora.
        /// </summary>
        /// <param name="request">Comando con los datos de la mejora.</param>
        /// <param name="cancellationToken">Token para cancelación.</param>
        /// <returns>Id de la mejora creada.</returns>
        /// <exception cref="ApiException">Si ocurre un error al crear la mejora.</exception>
        public async Task<int> Handle(CreateImprovementCommand request, CancellationToken cancellationToken)
        {
            // Crear entidad a partir del comando
            var entity = new RealStateApp.Core.Domain.Entities.Improvement
            {
                Id = 0, // El Id se asigna al guardar en la base de datos
                Name = request.Name,
                Description = request.Description
            };

            // Guardar en la base de datos
            entity = await _improvementRepository.AddAsync(entity);

            // Verificar si se creó correctamente
            if (entity == null)
                throw new ApiException("Error creating entity", (int)HttpStatusCode.InternalServerError);

            // Retornar Id de la mejora creada
            return entity.Id;
        }
    }
}
