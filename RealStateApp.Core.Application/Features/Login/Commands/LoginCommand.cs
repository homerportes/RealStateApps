using MediatR;
using Newtonsoft.Json;
using RealStateApp.Core.Application.Dtos.Login;
using RealStateApp.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Login.Commands
{
    public class LoginCommand : IRequest<LoginResponseForApi>
    {
        [JsonProperty("usuario")]

        public required string Username { get; set; }
        [JsonProperty("contrasena")]

        public required string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseForApi>
    {
        private readonly IAccountServiceForWebApi _accountServiceForApi;

        public LoginCommandHandler(IAccountServiceForWebApi accountServiceForWebApi)
        {
           _accountServiceForApi=accountServiceForWebApi; 
        }
        public  async Task<LoginResponseForApi> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountServiceForApi.AuthenticateAsync(new LoginDto { Password = request.Password, Username = request.Username });
            
            return result;
        }
    }


}
