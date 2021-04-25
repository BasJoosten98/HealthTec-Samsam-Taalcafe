using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taalcafe.DataAccess
{
    public class GenericRepository<TEntity, TConttext> : IGenericRepository<TEntity>
         where TEntity : class
         where TConttext : IdentityDbContext
    {
        protected readonly TConttext Context;

        protected GenericRepository(TConttext context)
        {
            Context = context;
        }

        public virtual void Add(TEntity model)
        {
            Context.Set<TEntity>().Add(model);
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public virtual bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        public virtual void Remove(TEntity model)
        {
            Context.Set<TEntity>().Remove(model);
        }

        public void Update(TEntity model)
        {
            Context.Set<TEntity>().Update(model);
        }
    }
}
