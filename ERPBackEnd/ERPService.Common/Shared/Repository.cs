using ERPService.Common.Interfaces;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Common.Shared
{
    public abstract class Repository : IRepository
    {
        private readonly DbContext _dbContext;
        private readonly UserContext _userContext;

        /// <summary>
        /// schema parameter enables linquistic object model, if empty multilingual disabled
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userContext"></param>
        /// <param name="schema"></param>
        public Repository(DbContext dbContext, IOwinContext context)
        {
            _dbContext = dbContext;
            if (context.Environment.ContainsKey("USRCTX"))
                _userContext = (UserContext)context.Environment["USRCTX"];
            else
                _userContext = new UserContext();
        }

        public T GetById<T>(Guid id) where T : BaseEntity
        {
            return _dbContext.Set<T>().AsNoTracking().SingleOrDefault(e => e.Id == id);
        }

        public List<T> List<T>(Func<T, bool> predicate) where T : BaseEntity
        {
            if (predicate == null)
                throw new Exception("Cannot return all values, predicate must be set");

            return _dbContext.Set<T>().AsNoTracking().Where(predicate).ToList();
        }

        public T Add<T>(T entity, bool comit = false) where T : BaseEntity
        {
            _dbContext.Set<T>().Add(entity);
            entity.CreatedBy = _userContext.Id;
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedBy = null;
            entity.ModifiedDate = null;
            if (comit)
                _dbContext.SaveChanges();

            return entity;
        }

        public void Delete<T>(T entity, bool comit = false) where T : BaseEntity
        {
            var c = _dbContext.Entry(entity);
            c.State = EntityState.Deleted;
            _dbContext.Set<T>().Remove(entity);
            if (comit)
                _dbContext.SaveChanges();
        }

        public T Update<T>(T entity, bool comit = false) where T : BaseEntity
        {
            var c = _dbContext.Entry(entity);
            c.State = EntityState.Modified;
            entity.ModifiedBy = _userContext.Id;
            entity.ModifiedDate = DateTime.Now;
            c.Property(x => x.CreatedBy).IsModified = false;
            c.Property(x => x.CreatedDate).IsModified = false;

            if (comit)
                _dbContext.SaveChanges();
            return entity;
        }

        public IQueryable<T> GetQuery<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetQuery<T>(params object[] includes) where T : BaseEntity
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);
            return query;
        }

        public T Get<T>(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            IQueryable<T> query = GetQuery<T>();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefault(predicate);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
