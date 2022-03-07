using CubDomain.Crawler.Data;
using CubDomain.Crawler.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CubDomain.Crawler.Service
{
    public class GenericService<TEntity> where TEntity : class,IBaseBO
    {
        private GenericRepository<TEntity> _repository;
        public TEntity EO;
        public GenericService(CContext context)
        {
            _repository = new GenericRepository<TEntity>(context);
            EO = Activator.CreateInstance<TEntity>();
        }
        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return _repository.GetWithRawSql(query, parameters);
        }

        public virtual async Task<List<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            return await _repository.Get(filter, orderBy, includeProperties);
        }

        public virtual List<TEntity> GetSync(
            bool dispose,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
             int? take = null,
            int? skip = null)
        {
            return _repository.GetSync(dispose,filter, orderBy, includeProperties, take, skip);
        }
        public virtual int GetCount(
           Expression<Func<TEntity, bool>> filter = null)
        {
            return _repository.GetCount(filter);
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _repository.GetSingleAsync(filter);
        }

        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            EO = _repository.GetSingle(filter);
            return EO;
        }

        public virtual async Task<TEntity> FindAsync(object id)
        {
            return await _repository.FindAsync(id);
        }

        public virtual TEntity Find(object id)
        {
            return _repository.Find(id);
        }

        public virtual void Save(Guid userAccountId, bool dispose)
        {
            _repository.Save(EO, userAccountId, dispose);
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _repository.AnyAsync(filter);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return _repository.Any(filter);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> ObjectsToBeRemoved, bool dispose)
        {
            _repository.DeleteRange(ObjectsToBeRemoved, dispose);
        }

        public virtual void Delete(object id, bool dispose)
        {
            TEntity entityToDelete = _repository.Find(id);
            Delete(entityToDelete, dispose);
        }

        public virtual void Delete(TEntity entityToDelete, bool dispose)
        {
            _repository.Delete(entityToDelete, dispose);
        }

        public virtual void SaveChanges()
        {
            _repository.SaveChanges();
        }
    }
}
