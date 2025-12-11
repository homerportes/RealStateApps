using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.SaleType;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using System.Threading;

using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetById
{
    /// <summary>
    /// Query para obtener un SaleType por su Id
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/saletypes/{id}
    /// 
    /// Respuesta exitosa:
    /// 200 OK
    /// {
    ///   "id": 1,
    ///   "name": "Venta Normal",
    ///   "description": "Venta estándar"
    /// }
    /// 
    /// Respuesta si no existe:
    /// 404 Not Found
    /// {
    ///   "message": "Sale Type not found with Id"
    /// }
    /// </remarks>
    public class GetSaleTypeByIdQuery : IRequest<SaleTypeDto>
    {
        /// <summary>
        /// Id del SaleType a obtener
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler para la query GetSaleTypeByIdQuery
    /// </summary>
    public class GetSaleTypeByIdQueryHandler : IRequestHandler<GetSaleTypeByIdQuery, SaleTypeDto>
    {
        private readonly ISaleTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetSaleTypeByIdQueryHandler(ISaleTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la query para obtener un SaleType por Id
        /// </summary>
        /// <param name="request">Query con el Id del SaleType</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>SaleTypeDto con la información del SaleType</returns>
        /// <exception cref="ApiException">Lanzada si no se encuentra el SaleType</exception>
        public async Task<SaleTypeDto> Handle(GetSaleTypeByIdQuery request, CancellationToken cancellationToken)
        {
            // Obtiene la query con include de propiedades relacionadas
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });


            // Busca el SaleType por Id
            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null) throw new ApiException("Sale Type not found with Id",(int)HttpStatusCode.NotFound);


          
            // Mapea la entidad a DTO
            var dto = _mapper.Map<SaleTypeDto>(entity);
            return dto;
        }
    }
}
