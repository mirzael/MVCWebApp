using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderWebApplication.Models;
using System.Data;
using System.Data.Entity;

namespace OrderWebApplication.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private OrderDBContext context = new OrderDBContext();
        private IRepository<Order> orderRepository;
        private IRepository<Item> itemRepository;
        private bool disposed = false;

        public UnitOfWork() { }

        /// <summary>
        /// Gets the order repository.
        /// </summary>
        /// <value>
        /// The order repository.
        /// </value>
        public IRepository<Order> OrderRepository 
        { 
            get{
                if (orderRepository == null)
                {
                    orderRepository = new GenericRepository<Order>(context);
                }

                return orderRepository;
            } 
        }

        /// <summary>
        /// Gets the item repository.
        /// </summary>
        /// <value>
        /// The item repository.
        /// </value>
        public IRepository<Item> ItemRepository 
        {
            get
            {
                if (itemRepository == null)
                {
                    itemRepository = new GenericRepository<Item>(context);
                }

                return itemRepository;
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            context.SaveChanges();
        }

        public DbContextTransaction BeginTransaction(IsolationLevel level)
        {
            return context.Database.BeginTransaction(level);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}