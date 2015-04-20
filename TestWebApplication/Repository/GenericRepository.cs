using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace OrderWebApplication.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        internal OrderDBContext context;
        internal DbSet<TEntity> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public GenericRepository(OrderDBContext context){
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the entities in the database.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">What to order by.</param>
        /// <param name="includeProperties">What other entities to include.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
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
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Gets the entity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual TEntity GetById(int id)
        {
            return dbSet.Find(id);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        /// <summary>
        /// Deletes the entity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual void Delete(int id)
        {
            TEntity entity = dbSet.Find(id);
            dbSet.Remove(entity);
        }

        /// <summary>
        /// Deletes the entity with the given identifier.
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Edit(TEntity entity)
        {
            dbSet.Attach(entity);

            context.Entry(entity).State = EntityState.Modified;
        }
    }
}