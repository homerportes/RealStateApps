using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType
{
    /// <summary>
    /// Comando para actualizar un SaleType existente
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// PUT /api/v1/saletypes/7
    /// 
    /// Body:
    /// {
    ///   "id": 7,
    ///   "name": "Venta Premium",
    ///   "description": "Tipo de venta con beneficios adicionales"
    /// }
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// {
    ///   "message": "SaleType actualizado exitosamente"
    /// }
    /// 
    /// Respuesta si el Id no existe:
    /// 404 Not Found
    /// {
    ///   "message": "Entity not found with Id"
    /// }
    /// </remarks>
    public class EditSaleTypeCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id del SaleType a editar
        /// </summary>
        /// <example>7</example>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del SaleType
        /// </summary>
        /// <example>Venta Premium</example>
        public required string Name { get; set; }

        /// <summary>
        /// Descripción del SaleType
        /// </summary>
        /// <example>Tipo de venta con beneficios adicionales</example>
        public required string Description { get; set; }
    }

    /// <summary>
    /// Handler encargado de actualizar un SaleType
    /// </summary>
    public class EditSaleTypeCommandHandler : IRequestHandler<EditSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _repository;

        public EditSaleTypeCommandHandler(ISaleTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ejecuta el comando para actualizar un SaleType
        /// </summary>
        /// <param name="request">Comando con los datos actualizados</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit</returns>
        /// <exception cref="ApiException">Si no se encuentra la entidad</exception>
        public async Task<Unit> Handle(EditSaleTypeCommand request, CancellationToken cancellationToken)
        {
            // Buscar la entidad
            Domain.Entities.SaleType? getEntity = await _repository.GetByIdAsync(request.Id);

            if (getEntity == null)
                throw new ApiException("Entity not found with Id");

            // Actualizar la entidad
            Domain.Entities.SaleType entity = new()
            {
                Description = request.Description,
                Name = request.Name,
                Id = request.Id
            };

            await _repository.UpdateAsync(request.Id, entity);
            return Unit.Value;
        }
    }
}
