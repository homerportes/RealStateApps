using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class PropertyTypeRepositoryTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;


        public PropertyTypeRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]

        public async Task AddAsync_Should_Add_Sale_Type_To_Database()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.PropertyType()
            {
                Id = 0,
                Name = "This is a test property Type",
                Description = "Test property type",
            };

            //Act
            var result = await repository.AddAsync(entity);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var saleypes = await context.PropertyTypes.ToListAsync();
            saleypes.Should().ContainSingle();
        }




        [Fact]

        public async Task AddAsync_Should_Throw_When_Null()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);


            //Act

            Func<Task> act = async () => await repository.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]

        public async Task GetById_Should_Return_Sale_Type_When_Exists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.PropertyType()
            {
                Id = 0,
                Name = "This is a test property Type",
                Description = "Test property type",
            };

            //Act
            var saleType = await repository.AddAsync(entity);

            var result = await repository.GetByIdAsync(saleType!.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var saleypes = await context.PropertyTypes.ToListAsync();
            saleypes.Should().ContainSingle();
        }


        [Fact]

        public async Task GetById_Should_Return_Property_Type_When_NotExists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);

            //Act

            var result = await repository.GetByIdAsync(9999);


            //Assert
            result.Should().BeNull();

        }



        [Fact]

        public async Task UpdateAsync_Should_Modify_Property_Type_InDatabase()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.PropertyType()
            {
                Id = 0,
                Name = "This is a test property Type",
                Description = "Test property type",
            };

            //Act
            var updatedEntity = await repository.AddAsync(entity);
            updatedEntity!.Name = "Updated property type";
            updatedEntity.Description = "Updated Description";

            var updated = await repository.UpdateAsync(updatedEntity.Id, updatedEntity);


            //Assert
            updated.Should().NotBeNull();
            updated.Name.Should().Be("Updated property type");
            updated.Description.Should().Be("Updated Description");

        }



        [Fact]

        public async Task UpdateAsync_Should_Return_Null_When_Propert_Type_NotFound()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.PropertyType()
            {
                Id = 0,
                Name = "This is a test property Type",
                Description = "Test property type",
            };
            //Act
            var savedEntity = await repository.AddAsync(entity);
            savedEntity!.Name = "Updated property type";
            savedEntity.Description = "Updated Description";

            var updated = await repository.UpdateAsync(9, savedEntity);


            //Assert
            updated.Should().BeNull();


        }


        [Fact]

        public async Task DeleteAsync_Should_Remove_PropertyType()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            var propertyType = new RealStateApp.Core.Domain.Entities.PropertyType()
            {
                Id = 0,
                Name = "This is a test property Type",
                Description = "Test description",
            };

            //Act
            var savedEntity = await repository.AddAsync(propertyType);


            await repository.DeleteAsync(savedEntity!.Id);
            var entity = await repository.GetByIdAsync(savedEntity.Id);

            //Assert
            entity.Should().BeNull();


        }


        [Fact]

        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Found()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);

            //Act
            Func<Task> act = async () => await repository.DeleteAsync(9999);


            //Assert
            await act.Should().NotThrowAsync();


        }



        [Fact]

        public async Task GetAllAsync_Should_Return_All_PropertyType()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyTypeRepository(context);
            await context.PropertyTypes.AddRangeAsync(

                 new Core.Domain.Entities.PropertyType { Name = "propertyType1", Description = "Description1", Id = 0 },
                 new Core.Domain.Entities.PropertyType { Name = "propertyType1", Description = "Description2", Id = 0 }

                 );
            await context.SaveChangesAsync();
            //Act
            var result = await repository.GetAllList();



            //Assert
            result.Should().HaveCount(2);


        }


        [Fact]

        public async Task GetAllAsync_Should_Return_Empty_When_No_PropertyTypes()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new PropertyRepository(context);

            //Act
            var result = await repository.GetAllList();



            //Assert
            result.Should().BeEmpty();


        }
    }
}
