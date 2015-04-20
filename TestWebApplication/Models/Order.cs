using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderWebApplication.Repository;


namespace OrderWebApplication.Models
{
    public enum ShippingType { FirstClass, Priority, Express}

    public class Order : EntityWithId
    {
        /// <summary>
        /// Gets or sets the time ordered.
        /// </summary>
        /// <value>
        /// The time ordered.
        /// </value>
        [Display(Name = "Time Ordered"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeOrdered { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [StringLength(60, MinimumLength = 3)]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the name of the orderer.
        /// </summary>
        /// <value>
        /// The name of the orderer.
        /// </value>
        [Display(Name = "Name")]
        public string OrdererName { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the item ordered.
        /// </summary>
        /// <value>
        /// The quantity of the item ordered.
        /// </value>
        [Range(1, 99)]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the type of the shipping.
        /// </summary>
        /// <value>
        /// The type of the shipping.
        /// </value>
        public ShippingType ShippingType { get; set; }

        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        [Display(Name="Item")]
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public Item Item { get; set; }
    }
}