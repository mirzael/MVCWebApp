using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderWebApplication.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the entities in the database.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">What to order by.</param>
        /// <param name="includeProperties">What other entities to include.</param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "");

        /// <summary>
        /// Gets the entity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        TEntity GetById(int id);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Deletes the entity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Delete(int id);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Specifies that the entity has been edited.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Edit(TEntity entity);
    }
}
