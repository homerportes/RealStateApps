using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;


namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class ImprovementRepositoryTests
    {

        private readonly DbContextOptions<RealStateContext> _dbContextOptions;


        public ImprovementRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]

        public async Task AddAsync_Should_Add_Improvement_To_Database()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.Improvement()
            {
                Id = 0,
                Name = "Improvement",
                Description = "This is a test improvement",
            };

            //Act
            var result = await repository.AddAsync(entity);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var improvements = await context.Improvements.ToListAsync();
            improvements.Should().ContainSingle();
        }




        [Fact]

        public async Task AddAsync_Should_Throw_When_Null()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);


            //Act

            Func<Task> act = async () => await repository.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]

        public async Task GetById_Should_Return_Improvement_When_Exists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var entity = new RealStateApp.Core.Domain.Entities.Improvement()
            {
                Id = 0,
                Name = "Improvement",
                Description = "This is a test improvement",
            };


            //Act
            var entityResult = await repository.AddAsync(entity);

            var result = await repository.GetByIdAsync(entityResult!.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var improvements = await context.Improvements.ToListAsync();
            improvements.Should().ContainSingle();
        }


        [Fact]

        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);

            //Act

            var result = await repository.GetByIdAsync(9999);


            //Assert
            result.Should().BeNull();

        }



        [Fact]

        public async Task UpdateAsync_Should_Modify_Sale_Type_InDatabase()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var improvement = new RealStateApp.Core.Domain.Entities.Improvement()
            {
                Id = 0,
                Name = "This is a test improvement",
                Description = "Test improvement",
            };

            //Act
            var saleType = await repository.AddAsync(improvement);
            saleType!.Name = "Updated improvement";
            saleType.Description = "Updated Description";

            var updated = await repository.UpdateAsync(saleType.Id, saleType);


            //Assert
            updated.Should().NotBeNull();
            updated.Name.Should().Be("Updated improvement");
            updated.Description.Should().Be("Updated Description");

        }



        [Fact]

        public async Task UpdateAsync_Should_Return_Null_When_Improvement_NotFound()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var improvement = new RealStateApp.Core.Domain.Entities.Improvement()
            {
                Id = 0,
                Name = "This is a test improvement",
                Description = "Test improvement",
            };


            //Act
            var improvementResult = await repository.AddAsync(improvement);
            improvementResult!.Name = "Updated saleType";
            improvementResult.Description = "Updated Description";

            var updated = await repository.UpdateAsync(9, improvementResult);


            //Assert
            updated.Should().BeNull();


        }


        [Fact]

        public async Task DeleteAsync_Should_Remove_SaletType()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            var improvement = new RealStateApp.Core.Domain.Entities.Improvement()
            {
                Id = 0,
                Name = "This is a test improvement",
                Description = "Test improvement",
            };


            //Act
            var improvementResult = await repository.AddAsync(improvement);


            await repository.DeleteAsync(improvement.Id);
            var entity = await repository.GetByIdAsync(improvement.Id);

            //Assert
            entity.Should().BeNull();


        }


        [Fact]

        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Found()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);

            //Act
            Func<Task> act = async () => await repository.DeleteAsync(9999);


            //Assert
            await act.Should().NotThrowAsync();


        }



        [Fact]

        public async Task GetAllAsync_Should_Return_All_Improvements()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);
            await context.Improvements.AddRangeAsync(

                 new Core.Domain.Entities.Improvement { Name = "improvement1", Description = "Description1", Id = 0 },
                 new Core.Domain.Entities.Improvement { Name = "improvement2", Description = "Description2", Id = 0 }

                 );
            await context.SaveChangesAsync();
            //Act
            var result = await repository.GetAllList();



            //Assert
            result.Should().HaveCount(2);


        }


        [Fact]

        public async Task GetAllAsync_Should_Return_Empty_When_Improvements()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var repository = new ImprovementRepository(context);

            //Act
            var result = await repository.GetAllList();



            //Assert
            result.Should().BeEmpty();


        }
    }
}
