using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType
{
    /// <summary>
    /// Comando para crear un nuevo tipo de venta (SaleType)
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// POST /api/v1/saletypes
    /// 
    /// Cuerpo de la solicitud:
    /// {
    ///   "name": "Alquiler",
    ///   "description": "Propiedades disponibles para alquilar"
    /// }
    /// 
    /// Respuesta exitosa:
    /// 201 Created
    /// {
    ///   "id": 7
    /// }
    /// </remarks>
    public class CreateSaleTypeCommand : IRequest<int>
    {
        /// <summary>
        /// Nombre del tipo de venta
        /// </summary>
        /// <example>Alquiler</example>
        [SwaggerParameter(Description = "Nombre del tipo de venta")]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción del tipo de venta
        /// </summary>
        /// <example>Propiedades disponibles para alquilar</example>
        [SwaggerParameter(Description = "Descripción del tipo de venta")]
        public required string Description { get; set; }
    }

    /// <summary>
    /// Handler encargado de crear un nuevo SaleType
    /// </summary>
    public class CreateSaleTypeCommandHandler : IRequestHandler<CreateSaleTypeCommand, int>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        /// <summary>
        /// Constructor del handler
        /// </summary>
        /// <param name="saleTypeRepository">Repositorio de tipos de venta</param>
        public CreateSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        /// <summary>
        /// Ejecuta el comando para crear un nuevo SaleType
        /// </summary>
        /// <param name="request">Comando con los datos del nuevo SaleType</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Id del nuevo SaleType</returns>
        /// <exception cref="ApiException">Si ocurre un error al crear el tipo de venta</exception>
        public async Task<int> Handle(CreateSaleTypeCommand request, CancellationToken cancellationToken)
        {

            // Crear la entidad
            Domain.Entities.SaleType entity = new()
            {
                Id = 0, // El Id será generado por la base de datos
                Name = request.Name,
                Description = request.Description
            };

            // Guardar en el repositorio
            entity = await _saleTypeRepository.AddAsync(entity);

            // Validar si se creó correctamente
            if (entity == null)
                throw new ApiException("Error creating sale type", (int)HttpStatusCode.InternalServerError);

            return entity.Id;
        }
    }
}
