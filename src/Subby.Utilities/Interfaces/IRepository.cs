using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastContent.Utilities.StoredProcedures;
using Microsoft.EntityFrameworkCore.Storage;
using NHibernate;

namespace Subby.Utilities.Interfaces
{
    public interface IRepository
    {
        // Task<T> GetByIdAsync<T>(int id) where T : BaseEntity;
        // Task<List<T>> ListAsync<T>() where T : BaseEntity;
        // Task<T> AddAsync<T>(T entity) where T : BaseEntity;
        // Task UpdateAsync<T>(T entity) where T : BaseEntity;
        // Task DeleteAsync<T>(T entity) where T : BaseEntity;
        // IQueryable<T> Linq<T>() where T : BaseEntity;
        TEntity GetById<TEntity>(int id) where TEntity : BaseEntity;
        TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void Update<TEntity>(TEntity entity) where TEntity : BaseEntity;
        void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;

        void Save();
        IQueryable<TEntity> Linq<TEntity>() where TEntity : BaseEntity;
        ITransaction BeginTransaction();
        IStoredProcedureBuilder BuildStoredProcedure();

    }
}