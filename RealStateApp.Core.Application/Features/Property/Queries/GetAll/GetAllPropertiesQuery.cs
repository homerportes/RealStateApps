using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;

namespace RealStateApp.Core.Application.Features.Property.Queries.GetAll
{
    /// <summary>
    /// Query para obtener todas las propiedades
    /// Endpoint: GET /api/v1/properties
    /// Roles: Administrador, Desarrollador
    /// </summary>
    public class GetAllPropertiesQuery : IRequest<IList<PropertyApiDto>>
    {
    }

    public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, IList<PropertyApiDto>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetAllPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IMapper mapper,
            IUserService userService)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IList<PropertyApiDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            // Obtener todas las propiedades incluyendo sus relaciones
            var propertiesQuery = _propertyRepository.GetAllQueryWithInclude(new List<string>
            {
                "PropertyType",
                "SaleType",
                "PropertyImprovements.Improvement"
            });

            var properties = await propertiesQuery.ToListAsync(cancellationToken);
            var propertyDtos = new List<PropertyApiDto>();

            foreach (var property in properties)
            {
                // Mapear la entidad Property a DTO
                var dto = _mapper.Map<PropertyApiDto>(property);

                // Obtener nombre del agente para cada propiedad
                var agent = await _userService.GetById(property.AgentId);
                dto.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";

                propertyDtos.Add(dto);
            }

            return propertyDtos;
        }
    }
}
