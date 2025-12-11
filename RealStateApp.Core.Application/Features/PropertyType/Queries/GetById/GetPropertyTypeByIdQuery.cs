using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Dtos.Properties;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Domain.Interfaces;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetById
{
    /// <summary>
    /// Query para obtener un PropertyType por su Id, incluyendo sus propiedades relacionadas.
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/v1/propertytypes/{id}
    ///
    /// Respuesta exitosa:
    /// 200 OK
    /// {
    ///   "id": 1,
    ///   "name": "Oficina",
    ///   "description": "Espacio para oficinas comerciales",
    ///   "properties": [
    ///       {
    ///           "id": 101,
    ///           "name": "Oficina Central",
    ///           "code": "OFC123",
    ///           "saleTypeId": 1,
    ///           "price": 500000
    ///       }
    ///   ]
    /// }
    ///
    /// Respuesta si no existe:
    /// 404 Not Found
    /// </remarks>
    public class GetPropertyTypeByIdQuery : IRequest<PropertyTypeDto>
    {
        /// <summary>
        /// Id del PropertyType a consultar
        /// </summary>
        /// <example>1</example>
        public required int Id { get; set; }
    }

    /// <summary>
    /// Handler para GetPropertyTypeByIdQuery
    /// </summary>
    public class GetPropertyTypeByIdQueryHandler : IRequestHandler<GetPropertyTypeByIdQuery, PropertyTypeDto>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetPropertyTypeByIdQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Ejecuta la query para obtener un PropertyType por Id con sus propiedades relacionadas
        /// </summary>
        /// <param name="request">Query con Id</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>PropertyTypeDto con lista de propiedades</returns>
        /// <exception cref="ApiException">Si no se encuentra la entidad</exception>
        public async Task<PropertyTypeDto> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
        {
            // Obtenemos la query incluyendo la relación "Properties"
            var listEntitiesQuery = _repository.GetAllQueryWithInclude(new List<string> { "Properties" });

            var entity = await listEntitiesQuery.FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null) throw new ApiException("Invalid Id",(int)HttpStatusCode.NotFound);


            // Convertimos la entidad a DTO
            var dto = _mapper.Map<PropertyTypeDto>(entity);
            return dto;
        }
    }
}
