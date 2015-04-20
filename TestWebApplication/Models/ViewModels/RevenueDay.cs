using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderWebApplication.Models.ViewModels
{
    public class RevenueDay
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}