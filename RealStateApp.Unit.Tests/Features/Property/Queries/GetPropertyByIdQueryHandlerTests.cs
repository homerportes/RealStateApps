using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Property.Queries.GetById;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Queries
{
    public class GetPropertyByIdQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly DbContextOptions<IdentityContext> _identityDbOptions;
        private readonly IMapper _mapper;
        private readonly Mock<IUserService> _userServiceMock;

        public GetPropertyByIdQueryHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyToDtoMappingProfile>();
                cfg.AddProfile<PropertyTypeToDtoMappingProfile>();
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
                cfg.AddProfile<ImprovementToDtoMappingProfile>();
            }, loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _identityDbOptions = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: $"IdentityTestDb_{Guid.NewGuid()}")
                .Options;

            // Mock IUserService
            _userServiceMock = new Mock<IUserService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnProperty_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_identityDbOptions);

            // Crear agente
            var agent = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Jane",
                LastName = "Smith",
                Dni = "98765432109",
                UserName = "janesmith",
                Email = "jane@example.com",
                PhoneNumber = "8099876543"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Crear PropertyType y SaleType
            var propertyType = new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "Apartamento",
                Description = "Apartamento moderno"
            };

            var saleType = new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "Alquiler",
                Description = "Alquiler mensual"
            };

            context.PropertyTypes.Add(propertyType);
            context.SaleTypes.Add(saleType);
            await context.SaveChangesAsync();

            // Crear Improvements
            var improvement1 = new Core.Domain.Entities.Improvement
            {
                Id = 1,
                Name = "Gimnasio",
                Description = "Gimnasio completo"
            };

            var improvement2 = new Core.Domain.Entities.Improvement
            {
                Id = 2,
                Name = "Parqueo",
                Description = "Parqueo techado"
            };

            context.Improvements.AddRange(improvement1, improvement2);
            await context.SaveChangesAsync();

            // Crear Property
            var property = new Core.Domain.Entities.Property
            {
                Id = 1,
                Code = "DEF456",
                Price = 250000,
                SizeInMeters = 85.0,
                Bedrooms = 2,
                Bathrooms = 1,
                Description = "Apartamento céntrico",
                Status = PropertyStatus.Available,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = agent.Id
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            // Agregar PropertyImprovements
            context.PropertyImprovements.AddRange(
                new Core.Domain.Entities.PropertyImprovement { PropertyId = 1, ImprovementId = 1 },
                new Core.Domain.Entities.PropertyImprovement { PropertyId = 1, ImprovementId = 2 }
            );
            await context.SaveChangesAsync();

            // Configurar UserService mock
            _userServiceMock.Setup(us => us.GetById(agent.Id))
                .ReturnsAsync(new UserDto
                {
                    Id = agent.Id,
                    FirstName = agent.FirstName,
                    LastName = agent.LastName,
                    Email = agent.Email,
                    PhoneNumber = agent.PhoneNumber,
                    UserName = agent.UserName,
                    Dni = agent.Dni,
                    Role = "Agent"
                });

            var repository = new PropertyRepository(context);
            var handler = new GetPropertyByIdQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            var result = await handler.Handle(new GetPropertyByIdQuery { Id = 1 }, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Code.Should().Be("DEF456");
            result.PropertyType.Should().Be("Apartamento");
            result.SaleType.Should().Be("Alquiler");
            result.AgentName.Should().Be("Jane Smith");
            result.AgentId.Should().Be(agent.Id);
            result.Improvements.Should().HaveCount(2);
            result.Improvements.Should().Contain(new[] { "Gimnasio", "Parqueo" });
            result.Bedrooms.Should().Be(2);
            result.Bathrooms.Should().Be(1);
            result.Price.Should().Be(250000);
            result.Status.Should().Be(PropertyStatus.Available);
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            var repository = new PropertyRepository(context);
            var handler = new GetPropertyByIdQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(
                new GetPropertyByIdQuery { Id = 999 }, 
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("El id de la propiedad es inválido");
        }
    }
}
