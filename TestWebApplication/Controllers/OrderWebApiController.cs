using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using OrderWebApplication.Models;
using OrderWebApplication.Repository;

namespace OrderWebApplication.Controllers
{
    public class OrderWebApiController : ApiController
    {
        private IUnitOfWork unitOfWork;

        public OrderWebApiController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public OrderWebApiController() : this(new UnitOfWork()){ }

        // GET: api/OrderWebApi
        public IQueryable<Order> GetOrders()
        {
            return unitOfWork.OrderRepository.Get().AsQueryable();
        }

        // GET: api/OrderWebApi/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/OrderWebApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.ID)
            {
                return BadRequest();
            }

            unitOfWork.OrderRepository.Edit(order);

            try
            {
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/OrderWebApi
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            unitOfWork.OrderRepository.Insert(order);
            unitOfWork.Save();

            return CreatedAtRoute("DefaultApi", new { id = order.ID }, order);
        }

        // DELETE: api/OrderWebApi/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = unitOfWork.OrderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            unitOfWork.OrderRepository.Delete(id);
            unitOfWork.Save();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return unitOfWork.OrderRepository.GetById(id) != null;
        }
    }
}