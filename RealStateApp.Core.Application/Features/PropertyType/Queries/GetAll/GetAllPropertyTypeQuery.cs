using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetAll
{
    /// <summary>
    /// Query para obtener todos los PropertyTypes disponibles
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/propertytypes
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// [
    ///    {
    ///       "id": 1,
    ///       "name": "Oficina",
    ///       "description": "Espacio para oficinas comerciales"
    ///    },
    ///    {
    ///       "id": 2,
    ///       "name": "Apartamento",
    ///       "description": "Vivienda residencial"
    ///    }
    /// ]
    /// 
    /// Respuesta si no hay PropertyTypes:
    /// 204 No Content
    /// </remarks>
    public class GetAllPropertyTypeQuery : IRequest<IList<PropertyTypeDto>>
    {
    }

    /// <summary>
    /// Handler para GetAllPropertyTypeQuery
    /// </summary>
    public class GetAllPropertyTypeQueryHandler : IRequestHandler<GetAllPropertyTypeQuery, IList<PropertyTypeDto>>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPropertyTypeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la query para obtener todos los PropertyTypes
        /// </summary>
        /// <param name="request">Query sin parámetros</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de PropertyTypeDto</returns>
        public async Task<IList<PropertyTypeDto>> Handle(GetAllPropertyTypeQuery request, CancellationToken cancellationToken)
        {
            // Obtenemos la query completa
            var listEntitiesQuery = _repository.GetAllQuery();

            // Convertimos a DTOs usando AutoMapper
            var listEntityDtos = await listEntitiesQuery
                .ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return listEntityDtos;
        }
    }
}
