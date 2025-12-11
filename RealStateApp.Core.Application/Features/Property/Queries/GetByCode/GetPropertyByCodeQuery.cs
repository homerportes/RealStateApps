using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Domain.Interfaces;
using System.Net;

namespace RealStateApp.Core.Application.Features.Property.Queries.GetByCode
{
    /// <summary>
    /// Query para obtener una propiedad por su Código
    /// Endpoint: GET /api/v1/properties/code/{code}
    /// Roles: Administrador, Desarrollador
    /// </summary>
    public class GetPropertyByCodeQuery : IRequest<PropertyApiDto>
    {
        public required string Code { get; set; }
    }

    public class GetPropertyByCodeQueryHandler : IRequestHandler<GetPropertyByCodeQuery, PropertyApiDto>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetPropertyByCodeQueryHandler(
            IPropertyRepository propertyRepository, 
            IMapper mapper,
            IUserService userService)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<PropertyApiDto> Handle(GetPropertyByCodeQuery request, CancellationToken cancellationToken)
        {
            var propertiesQuery = _propertyRepository.GetAllQueryWithInclude(new List<string>
            {
                "PropertyType",
                "SaleType",
                "PropertyImprovements.Improvement"
            });

            var property = await propertiesQuery
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (property == null)
                throw new ApiException("El código de la propiedad es inválido",(int)HttpStatusCode.NotFound);

            var dto = _mapper.Map<PropertyApiDto>(property);
            
            // Obtener el nombre del agente usando UserService
            var agent = await _userService.GetById(property.AgentId);
            dto.AgentName = agent != null ? $"{agent.FirstName} {agent.LastName}" : "Unknown";

            return dto;
        }
    }
}
