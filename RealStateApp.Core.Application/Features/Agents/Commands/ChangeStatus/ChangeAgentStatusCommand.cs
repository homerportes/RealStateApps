using MediatR;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus
{
    public class ChangeAgentStatusCommand : IRequest<Unit>
    {
        public string ? Id { get; set; }
        public bool Status { get; set; }
    }

    public class ChangeAgentStatusCommandHandler : IRequestHandler<ChangeAgentStatusCommand, Unit>
    {
        private readonly IUserService _userService;

        public ChangeAgentStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<Unit> Handle(ChangeAgentStatusCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id))
                throw new ApiException("Agent Id is required", HttpStatusCode.BadRequest);

            var operationStatus= await _userService.SetStatus(request.Id, request.Status);

            if (operationStatus == false) throw new ApiException("Entity Not found", HttpStatusCode.NotFound);
            return Unit.Value;
        }
    }
}
