using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OrderWebApplication.Models;

namespace OrderWebApplication.Repository
{
    public class OrderDBContext : DbContext
    {
        public OrderDBContext()
        {
            this.Configuration.ProxyCreationEnabled = false; 
        }
        /// <summary>
        /// Gets or sets the orders in the DB.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the items in the DB.
        /// </summary>
        /// <value>
        /// The items in the DB.
        /// </value>
        public DbSet<Item> Items { get; set; }
    }
}