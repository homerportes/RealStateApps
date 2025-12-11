using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType
{
    /// <summary>
    /// Comando para eliminar un SaleType existente
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// DELETE /api/v1/saletypes/7
    /// 
    /// Respuesta exitosa:
    /// 204 No Content
    /// 
    /// Respuesta si el Id no existe:
    /// 404 Not Found
    /// {
    ///   "message": "Entity not found with this id"
    /// }
    /// </remarks>
    public class DeleteSaleTypeCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id del SaleType a eliminar
        /// </summary>
        /// <example>7</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler encargado de eliminar un SaleType
    /// </summary>
    public class DeleteSaleTypeCommandHandler : IRequestHandler<DeleteSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _repository;

        /// <summary>
        /// Constructor del handler
        /// </summary>
        /// <param name="repository">Repositorio de SaleType</param>
        public DeleteSaleTypeCommandHandler(ISaleTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ejecuta el comando para eliminar un SaleType
        /// </summary>
        /// <param name="request">Comando con el Id a eliminar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit</returns>
        /// <exception cref="ApiException">Si no se encuentra la entidad</exception>
        public async Task<Unit> Handle(DeleteSaleTypeCommand request, CancellationToken cancellationToken)
        {
            // Buscar la entidad
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ApiException("Entity not found with this id");

            // Eliminar la entidad
            await _repository.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
