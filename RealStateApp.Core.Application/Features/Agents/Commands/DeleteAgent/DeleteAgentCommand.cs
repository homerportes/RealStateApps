using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System.Net;

namespace RealStateApp.Core.Application.Features.Agents.Commands.DeleteAgent
{
    /// <summary>
    /// Comando para eliminar un agente y todas sus propiedades asociadas.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// DELETE /api/v1/agents/{id}
    ///
    /// Respuesta exitosa:
    /// 204 No Content
    ///
    /// Respuesta si el agente no existe:
    /// 404 Not Found
    /// </remarks>
    public class DeleteAgentCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id del agente a eliminar.
        /// </summary>
        /// <example>"123e4567-e89b-12d3-a456-426614174000"</example>
        public string? Id { get; set; }
    }

    /// <summary>
    /// Handler para DeleteAgentCommand.
    /// </summary>
    public class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand, Unit>
    {
        private readonly IUserService _userService;
        private readonly IPropertyRepository _propertyRepository;

        public DeleteAgentCommandHandler(IUserService userService, IPropertyRepository propertyRepository)
        {
            _userService = userService;
            _propertyRepository = propertyRepository;
        }

        /// <summary>
        /// Ejecuta la eliminación del agente y sus propiedades asociadas.
        /// </summary>
        /// <param name="request">Comando con Id del agente</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit</returns>
        /// <exception cref="ApiException">Si el agente no existe o ocurre un error al eliminarlo</exception>
        public async Task<Unit> Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
        {
            // Verificar que el agente existe
            var agent = await _userService.GetById(request.Id ?? "");
            if (agent == null)
                throw new ApiException("Agent not found", (int)HttpStatusCode.NotFound);

            // Eliminar todas las propiedades del agente
            await _propertyRepository.DeleteAgentProperties(request.Id ?? "");

            // Eliminar el agente
            var result = await _userService.DeleteAsync(request.Id ?? "");

            if (result.HasError)
                throw new ApiException("Error deleting agent", (int)HttpStatusCode.InternalServerError);

            return Unit.Value;
        }
    }
}
