﻿using iHealth.Core.Application.Interfaces.Repositories;
using iHealth.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace iHealth.Infrastructure.Persistence.Repository
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        private readonly iHealthContext _context;
        private readonly DbSet<Entity> _entities;
        public GenericRepository(iHealthContext context) 
        {
            _context = context;
            _entities = context.Set<Entity>();
        }
        public virtual async Task DeleteAsync(Entity entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<Entity, bool>> filter)
        {
            return await _entities.AnyAsync(filter);
        }

        public virtual async Task<List<Entity>> FindAllAsync(Expression<Func<Entity, bool>> filter)
        {
            return  await _entities.Where(filter).ToListAsync();
        }

        public virtual async Task<List<Entity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public virtual async Task<Entity> GetEntityByIDAsync(int Id)
        {
            return await _entities.FindAsync(Id);
        }

        public virtual async Task<Entity> SaveAsync(Entity entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(Entity entity, int Id)
        {
            var entry = await _context.Set<Entity>().FindAsync(Id);
            _context.Entry(entry).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Entity>> GetAllWithIncludeAsync(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();

            foreach (string property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }
    }
}
