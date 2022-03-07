using CubDomain.Crawler.BO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CubDomain.Crawler.Data
{
    public class GenericRepository<TEntity> where TEntity : class , IBaseBO
    {
        protected internal CContext context;
        protected internal DbSet<TEntity> dbSet;

        public GenericRepository(CContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public GenericRepository()
        {

        }

        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public virtual async Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }


            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual List<TEntity> GetSync(
            bool dispose,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? take = null,
            int? skip = null)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
			
			if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            
			if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            List<TEntity> list = query.ToList();
            
            if (dispose)
            {
                context.Dispose();
            }
            return list;
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbSet.Where(filter).SingleOrDefaultAsync();
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            return dbSet.Where(filter).SingleOrDefault();
        }

        public virtual async Task<TEntity> FindAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual TEntity Find(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Save(TEntity entity, Guid userAccountId, bool dispose)
        {
            if (entity.ID == new Guid())
            {
                entity.ID = Guid.NewGuid();
                entity.InsertDateTime = DateTime.Now;
                entity.UpdateDateTime = DateTime.Now;
                entity.InsertUserAccountId = userAccountId;
                entity.UpdateUserAccountId = userAccountId;
                dbSet.Add(entity);
            }
            else
            {
                entity.UpdateDateTime = DateTime.Now;
                entity.UpdateUserAccountId = userAccountId;
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
            }
            if (dispose)
            {
                context.SaveChanges();
                context.Dispose();
            }
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbSet.AnyAsync(filter);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return dbSet.Any(filter);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> ObjectsToBeRemoved, bool dispose)
        {
            dbSet.RemoveRange(ObjectsToBeRemoved);
            if (dispose)
            {
                context.SaveChanges();
                context.Dispose();
            }
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete, bool dispose)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            if (dispose)
            {
                context.SaveChanges();
                context.Dispose();
            }
        }
        public virtual void SaveChanges()
        {
            this.context.SaveChanges();
            this.context.Dispose();
        }
        public int GetCount(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Count();
        }
    }
}
