using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Improvement;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Queries.GetById
{
    /// <summary>
    /// Query para obtener una mejora específica por su Id.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/improvements/5
    /// 
    /// Ejemplo de respuesta:
    /// {
    ///   "id": 5,
    ///   "name": "Cámaras de seguridad",
    ///   "description": "Se instalan cámaras de última generación"
    /// }
    /// </remarks>
    public class GetImprovementByIdQuery : IRequest<ImprovementDto>
    {
        /// <summary>
        /// Id de la mejora a obtener
        /// </summary>
        /// <example>5</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler encargado de obtener la mejora por Id
    /// </summary>
    public class GetImprovementByIdQueryHandler : IRequestHandler<GetImprovementByIdQuery, ImprovementDto>
    {
        private readonly IImprovementRepository _improvementRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del handler
        /// </summary>
        /// <param name="improvementRepository">Repositorio de mejoras</param>
        /// <param name="mapper">Mapper para convertir entidades a DTO</param>
        public GetImprovementByIdQueryHandler(IImprovementRepository improvementRepository, IMapper mapper)
        {
            _improvementRepository = improvementRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la consulta para obtener la mejora por Id
        /// </summary>
        /// <param name="request">Query con Id de la mejora</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Mejora en formato DTO</returns>
        /// <exception cref="ApiException">Si no se encuentra la mejora</exception>
        public async Task<ImprovementDto> Handle(GetImprovementByIdQuery request, CancellationToken cancellationToken)
        {
            // Obtener queryable incluyendo propiedades relacionadas
            var listEntitiesQuery = _improvementRepository.GetAllQueryWithInclude(new List<string> { "PropertyImprovements" });

            // Buscar la entidad por Id
            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);

            if (entity == null)
                throw new ApiException("Entity Not found with this id");

            // Mapear a DTO
            var dto = _mapper.Map<ImprovementDto>(entity);
            return dto;
        }
    }
}
