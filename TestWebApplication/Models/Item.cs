using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using OrderWebApplication.Repository;

namespace OrderWebApplication.Models
{
    public class Item : EntityWithId
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>
        /// The name of the item.
        /// </value>
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price of the item.
        /// </summary>
        /// <value>
        /// The price of the item.
        /// </value>
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        /// <summary>
        /// All orders tied to this item.
        /// </summary>
        /// <value>
        /// The orders that reference this item.
        /// </value>
        public virtual List<Order> Orders { get; set; }
    }
}