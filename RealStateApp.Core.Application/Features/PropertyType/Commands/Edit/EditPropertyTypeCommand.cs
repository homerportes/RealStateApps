using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net;
using System.Threading;

using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Edit
{
    /// <summary>
    /// Comando para editar un PropertyType existente
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// PUT /api/v1/propertytypes/1
    /// {
    ///    "id": 1,
    ///    "name": "Nuevo nombre",
    ///    "description": "Nueva descripción"
    /// }
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// 
    /// Respuesta si no se encuentra la entidad:
    /// 404 Not Found
    /// {
    ///    "message": "Property type not found with this id"
    /// }
    /// </remarks>
    public class EditPropertyTypeCommand : IRequest<Unit>
    {
        /// <summary>
        /// Nombre actualizado del PropertyType
        /// </summary>
        /// <example>Oficina</example>
        public required string Name { get; set; }

        /// <summary>
        /// Descripción actualizada del PropertyType
        /// </summary>
        /// <example>Espacio para oficinas comerciales</example>
        public required string Description { get; set; }

        /// <summary>
        /// Id del PropertyType a editar
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler del comando EditPropertyTypeCommand
    /// </summary>
    public class EditPropertyTypeCommandHandler : IRequestHandler<EditPropertyTypeCommand, Unit>
    {
        private readonly IPropertyTypeRepository _repository;

        public EditPropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ejecuta la edición del PropertyType
        /// </summary>
        /// <param name="request">Datos del PropertyType a editar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit (sin contenido)</returns>
        /// <exception cref="ApiException">Si no se encuentra el PropertyType</exception>
        public async Task<Unit> Handle(EditPropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var getEntity = await _repository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ApiException("Property type not found with this id",(int)HttpStatusCode.NotFound);

              


            var entity = new Domain.Entities.PropertyType
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };

            await _repository.UpdateAsync(request.Id, entity);

            return Unit.Value;
        }
    }
}
