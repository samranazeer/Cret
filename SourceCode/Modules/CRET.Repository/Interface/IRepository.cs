using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace CRET.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region Methods
        /// <summary>
        /// Implementation of fetching all the records of a DBSet.
        /// </summary>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        /// <summary>
        /// Implementation of fetching record on the basis of ID.
        /// </summary>
        TEntity GetByID(Guid id);
        /// <summary>
        /// Implementation of adding new record to DBSet.
        /// </summary>
        void Insert(TEntity entity);
        /// <summary>
        /// Implementation of adding list to DBSet.
        /// </summary>
        void BulkInsert(IEnumerable<TEntity> entities);
        /// <summary>
        /// Implementation of updating existing record of DBSet.
        /// </summary>
        void Update(TEntity entityToUpdate);
        void UpdateRange(IEnumerable<TEntity> entitiesToUpdate);

        /// <summary>
        /// Implementation of deleting a record of a DBSet.
        /// </summary>
        void Delete(TEntity entityToDelete);
        /// <summary>
        /// Implementation of deleting a record of a DBSet on the basis of ID.
        /// </summary>
        void Delete(Guid id);
        /// <summary>
        /// Implementation of deleting a range of records of a DBSet.
        /// </summary>
        void DeleteRange();
        /// <summary>
        /// Implementation of deleting all records of a DBSet.
        /// </summary>
        void DeleteAll();
        #endregion
    }
}
