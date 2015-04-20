using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderWebApplication.Models;

namespace OrderWebApplication.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the order repository.
        /// </summary>
        /// <value>
        /// The order repository.
        /// </value>
        IRepository<Order> OrderRepository{ get; }
        /// <summary>
        /// Gets the item repository.
        /// </summary>
        /// <value>
        /// The item repository.
        /// </value>
        IRepository<Item> ItemRepository { get; }
        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();
    }
}