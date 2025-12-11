using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties
{
    /// <summary>
    /// Query para obtener todas las propiedades de un agente específico.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/agents/{agentId}/properties
    ///
    /// Respuesta exitosa:
    /// 200 OK
    /// [
    ///   {
    ///     "id": 1,
    ///     "code": "PROP123",
    ///     "title": "Apartamento en zona céntrica",
    ///     "description": "Amplio y luminoso",
    ///     "agentId": "123e4567-e89b-12d3-a456-426614174000",
    ///     "agentName": "Juan Pérez",
    ///     ...
    ///   }
    /// ]
    ///
    /// Respuesta si no tiene propiedades:
    /// 204 No Content
    /// </remarks>
    public class GetAgentPropertiesQuery : IRequest<IList<PropertyApiDto>>
    {
        /// <summary>
        /// Id del agente
        /// </summary>
        /// <example>"123e4567-e89b-12d3-a456-426614174000"</example>
        public required string AgentId { get; set; }
    }

    /// <summary>
    /// Handler para GetAgentPropertiesQuery.
    /// </summary>
    public class GetAgentPropertiesQueryHandler : IRequestHandler<GetAgentPropertiesQuery, IList<PropertyApiDto>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetAgentPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IMapper mapper,
            IUserService userService)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Ejecuta la query y retorna la lista de propiedades del agente con el nombre completo del agente.
        /// </summary>
        /// <param name="request">Query con Id del agente</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de PropertyApiDto</returns>
        public async Task<IList<PropertyApiDto>> Handle(GetAgentPropertiesQuery request, CancellationToken cancellationToken)
        {
            // Obtener propiedades del agente
            var properties = await _propertyRepository.GetListByAgentId(request.AgentId);

            var propertyDtos = new List<PropertyApiDto>();

            foreach (var property in properties)
            {
                var dto = _mapper.Map<PropertyApiDto>(property);

                // Obtener nombre del agente
                var agent = await _userService.GetById(property.AgentId);
                dto.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";

                propertyDtos.Add(dto);
            }

            return propertyDtos;
        }
    }
}
