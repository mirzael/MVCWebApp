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
    public class ItemWebApiController : ApiController
    {
        private IUnitOfWork unitOfWork;

        public ItemWebApiController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        // GET: api/ItemWebApi
        public IQueryable<Item> GetItems()
        {
            return unitOfWork.ItemRepository.Get().AsQueryable();
        }

        // GET: api/ItemWebApi/5
        [ResponseType(typeof(Item))]
        public IHttpActionResult GetItem(int id)
        {
            Item item = unitOfWork.ItemRepository.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: api/ItemWebApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutItem(int id, Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.ID)
            {
                return BadRequest();
            }

            unitOfWork.ItemRepository.Insert(item);
            try
            {
                unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
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

        // POST: api/ItemWebApi
        [ResponseType(typeof(Item))]
        public IHttpActionResult PostItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            unitOfWork.ItemRepository.Insert(item);
            unitOfWork.Save();

            return CreatedAtRoute("DefaultApi", new { id = item.ID }, item);
        }

        // DELETE: api/ItemWebApi/5
        [ResponseType(typeof(Item))]
        public IHttpActionResult DeleteItem(int id)
        {
            Item item = unitOfWork.ItemRepository.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            unitOfWork.ItemRepository.Delete(id);
            unitOfWork.Save();

            return Ok(item);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemExists(int id)
        {
            return unitOfWork.OrderRepository.GetById(id) != null;
        }
    }
}