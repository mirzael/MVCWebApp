using NUnit.Framework;
using OrderWebApplication.Controllers;
using OrderWebApplication.Models;
using OrderWebApplication.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Web.Http.Results;

namespace OrderWebApplication.Tests
{
    [TestFixture]
    public class OrderWebApiControllerTests
    {
        public IUnitOfWork inMemoryUnitOfWork;
        public OrderWebApiController controller;

        List<Item> _mockItemData;
        List<Order> _mockOrderData;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _mockItemData = new List<Item> {
                new Item{
                    ID = 1,
                    Name = "Item1",
                    Price = 10.99M
                },
                new Item{
                    ID = 2,
                    Name = "Item2",
                    Price = 10.12M
                },
            };

            _mockOrderData = new List<Order> {
                new Order{ 
                    ID = 1,
                    Address = "Ad", 
                    ItemID = _mockItemData[0].ID, 
                    OrdererName = "Alphonse", 
                    Quantity = 2, 
                    ShippingType = ShippingType.Express, 
                    TimeOrdered = new DateTime(2011,11,11),
                    Item = _mockItemData[0]
                },
                new Order{
                    ID = 2,
                    Address = "Mine", 
                    ItemID = _mockItemData[1].ID, 
                    OrdererName = "Bobby", 
                    Quantity = 2, 
                    ShippingType = ShippingType.Express, 
                    TimeOrdered = new DateTime(2012,12,12),
                    Item = _mockItemData[1]
                }
            };


            inMemoryUnitOfWork = new InMemoryUnitOfWork();

            controller = new OrderWebApiController(inMemoryUnitOfWork);
        }

        [SetUp]
        public void initializeUoW()
        {
            var items = inMemoryUnitOfWork.ItemRepository.Get().ToList();

            for (int i = 0; i < items.Count; i++)
            {
                inMemoryUnitOfWork.ItemRepository.Delete(items[i].ID);
            }

            var orders = inMemoryUnitOfWork.OrderRepository.Get().ToList();

            for (int i = 0; i < orders.Count; i++)
            {
                inMemoryUnitOfWork.OrderRepository.Delete(orders[i].ID);
            }


            inMemoryUnitOfWork.ItemRepository.Insert(_mockItemData[0]);
            inMemoryUnitOfWork.ItemRepository.Insert(_mockItemData[1]);
            inMemoryUnitOfWork.OrderRepository.Insert(_mockOrderData[0]);
            inMemoryUnitOfWork.OrderRepository.Insert(_mockOrderData[1]);
        }

        [Test]
        public void PostOrder_AddsNewOrder()
        {
            var order = new Order
            {
                Address = "124 St",
                ID = 5,
                Item = _mockItemData[0],
                ItemID = _mockItemData[0].ID,
                OrdererName = "Bo bo bo",
                Quantity = 10,
                ShippingType = ShippingType.Express,
                TimeOrdered = DateTime.Now
            };

            controller.PostOrder(order);

            inMemoryUnitOfWork.OrderRepository.GetById(order.ID).Should().Be(order);
        }

        [Test]
        public void DeleteOrder_DeletesOrder()
        {
            controller.DeleteOrder(1);

            inMemoryUnitOfWork.OrderRepository.Get().Should().BeEquivalentTo(new List<Order>(){ _mockOrderData[1] });
        }

        [Test]
        public void GetOrders_GetsAllOrders()
        {
            var orders = controller.GetOrders();

            orders.Should().Equal(_mockOrderData);
        }

        [Test]
        public void GetOrder_ReturnsProperOrder()
        {
            var order = controller.GetOrder(1) as OkNegotiatedContentResult<Order>;

            order.Content.Should().Be(_mockOrderData[0]);
        }

        [Test]
        public void PutOrder_EditsProperOrder()
        {
            var order = _mockOrderData[0];
            order.Address = "New Address";

            controller.PutOrder(1, order);

            inMemoryUnitOfWork.OrderRepository.GetById(1).Address.Should().Be(order.Address);
        }
    }
}
