using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Create
{
    /// <summary>
    /// Comando para crear un nuevo PropertyType
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// POST /api/v1/propertytypes
    /// {
    ///    "name": "Casa",
    ///    "description": "Propiedad residencial"
    /// }
    /// 
    /// Respuesta exitosa:
    /// 201 Created
    /// {
    ///   "id": 1
    /// }
    /// 
    /// Respuesta si ocurre error:
    /// 500 Internal Server Error
    /// {
    ///   "message": "Error creating Property Type"
    /// }
    /// </remarks>
    public class CreatePropertyTypeCommand : IRequest<int>
    {
        /// <summary>
        /// Nombre del tipo de propiedad
        /// </summary>
        /// <example>Casa</example>
        public required string Name { get; set; }

        /// <summary>
        /// Descripción del tipo de propiedad
        /// </summary>
        /// <example>Propiedad residencial</example>
        public required string Description { get; set; }
    }

    /// <summary>
    /// Handler del comando CreatePropertyTypeCommand
    /// </summary>
    public class CreatePropertyTypeCommandHandler : IRequestHandler<CreatePropertyTypeCommand, int>
    {
        private readonly IPropertyTypeRepository _repository;

        public CreatePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ejecuta la creación de un PropertyType
        /// </summary>
        /// <param name="request">Datos del PropertyType</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Id del PropertyType creado</returns>
        /// <exception cref="ApiException">Si ocurre un error al crear la entidad</exception>
        public async Task<int> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.AddAsync(new Domain.Entities.PropertyType
            {
                Description = request.Description,
                Name = request.Name,
                Id = 0
            });

            if (entity == null)
                throw new ApiException("Error creating Property Type", HttpStatusCode.InternalServerError);

            return entity.Id;
        }
    }
}
