using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Improvement.Commands.CreateImprovement;
using RealStateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType
{
    public class CreateSaleTypeCommand: IRequest<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }


    public class CreateSaleTypeCommandHandler : IRequestHandler<CreateSaleTypeCommand, int>
    {

        private readonly ISaleTypeRepository _saleTypeRepository;
        public CreateSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }
        public async Task<int> Handle(CreateSaleTypeCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.SaleType entity = new()
            {
                Id = 0,
                Description = request.Description,
                Name = request.Name,
            };
           


            entity = await _saleTypeRepository.AddAsync(entity);

            if (entity == null)
                throw new ApiException("Error creating sale type", (int)HttpStatusCode.InternalServerError);
            return entity!.Id;
        }
    }

}
