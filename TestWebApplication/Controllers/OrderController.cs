using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OrderWebApplication.Models;
using OrderWebApplication.Repository;
using System.Linq.Expressions;
using OrderWebApplication.Models.ViewModels;

namespace OrderWebApplication.Controllers
{
    public class OrderController : Controller
    {
        private IUnitOfWork unitOfWork;
        private const int UNITS_PER_PAGE = 3;

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Shows all of the Orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(unitOfWork.OrderRepository.Get().ToPagedList(1, UNITS_PER_PAGE));
        }

        /// <summary>
        /// The partial page that shows all of the orders.
        /// </summary>
        /// <param name="ordererName">Name of the orderer.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="currentOrderer">The current orderer.</param>
        /// <param name="currentItemId">The current item identifier.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="page">The page.</param>
        /// <param name="currentSort">The current sort order.</param>
        /// <returns></returns>
        public PartialViewResult _orderDetails(string ordererName, int? itemId, string currentOrderer, int? currentItemId, string sortOrder, int? page, string currentSort)
        {
            //If there is a new filter, set the page back to one - the amount of pages may have changed
            if (!string.IsNullOrEmpty(ordererName) || itemId.HasValue)
            {
                page = 1;
            }
            else
            {
                ordererName = currentOrderer;
                itemId = currentItemId;
            }

            //Adding params to the ViewBag in order for the view to reference

            //If the sortOrder is empty, set the name sort param to descending - otherwise keep it empty
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            //If the sort order is by Date, then set it to descending, because we want the opposite if they click it
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            //The list of items that are in the database are needed when computing the total cost of the orders
            ViewBag.ItemID = new SelectList(unitOfWork.ItemRepository.Get(), "ID", "Name");

            //Store the current sorting order for the view
            ViewBag.CurrentOrdererFilter = ordererName;
            ViewBag.CurrentItemIdFilter = itemId;
            ViewBag.CurrentSort = sortOrder;

            Expression<Func<Order, bool>> filter = o => true;

            //If we are filtering by orderer name, then filter it by that name
            if (!string.IsNullOrEmpty(ordererName))
            {
                filter = HelperMethods.AndCombineExpressions<Order>(filter, o => o.OrdererName.Contains(ordererName));
            }

            //If we are filtering by item id, then filter it by ID
            if (itemId.HasValue)
            {
                filter = HelperMethods.AndCombineExpressions<Order>(filter, o => o.ItemID == itemId.Value);
            }

            IEnumerable<Order> orders;

            //Sort the list that we are showing - By default sort by orderer name
            switch (sortOrder)
            {
                case "name_desc":
                    orders = unitOfWork.OrderRepository.Get(filter, query => query.OrderByDescending(o => o.OrdererName), "Item");
                    break;
                case "Date":
                    orders = unitOfWork.OrderRepository.Get(filter, query => query.OrderBy(o => o.TimeOrdered), "Item");
                    break;
                case "date_desc":
                    orders = unitOfWork.OrderRepository.Get(filter, query => query.OrderByDescending(o => o.TimeOrdered), "Item");
                    break;
                default:
                    orders = unitOfWork.OrderRepository.Get(filter, query => query.OrderBy(o => o.OrdererName), "Item");
                    break;
            }

            //If there isn't a page number, default to the first page
            int pageNumber = (page ?? 1);

            return PartialView(orders.ToPagedList(pageNumber, UNITS_PER_PAGE));
        }

        // GET: /Order/Details/5        
        /// <summary>
        /// Details the order with the= specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the order.</param>
        /// <returns></returns>
        [Route("Order/{id:int}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = unitOfWork.OrderRepository.Get(includeProperties: "Item", filter: o => o.ID == id.Value).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: /Order/Create        
        /// <summary>
        /// Displays the page to create a new order.
        /// </summary>
        /// <returns></returns>
        [Authorize()]
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(unitOfWork.ItemRepository.Get(), "ID", "Name");
            return View();
        }

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.        
        /// <summary>
        /// Creates the specified order.
        /// </summary>
        /// <param name="order">The order to create.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public ActionResult Create([Bind(Include="ID,TimeOrdered,Address,OrdererName,Quantity,ShippingType,ItemID")] Order order)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.OrderRepository.Insert(order);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.ItemID = new SelectList(unitOfWork.ItemRepository.Get(), "ID", "Name", order.ItemID);
            return View(order);
        }

        // GET: /Order/Edit/5        
        /// <summary>
        /// Edits the Order with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the Order.</param>
        /// <returns></returns>
        [Authorize()]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = unitOfWork.OrderRepository.GetById(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemID = new SelectList(unitOfWork.ItemRepository.Get(), "ID", "Name", order.ItemID);
            return View(order);
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.        
        /// <summary>
        /// Edits the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public ActionResult Edit([Bind(Include="ID,TimeOrdered,Address,OrdererName,Quantity,ShippingType,ItemID")] Order order)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.OrderRepository.Edit(order);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(unitOfWork.ItemRepository.Get(), "ID", "Name", order.ItemID);
            return View(order);
        }

        // GET: /Order/Delete/5        
        /// <summary>
        /// Displays the page to delete the page with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the order.</param>
        /// <returns></returns>
        [Authorize()]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = unitOfWork.OrderRepository.GetById(id.Value);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: /Order/Delete/5        
        /// <summary>
        /// Deletes the order with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public ActionResult DeleteConfirmed(int id)
        {
            unitOfWork.OrderRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [Authorize()]
        public ActionResult ShowRevenue()
        {
            var orders = unitOfWork.OrderRepository.Get(includeProperties: "Item", orderBy: s => s.OrderBy(o => o.TimeOrdered));
            var revenueDates = new List<RevenueDay>();

            foreach (Order ord in orders)
            {
                var revenue = ord.Quantity * ord.Item.Price;
                var revDay = revenueDates.Find(r => r.Date == ord.TimeOrdered);
                
                if(revDay == null){
                    revDay = new RevenueDay { Date = ord.TimeOrdered, TotalRevenue = 0M };
                    revenueDates.Add(revDay);
                }

                revDay.TotalRevenue += revenue;
            }
            return View(revenueDates);
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
