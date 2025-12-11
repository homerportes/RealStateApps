using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Queries.GetAll
{
    /// <summary>
    /// Query para obtener todas las mejoras (Improvements) registradas en el sistema.
    /// </summary>
    /// <remarks>
    /// Ejemplo de respuesta:
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "Cámaras de seguridad",
    ///     "description": "Se instalan cámaras de última generación"
    ///   },
    ///   {
    ///     "id": 2,
    ///     "name": "Piscina",
    ///     "description": "Piscina al aire libre con climatización"
    ///   }
    /// ]
    /// </remarks>
    public class GetAllImprovementQuery : IRequest<IList<ImprovementDto>>
    {
    }

    /// <summary>
    /// Handler encargado de procesar la obtención de todas las mejoras.
    /// </summary>
    public class GetAllImprovementQueryHandler : IRequestHandler<GetAllImprovementQuery, IList<ImprovementDto>>
    {
        private readonly IImprovementRepository _improvementRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del handler.
        /// </summary>
        /// <param name="improvementRepository">Repositorio para acceder a la base de datos de mejoras.</param>
        /// <param name="mapper">Mapper para convertir entidades a DTOs.</param>
        public GetAllImprovementQueryHandler(IImprovementRepository improvementRepository, IMapper mapper)
        {
            _improvementRepository = improvementRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la consulta para obtener todas las mejoras.
        /// </summary>
        /// <param name="request">Query sin parámetros.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Lista de mejoras en formato DTO.</returns>
        public async Task<IList<ImprovementDto>> Handle(GetAllImprovementQuery request, CancellationToken cancellationToken)
        {
            // Obtener queryable de la base de datos
            var listEntitiesQuery = _improvementRepository.GetAllQuery();

            // Proyectar entidades a DTOs usando AutoMapper y ejecutar la consulta
            var listEntityDtos = await listEntitiesQuery
                .ProjectTo<ImprovementDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return listEntityDtos;
        }
    }
}
