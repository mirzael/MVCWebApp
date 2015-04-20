using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrderWebApplication.Models;
using OrderWebApplication.Repository;

namespace OrderWebApplication.Controllers
{
    public class ItemController : Controller
    {
       private IUnitOfWork unitOfWork;

        public ItemController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ItemController() : this(new UnitOfWork()){ }

        // GET: /Item/        
        /// <summary>
        /// Displays all Items.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(unitOfWork.ItemRepository.Get());
        }

        // GET: /Item/Details/5        
        /// <summary>
        /// Details the item with the specified identifier.
        /// </summary> 
        /// <param name="id">The identifier of the item.</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = unitOfWork.ItemRepository.GetById(id.Value);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: /Item/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.        
        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,Price")] Item item)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.ItemRepository.Insert(item);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        // GET: /Item/Edit/5        
        /// <summary>
        /// Edits the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the Item.</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = unitOfWork.ItemRepository.GetById(id.Value);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.        
        /// <summary>
        /// Edits the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,Price")] Item item)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.ItemRepository.Edit(item);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: /Item/Delete/5        
        /// <summary>
        /// Displays a page to confirm the deletion the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = unitOfWork.ItemRepository.GetById(id.Value);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Item/Delete/5        
        /// <summary>
        /// Deletes the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the item.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unitOfWork.ItemRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");
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
