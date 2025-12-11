using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.DeleteProperty
{
    /// <summary>
    /// Comando para eliminar una propiedad por su código único.
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// DELETE /api/v1/properties
    /// {
    ///   "code": "AB12CD"
    /// }
    ///
    /// Respuesta exitosa:
    /// 204 No Content
    ///
    /// Respuesta si no existe la propiedad:
    /// 404 Not Found
    /// {
    ///   "message": "La propiedad no existe con ese código"
    /// }
    /// </remarks>
    public class DeletePropertyCommand : IRequest<Unit>
    {
        [SwaggerParameter(Description = "Código único de 6 caracteres de la propiedad")]
        public required string Code { get; set; }
    }

    /// <summary>
    /// Handler para eliminar la propiedad.
    /// </summary>
    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Unit>
    {
        private readonly IPropertyRepository _propertyRepository;

        public DeletePropertyCommandHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<Unit> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            // Buscar la propiedad por código
            var existingProperty = await _propertyRepository.GetByCode(request.Code);

            if (existingProperty == null)
                throw new ApiException("La propiedad no existe con ese código", (int)HttpStatusCode.NotFound);

            // Eliminar propiedad
            await _propertyRepository.DeleteAsync(existingProperty.Id);

            return Unit.Value;
        }
    }
}
