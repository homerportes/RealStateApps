using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetAllWithInclude
{
    /// <summary>
    /// Query para obtener todos los PropertyTypes junto con sus propiedades relacionadas
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/propertytypes/withinclude
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// [
    ///    {
    ///       "id": 1,
    ///       "name": "Oficina",
    ///       "description": "Espacio para oficinas comerciales",
    ///       "properties": [
    ///           {
    ///               "id": 101,
    ///               "name": "Oficina Central",
    ///               "code": "OFC123",
    ///               "saleTypeId": 1,
    ///               "price": 500000
    ///           }
    ///       ]
    ///    }
    /// ]
    /// 
    /// Respuesta si no hay PropertyTypes:
    /// 204 No Content
    /// </remarks>
    public class GetAllPropertyTypeWithIncludeQuery : IRequest<IList<PropertyTypeDto>>
    {
    }

    /// <summary>
    /// Handler para GetAllPropertyTypeWithIncludeQuery
    /// </summary>
    public class GetAllPropertyTypeWithIncludeQueryHandler : IRequestHandler<GetAllPropertyTypeWithIncludeQuery, IList<PropertyTypeDto>>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPropertyTypeWithIncludeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la query para obtener todos los PropertyTypes con sus propiedades incluidas
        /// </summary>
        /// <param name="request">Query sin parámetros</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de PropertyTypeDto con propiedades relacionadas</returns>
        public async Task<IList<PropertyTypeDto>> Handle(GetAllPropertyTypeWithIncludeQuery request, CancellationToken cancellationToken)
        {
            // Obtenemos la query incluyendo la relación "Properties"
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });

            // Convertimos a DTOs usando AutoMapper
            var listEntityDtos = await listEntitiesQuery
                .ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return listEntityDtos;
        }
    }
}
