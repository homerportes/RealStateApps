using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus
{
    /// <summary>
    /// Comando para cambiar el estado (activo/inactivo) de un agente.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// PATCH /api/v1/agents/{id}/status
    ///
    /// Request body:
    /// {
    ///   "status": true
    /// }
    ///
    /// Respuesta exitosa:
    /// 204 No Content
    ///
    /// Respuesta si el agente no existe:
    /// 404 Not Found
    /// </remarks>
    public class ChangeAgentStatusCommand : IRequest<Unit>
    {
        /// <summary>
        /// Id del agente cuyo estado se quiere cambiar.
        /// </summary>
        /// <example>"123e4567-e89b-12d3-a456-426614174000"</example>
        public string? Id { get; set; }

        /// <summary>
        /// Nuevo estado del agente (true = activo, false = inactivo)
        /// </summary>
        /// <example>true</example>
        public bool Status { get; set; }
    }

    /// <summary>
    /// Handler para ChangeAgentStatusCommand
    /// </summary>
    public class ChangeAgentStatusCommandHandler : IRequestHandler<ChangeAgentStatusCommand, Unit>
    {
        private readonly IUserService _userService;

        public ChangeAgentStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Ejecuta el cambio de estado del agente.
        /// </summary>
        /// <param name="request">Comando con Id y nuevo estado</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Unit</returns>
        /// <exception cref="ApiException">Si el agente no existe</exception>
        public async Task<Unit> Handle(ChangeAgentStatusCommand request, CancellationToken cancellationToken)
        {
            var operationStatus = await _userService.SetStatus(request.Id, request.Status);

            if (!operationStatus)
                throw new ApiException("Entity Not found", HttpStatusCode.NotFound);


            if (operationStatus == false) throw new ApiException("Entity Not found", (int)HttpStatusCode.NotFound);

            return Unit.Value;
        }
    }
}
