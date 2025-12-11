using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Persistence.Repositories
{
    public class GenericRepository <Entity>: IGenericRepository<Entity>where Entity : class
    {   private readonly RealStateContext _context;

        public GenericRepository(RealStateContext context)
        {
            _context = context;
        }



        public virtual async Task<Entity?> AddAsync(Entity entity)
        {
            if (entity == null)
          throw new ArgumentNullException();

        


            await _context.Set<Entity>().AddAsync(entity);

            var result =await _context.Set<Entity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return result !=null? entity :null;


        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Entity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<List<Entity>?> GetAllList()
        {
            return await _context.Set<Entity>().ToListAsync();
        }


        public virtual async Task<Entity?> GetByIdAsync(int id)
        {
            return await _context.Set<Entity>().FindAsync(id);
        }





        public async Task AddRangeAsync(List<Entity> entities)
        {
            await _context.Set<Entity>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();

        }



        public virtual async Task DeleteRangeAsync(List<Entity> entities)
        {
            _context.Set<Entity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }



        public virtual async Task<List<Entity>?> GetAllListWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }

        public virtual IQueryable<Entity> GetAllQuery()
        {
            return _context.Set<Entity>().AsQueryable();
        }
        public virtual IQueryable<Entity> GetAllQueryWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return query;
        }



        public virtual async Task<Entity?> UpdateAsync(int id, Entity entity)
        {
            var entry = await _context.Set<Entity>().FindAsync(id);

            if (entry != null)
            {
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            return entry;
        }
        public async Task UpdateRangeAsync(List<Entity> entities)
        {
            // limpiar todo el tracking de EF
            _context.ChangeTracker.Clear();

            var dbSet = _context.Set<Entity>();

            foreach (var entity in entities)
            {
                dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }


    }
}
