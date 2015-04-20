using OrderWebApplication.Models;
using OrderWebApplication.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderWebApplication.Repository
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private IRepository<Order> orderRepository;
        private IRepository<Item> itemRepository;

        public IRepository<Order> OrderRepository
        {
            get 
            {
                if (orderRepository == null)
                {
                    orderRepository = new GenericInMemoryRepository<Order>();
                }
                return orderRepository;
            }
        }

        public IRepository<Item> ItemRepository
        {
            get
            {
                if (itemRepository == null)
                {
                    itemRepository = new GenericInMemoryRepository<Item>();
                }
                return itemRepository;
            }
        }

        public void Save()
        {
            ;
        }

        public void Dispose()
        {
            ;
        }
    }
}
