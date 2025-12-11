using AutoMapper;
using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Delete
{
    /// <summary>
    /// Comando para eliminar un PropertyType existente por su Id
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// DELETE /api/v1/propertytypes/1
    /// 
    /// Respuesta exitosa:
    /// 204 No Content
    /// 
    /// Respuesta si no se encuentra la entidad:
    /// 404 Not Found
    /// {
    ///    "message": "Entity not found with this id"
    /// }
    /// </remarks>
    public class DeletePropertyTypeCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id del PropertyType a eliminar
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler del comando DeletePropertyTypeCommand
    /// </summary>
    public class DeletePropertyTypeCommandHandler : IRequestHandler<DeletePropertyTypeCommand, Unit>
    {
        private readonly IPropertyTypeRepository _repository;

        public DeletePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ejecuta la eliminación del PropertyType
        /// </summary>
        /// <param name="request">Id del PropertyType a eliminar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit (sin contenido)</returns>
        /// <exception cref="ApiException">Si no se encuentra la entidad</exception>
        public async Task<Unit> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new ApiException("Entity not found with this id");

            await _repository.DeleteAsync(request.Id);
            return Unit.Value;
        }
    }
}
