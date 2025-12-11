using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Property.Queries.GetByCode;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Queries
{
    public class GetPropertyByCodeQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly DbContextOptions<IdentityContext> _identityDbOptions;
        private readonly IMapper _mapper;
        private readonly Mock<IUserService> _userServiceMock;

        public GetPropertyByCodeQueryHandlerTests()
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
        public async Task Handle_ShouldReturnProperty_WhenCodeExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_identityDbOptions);

            // Crear agente
            var agent = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Carlos",
                LastName = "Rodriguez",
                Dni = "11122233344",
                UserName = "carlosr",
                Email = "carlos@example.com",
                PhoneNumber = "8091112233"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Crear PropertyType y SaleType
            var propertyType = new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "Villa",
                Description = "Villa de lujo"
            };

            var saleType = new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "Venta",
                Description = "Venta directa"
            };

            context.PropertyTypes.Add(propertyType);
            context.SaleTypes.Add(saleType);
            await context.SaveChangesAsync();

            // Crear Improvement
            var improvement = new Core.Domain.Entities.Improvement
            {
                Id = 1,
                Name = "Jacuzzi",
                Description = "Jacuzzi exterior"
            };

            context.Improvements.Add(improvement);
            await context.SaveChangesAsync();

            // Crear Property con código específico
            var property = new Core.Domain.Entities.Property
            {
                Id = 1,
                Code = "GHI789",
                Price = 500000,
                SizeInMeters = 250.0,
                Bedrooms = 5,
                Bathrooms = 4,
                Description = "Villa de lujo con vista al mar",
                Status = PropertyStatus.Available,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = agent.Id
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            // Agregar PropertyImprovement
            context.PropertyImprovements.Add(
                new Core.Domain.Entities.PropertyImprovement { PropertyId = 1, ImprovementId = 1 }
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
            var handler = new GetPropertyByCodeQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            var result = await handler.Handle(new GetPropertyByCodeQuery { Code = "GHI789" }, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("GHI789");
            result.PropertyType.Should().Be("Villa");
            result.SaleType.Should().Be("Venta");
            result.AgentName.Should().Be("Carlos Rodriguez");
            result.AgentId.Should().Be(agent.Id);
            result.Improvements.Should().Contain("Jacuzzi");
            result.Bedrooms.Should().Be(5);
            result.Bathrooms.Should().Be(4);
            result.Price.Should().Be(500000);
            result.SizeInMeters.Should().Be(250.0);
            result.Description.Should().Be("Villa de lujo con vista al mar");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenCodeDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            var repository = new PropertyRepository(context);
            var handler = new GetPropertyByCodeQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(
                new GetPropertyByCodeQuery { Code = "NOEXIST" }, 
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("El código de la propiedad es inválido");
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertyWithoutImprovements_WhenPropertyHasNoImprovements()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_identityDbOptions);

            // Crear agente
            var agent = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Maria",
                LastName = "Lopez",
                Dni = "55566677788",
                UserName = "marialop",
                Email = "maria@example.com",
                PhoneNumber = "8095556677"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Crear PropertyType y SaleType
            context.PropertyTypes.Add(new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "Estudio",
                Description = "Estudio pequeño"
            });

            context.SaleTypes.Add(new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "Alquiler",
                Description = "Alquiler mensual"
            });
            await context.SaveChangesAsync();

            // Crear Property sin mejoras
            var property = new Core.Domain.Entities.Property
            {
                Id = 1,
                Code = "STD001",
                Price = 50000,
                SizeInMeters = 35.0,
                Bedrooms = 1,
                Bathrooms = 1,
                Description = "Estudio básico",
                Status = PropertyStatus.Available,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = agent.Id
            };

            context.Properties.Add(property);
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
            var handler = new GetPropertyByCodeQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            var result = await handler.Handle(new GetPropertyByCodeQuery { Code = "STD001" }, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("STD001");
            result.Improvements.Should().BeEmpty();
        }
    }
}
