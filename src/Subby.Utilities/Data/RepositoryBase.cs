using System;
using System.Linq;
using LastContent.Utilities.Data;
using LastContent.Utilities.StoredProcedures;
using Microsoft.EntityFrameworkCore.Storage;
using NHibernate;
using Subby.Utilities.Interfaces;

namespace Subby.Utilities.Data
{
    public class RepositoryBase : IRepository, IDisposable
    {
        private ISession _session = null;
        private bool _disposed;
        public RepositoryBase()
        {
            _session = Database.OpenSession();
        }

        public RepositoryBase(ISession session)
        {
            _session = session;
        }

        public IStoredProcedureBuilder BuildStoredProcedure()
        {
            var builder = new NHibernateStoredProcedure(_session);
            return builder;
        }

        #region IRepository Members

        public TEntity GetById<TEntity>(int id) where TEntity : BaseEntity
        {
            return _session.Query<TEntity>().FirstOrDefault(x => x.Id == id);
        }

        public TEntity Add<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            _session.SaveOrUpdate(entity);
            return entity;
        }

        public void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            _session.SaveOrUpdate(entity);
            _session.Flush();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            _session.Delete(entity);
        }

        public void Save()
        {
            _session.Flush();
        }

        public virtual IQueryable<TEntity> Linq<TEntity>() where TEntity : BaseEntity
        {
            return (from entity in _session.Query<TEntity>() select entity);
        }

        

        #endregion

        public ITransaction BeginTransaction()
        {
            return _session.BeginTransaction();
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _session.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}