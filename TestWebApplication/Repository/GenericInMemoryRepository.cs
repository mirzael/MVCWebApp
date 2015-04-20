using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace OrderWebApplication.Repository
{
    public class GenericInMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : EntityWithId
    {
        private List<TEntity> entities = new List<TEntity>();

        /// <summary>
        /// Gets the TEntitys in the database.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">What to order by.</param>
        /// <param name="includeProperties">What other entities to include.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            List<TEntity> filtEntitys = entities;
            if (filter != null)
            {
                filtEntitys = filtEntitys.Where(filter.Compile()).ToList();
            }

            if (orderBy != null)
            {
                filtEntitys = orderBy(filtEntitys.AsQueryable()).ToList();
            }

            return filtEntitys;
        }

        /// <summary>
        /// Gets the TEntity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public TEntity GetById(int id)
        {
            return entities.Where(o => o.ID == id).FirstOrDefault();
        }

        /// <summary>
        /// Inserts the specified TEntity.
        /// </summary>
        /// <param name="entity">The TEntity.</param>
        public void Insert(TEntity entity)
        {
            entities.Add(entity);
        }

        /// <summary>
        /// Deletes the TEntity with the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Delete(int id)
        {
            var TEntity = entities.Find(o => o.ID == id);
            var index = entities.IndexOf(TEntity);

            entities.RemoveAt(index);
        }

        /// <summary>
        /// Deletes the TEntity with the given identifier.
        /// </summary>
        /// <param name="entityToDelete"></param>
        public void Delete(TEntity entityToDelete)
        {
            entities.Remove(entityToDelete);
        }

        /// <summary>
        /// Specifies that the TEntity has been edited.
        /// </summary>
        /// <param name="entity">The TEntity.</param>
        public void Edit(TEntity entity)
        {
            var TEntity = entities.Find(o => o.ID == entity.ID);
            var index = entities.IndexOf(TEntity);
            entities[index] = entity;
        }
    }
}