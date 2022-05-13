using ERPService.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ERPService.Common.Interfaces
{
    public interface IRepository
    {
        T GetById<T>(Guid id) where T : BaseEntity;
        IQueryable<T> GetQuery<T>() where T : BaseEntity;
        IQueryable<T> GetQuery<T>(params object[] includes) where T : BaseEntity;
        T Get<T>(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includes) where T : BaseEntity;
        List<T> List<T>(Func<T, bool> predicate) where T : BaseEntity;
        T Add<T>(T entity, bool comit = false) where T : BaseEntity;
        T Update<T>(T entity, bool comit = false) where T : BaseEntity;
        void Delete<T>(T entity, bool comit = false) where T : BaseEntity;
        void SaveChanges();
    }
}
