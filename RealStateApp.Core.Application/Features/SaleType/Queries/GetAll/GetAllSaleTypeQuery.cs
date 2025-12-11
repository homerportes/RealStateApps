using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetAll
{
    /// <summary>
    /// Query para obtener todos los SaleTypes
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/saletypes
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "Venta Normal",
    ///     "description": "Venta estándar"
    ///   },
    ///   {
    ///     "id": 2,
    ///     "name": "Venta Premium",
    ///     "description": "Venta con beneficios adicionales"
    ///   }
    /// ]
    /// 
    /// Respuesta vacía:
    /// 204 No Content
    /// </remarks>
    public class GetAllSaleTypeQuery : IRequest<IList<SaleTypeDto>>
    {
    }

    /// <summary>
    /// Handler para la query GetAllSaleTypeQuery
    /// </summary>
    public class GetAllSaleTypeQueryHandler : IRequestHandler<GetAllSaleTypeQuery, IList<SaleTypeDto>>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSaleTypeQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la query para obtener todos los SaleTypes
        /// </summary>
        /// <param name="request">Query</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de SaleTypeDto</returns>
        public async Task<IList<SaleTypeDto>> Handle(GetAllSaleTypeQuery request, CancellationToken cancellationToken)
        {
            // Obtiene la query de todos los SaleTypes desde el repositorio
            var listEntitiesQuery = _repository.GetAllQuery();

            // Mapea a DTOs y ejecuta la consulta
            var listEntityDtos = await listEntitiesQuery
                .ProjectTo<SaleTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return listEntityDtos;
        }
    }
}
