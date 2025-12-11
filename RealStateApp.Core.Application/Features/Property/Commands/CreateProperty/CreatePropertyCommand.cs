using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Commands.CreateProperty
{
    /// <summary>
    /// Comando para crear una propiedad nueva.
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// POST /api/v1/properties
    /// {
    ///   "price": 250000,
    ///   "sizeInMeters": 120,
    ///   "bedrooms": 3,
    ///   "bathrooms": 2,
    ///   "description": "Hermosa casa con patio amplio",
    ///   "propertyTypeId": 1,
    ///   "saleTypeId": 2,
    ///   "agentId": "123e4567-e89b-12d3-a456-426614174000",
    ///   "improvementIds": [1, 3]
    /// }
    ///
    /// Respuesta exitosa:
    /// 201 Created
    /// {
    ///   "id": 101
    /// }
    ///
    /// Respuesta si falla:
    /// 400 Bad Request
    /// {
    ///   "message": "El tipo de propiedad no existe"
    /// }
    /// </remarks>
    public class CreatePropertyCommand : IRequest<int>
    {
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

        [SwaggerParameter(Description = "ID del agente que publica la propiedad")]
        public required string AgentId { get; set; }

        [SwaggerParameter(Description = "Lista de IDs de mejoras aplicables")]
        public List<int> ImprovementIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Handler para crear una propiedad.
    /// </summary>
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISaleTypeRepository _saleTypeRepository;

        public CreatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISaleTypeRepository saleTypeRepository)
        {
            _propertyRepository = propertyRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            // Validar existencia de PropertyType
            var propertyType = await _propertyTypeRepository.GetByIdAsync(request.PropertyTypeId);
            if (propertyType == null)
                throw new ApiException("El tipo de propiedad no existe", (int)HttpStatusCode.BadRequest);

            // Validar existencia de SaleType
            var saleType = await _saleTypeRepository.GetByIdAsync(request.SaleTypeId);
            if (saleType == null)
                throw new ApiException("El tipo de venta no existe", (int)HttpStatusCode.BadRequest);

            // Generar código único
            string code = await GenerateUniqueCodeAsync();

            Domain.Entities.Property entity = new()
            {
                Id = 0,
                Code = code,
                Price = request.Price,
                SizeInMeters = request.SizeInMeters,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                Description = request.Description,
                PropertyTypeId = request.PropertyTypeId,
                SaleTypeId = request.SaleTypeId,
                AgentId = request.AgentId,
                Status = Domain.Common.Enums.PropertyStatus.Available
            };

            entity = await _propertyRepository.AddAsync(entity);

            if (entity == null)
                throw new ApiException("Error creating property", (int)HttpStatusCode.InternalServerError);

            // Agregar mejoras si se especifican
            if (request.ImprovementIds.Any())
            {
                foreach (var improvementId in request.ImprovementIds)
                {
                    var propertyImprovement = new PropertyImprovement
                    {
                        PropertyId = entity.Id,
                        ImprovementId = improvementId
                    };
                    entity.PropertyImprovements.Add(propertyImprovement);
                }
                await _propertyRepository.UpdateAsync(entity.Id, entity);
            }

            return entity.Id;
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;

            do
            {
                code = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var existingProperty = await _propertyRepository.GetByCode(code);
                if (existingProperty == null)
                    break;

            } while (true);

            return code;
        }
    }
}
