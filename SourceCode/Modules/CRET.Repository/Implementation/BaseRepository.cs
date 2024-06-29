using CRET.DataAccessLayer;
using CRET.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRET.Repository.Implementation
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private DbSet<TEntity> dbSet;
        #endregion

        #region Constructor
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = _context.Set<TEntity>();
        }
        #endregion

        #region IRepository<TEntity> Implementation
        /// <summary>
        /// Implementation of fetching all the records of a DBSet.
        /// </summary>
        public IEnumerable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }


            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        /// <summary>
        /// Implementation of fetching record on the basis of ID.
        /// </summary>
        public TEntity GetByID(Guid id)
        {
            return dbSet.Find(id);
        }
        /// <summary>
        /// Implementation of adding new record to DBSet.
        /// </summary>
        public void Insert(TEntity entity)
        {
            dbSet.Add(entity);
            _context.SaveChanges();
        }
        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            //foreach (var item in entities)
            //{
            //    dbSet.Add(item);
            //}
            dbSet.AddRange(entities);
            _context.SaveChanges();
        }
        /// <summary>
        /// Implementation of updating existing record of DBSet.
        /// </summary>
        public void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Detached;
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
        } 
        /// <summary>
        /// Implementation of rnage updating existing record of DBSet.
        /// </summary>
        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            foreach (var entity in entitiesToUpdate)
            {
                dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Detached;
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }
        /// <summary>
        /// Implementation of deleting a record of a DBSet.
        /// </summary>
        public void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            _context.SaveChanges();
        }
        /// <summary>
        /// Implementation of deleting a record of a DBSet on the basis of ID.
        /// </summary>
        public void Delete(Guid id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        /// <summary>
        /// Implementation of deleting a range of records of a DBSet.
        /// </summary>
        public void DeleteRange()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Implementation of deleting all records of a DBSet.
        /// </summary>
        public void DeleteAll()
        {
            dbSet.RemoveRange(dbSet);
            _context.SaveChanges();
        }
        #endregion

    }
}
