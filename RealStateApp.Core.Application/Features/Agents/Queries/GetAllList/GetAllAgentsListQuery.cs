using MediatR;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAllList
{
    /// <summary>
    /// Query para obtener la lista de todos los agentes junto con el conteo de sus propiedades.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/agents
    ///
    /// Respuesta exitosa:
    /// 200 OK
    /// [
    ///   {
    ///     "id": "123e4567-e89b-12d3-a456-426614174000",
    ///     "firstName": "Juan",
    ///     "lastName": "Pérez",
    ///     "email": "juan.perez@example.com",
    ///     "status": "Active",
    ///     "propertiesCount": 5
    ///   },
    ///   {
    ///     "id": "987e6543-e21b-32d3-a456-426614174999",
    ///     "firstName": "Ana",
    ///     "lastName": "Gómez",
    ///     "email": "ana.gomez@example.com",
    ///     "status": "Inactive",
    ///     "propertiesCount": 0
    ///   }
    /// ]
    ///
    /// Respuesta si no hay agentes:
    /// 204 No Content
    /// </remarks>
    public class GetAllAgentsListQuery : IRequest<IList<AgentDto>>
    {
    }

    /// <summary>
    /// Handler para GetAllAgentsListQuery.
    /// </summary>
    public class GetAllAgentsListQueryHandler : IRequestHandler<GetAllAgentsListQuery, IList<AgentDto>>
    {
        private readonly IUserService _userService;
        private readonly IPropertyRepository _propertyRepository;

        public GetAllAgentsListQueryHandler(IUserService userService, IPropertyRepository propertyRepository)
        {
            _userService = userService;
            _propertyRepository = propertyRepository;
        }

        /// <summary>
        /// Ejecuta la query y devuelve todos los agentes con el número de propiedades que tienen.
        /// </summary>
        /// <param name="request">Query sin parámetros</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de AgentDto con información de cada agente y su conteo de propiedades</returns>
        public async Task<IList<AgentDto>> Handle(GetAllAgentsListQuery request, CancellationToken cancellationToken)
        {
            // Obtener un diccionario con el conteo de propiedades por agente
            var dictionary = await _propertyRepository.GetAgentsPropertiesCount();

            if (dictionary == null)
                throw new ApiException("Properties count data not available");

            // Obtener los datos de los agentes (solo los que son agentes)
            var dtos = await _userService.GetUsersAgentOnly(dictionary);

            if (dtos == null)
                throw new ApiException("Agents data not available");

            return dtos;
        }
    }
}
