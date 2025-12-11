using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.EditProperty
{
    /// <summary>
    /// Comando para editar una propiedad existente
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// PUT /api/v1/properties/1
    /// {
    ///   "id": 1,
    ///   "price": 250000,
    ///   "sizeInMeters": 120.5,
    ///   "bedrooms": 3,
    ///   "bathrooms": 2,
    ///   "description": "Hermosa propiedad renovada",
    ///   "propertyTypeId": 2,
    ///   "saleTypeId": 1,
    ///   "status": 0,
    ///   "improvementIds": [1, 3, 5]
    /// }
    /// 
    /// Respuesta exitosa:
    /// 204 No Content
    /// 
    /// Errores posibles:
    /// 404 Not Found - si la propiedad no existe
    /// 400 Bad Request - si el tipo de propiedad o tipo de venta no existen
    /// </remarks>
    public class EditPropertyCommand : IRequest<Unit>
    {
        [SwaggerParameter(Description = "ID de la propiedad")]
        public int Id { get; set; }

        [SwaggerParameter(Description = "Precio de la propiedad")]
        public decimal Price { get; set; }

        [SwaggerParameter(Description = "Tamaño en metros cuadrados")]
        public double SizeInMeters { get; set; }

        [SwaggerParameter(Description = "Número de habitaciones")]
        public int Bedrooms { get; set; }

        [SwaggerParameter(Description = "Número de baños")]
        public int Bathrooms { get; set; }

        [SwaggerParameter(Description = "Descripción de la propiedad")]
        public required string Description { get; set; }

        [SwaggerParameter(Description = "ID del tipo de propiedad")]
        public int PropertyTypeId { get; set; }

        [SwaggerParameter(Description = "ID del tipo de venta")]
        public int SaleTypeId { get; set; }

        [SwaggerParameter(Description = "Estado de la propiedad")]
        public int Status { get; set; }

        [SwaggerParameter(Description = "Lista de IDs de mejoras")]
        public List<int> ImprovementIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Handler para actualizar los datos de la propiedad
    /// </summary>
    public class EditPropertyCommandHandler : IRequestHandler<EditPropertyCommand, Unit>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IPropertyImprovementRepository _propertyImprovementRepository;

        public EditPropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository,
            IPropertyImprovementRepository propertyImprovementRepository)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
            _propertyImprovementRepository = propertyImprovementRepository;
        }

        public async Task<Unit> Handle(EditPropertyCommand request, CancellationToken cancellationToken)
        {
            // Validar existencia de la propiedad
            var existingProperty = await _propertyRepository.GetByIdAsync(request.Id);
            if (existingProperty == null)
                throw new ApiException("La propiedad no existe con ese ID", (int)HttpStatusCode.NotFound);

            // Validar PropertyType
            var propertyType = await _propertyTypeRepository.GetByIdAsync(request.PropertyTypeId);
            if (propertyType == null)
                throw new ApiException("El tipo de propiedad no existe", (int)HttpStatusCode.BadRequest);

            // Validar SaleType
            var saleType = await _saleTypeRepository.GetByIdAsync(request.SaleTypeId);
            if (saleType == null)
                throw new ApiException("El tipo de venta no existe", (int)HttpStatusCode.BadRequest);

            // Actualizar campos de la propiedad
            existingProperty.Price = request.Price;
            existingProperty.SizeInMeters = request.SizeInMeters;
            existingProperty.Bedrooms = request.Bedrooms;
            existingProperty.Bathrooms = request.Bathrooms;
            existingProperty.Description = request.Description;
            existingProperty.PropertyTypeId = request.PropertyTypeId;
            existingProperty.SaleTypeId = request.SaleTypeId;
            existingProperty.Status = (Domain.Common.Enums.PropertyStatus)request.Status;

            // Actualizar propiedad
            await _propertyRepository.UpdateAsync(existingProperty.Id, existingProperty);

            // Actualizar mejoras: primero eliminar todas
            await _propertyImprovementRepository.DeleteByPropertyId(existingProperty.Id);

            // Agregar nuevas mejoras
            foreach (var improvementId in request.ImprovementIds)
            {
                var propertyImprovement = new Domain.Entities.PropertyImprovement
                {
                    PropertyId = existingProperty.Id,
                    ImprovementId = improvementId
                };
                await _propertyImprovementRepository.AddAsync(propertyImprovement);
            }

            return Unit.Value;
        }
    }
}
