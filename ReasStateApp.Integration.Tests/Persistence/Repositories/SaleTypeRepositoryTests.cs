using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

using FluentAssertions;

namespace RealStateApp.Integration.Tests.Persistence.Repositories
{
    public class SaleTypeRepositoryTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;


        public SaleTypeRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]

        public async Task AddAsync_Should_Add_Sale_Type_To_Database()
        {
            using var context= new RealStateContext( _dbContextOptions );
            var SaleTypeRepository= new SaleTypeRepository(context);
            var SaleType = new RealStateApp.Core.Domain.Entities.SaleType()
            {
                Id = 0,
                Name ="This is a test sale Type",
                Description="Test SaleType",
            };

            //Act
            var result= await SaleTypeRepository.AddAsync(SaleType);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan( 0 );
            var saleypes= await context.SaleTypes.ToListAsync();
            saleypes.Should().ContainSingle();
        }




        [Fact]

        public async Task AddAsync_Should_Throw_When_Null()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);


            //Act

                Func<Task> act = async () => await SaleTypeRepository.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]

        public async Task GetById_Should_Return_Sale_Type_When_Exists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
            var SaleType = new RealStateApp.Core.Domain.Entities.SaleType()
            {
                Id = 0,
                Name = "This is a test sale Type",
                Description = "Test SaleType",
            };

            //Act
            var saleType = await SaleTypeRepository.AddAsync(SaleType);

            var result = await SaleTypeRepository.GetByIdAsync(saleType!.Id);


            //Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            var saleypes = await context.SaleTypes.ToListAsync();
            saleypes.Should().ContainSingle();
        }


        [Fact]

        public async Task GetById_Should_Return_Sale_Type_When_NotExists()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
         
            //Act

            var result = await SaleTypeRepository.GetByIdAsync(9999);


            //Assert
            result.Should().BeNull();
           
        }



        [Fact]

        public async Task UpdateAsync_Should_Modify_Sale_Type_InDatabase()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
            var SaleType = new RealStateApp.Core.Domain.Entities.SaleType()
            {
                Id = 0,
                Name = "This is a test sale Type",
                Description = "Test SaleType",
            };

            //Act
            var saleType = await SaleTypeRepository.AddAsync(SaleType);
            saleType!.Name = "Updated saleType";
            saleType.Description = "Updated Description";

            var updated = await SaleTypeRepository.UpdateAsync(saleType.Id, saleType);


            //Assert
            updated.Should().NotBeNull();
            updated.Name.Should().Be("Updated saleType");
            updated.Description.Should().Be("Updated Description");

        }



        [Fact]

        public async Task UpdateAsync_Should_Return_Null_When_Sale_Type_NotFound()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
            var SaleType = new RealStateApp.Core.Domain.Entities.SaleType()
            {
                Id = 0,
                Name = "This is a test sale Type",
                Description = "Test SaleType",
            };

            //Act
            var saleType = await SaleTypeRepository.AddAsync(SaleType);
            saleType!.Name = "Updated saleType";
            saleType.Description = "Updated Description";

            var updated = await SaleTypeRepository.UpdateAsync(9, saleType);


            //Assert
            updated.Should().BeNull();
           

        }


        [Fact]

        public async Task DeleteAsync_Should_Remove_SaletType()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
            var SaleType = new RealStateApp.Core.Domain.Entities.SaleType()
            {
                Id = 0,
                Name = "This is a test sale Type",
                Description = "Test SaleType",
            };

            //Act
            var saleType = await SaleTypeRepository.AddAsync(SaleType);
          

            await SaleTypeRepository.DeleteAsync(saleType!.Id);
            var entity= await SaleTypeRepository.GetByIdAsync(saleType.Id);

            //Assert
            entity.Should().BeNull();


        }


        [Fact]

        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Found()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
          
            //Act
            Func<Task> act= async ()=> await SaleTypeRepository.DeleteAsync(9999);


            //Assert
            await act.Should().NotThrowAsync();


        }



        [Fact]

        public async Task GetAllAsync_Should_Return_All_SaleTypes()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
            await context.SaleTypes.AddRangeAsync(

                 new Core.Domain.Entities.SaleType { Name = "saletype1", Description = "Description1", Id = 0 },
                 new Core.Domain.Entities.SaleType { Name = "saletype2", Description = "Description2", Id = 0 }

                 );
            await context.SaveChangesAsync();
            //Act
            var result = await SaleTypeRepository.GetAllList();



            //Assert
            result.Should().HaveCount(2);


        }



        [Fact]

        public async Task GetAllAsync_Should_Return_Empty_When_No_SaleTypes()
        {
            using var context = new RealStateContext(_dbContextOptions);
            var SaleTypeRepository = new SaleTypeRepository(context);
         
            //Act
            var result = await SaleTypeRepository.GetAllList();



            //Assert
            result.Should().BeEmpty();


        }
    }
}
