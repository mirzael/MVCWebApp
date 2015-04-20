using NUnit.Framework;
using Moq;
using System;
using OrderWebApplication.Repository;
using OrderWebApplication.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using OrderWebApplication.Controllers;
using System.Web.Mvc;
using FluentAssertions;

namespace OrderWebApplication.Tests
{
    [TestFixture]
    public class OrderControllerTests
    {
        public IUnitOfWork inMemoryUnitOfWork;
        public OrderController controller;

        List<Item> _mockItemData;
        List<Order> _mockOrderData;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _mockItemData= new List<Item> {
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

            inMemoryUnitOfWork.ItemRepository.Insert(_mockItemData[0]);
            inMemoryUnitOfWork.ItemRepository.Insert(_mockItemData[1]);
            inMemoryUnitOfWork.OrderRepository.Insert(_mockOrderData[0]);
            inMemoryUnitOfWork.OrderRepository.Insert(_mockOrderData[1]);

            controller = new OrderController(inMemoryUnitOfWork);
        }

        [Test]
        public void Index_HasRightUrl()
        {
            var result = (PartialViewResult)controller._orderDetails(null, null, null, null, null, null, null);

            result.ViewName.Should().BeEquivalentTo(string.Empty);
        }

        [Test]
        public void Index_ReturnsFullListOfOrders()
        {
            var result = (PartialViewResult)controller._orderDetails(null, null, null, null, null, null, null);

            var orders = (IEnumerable<Order>)result.ViewData.Model;
            orders.Should().Equal(_mockOrderData);
        }

        [Test]
        public void Index_WithOrdererNameFilter_ShouldFilterByOrdererName()
        {
            var result = (PartialViewResult)controller._orderDetails("Bobby", null, null, null, null, null, null);

            var orders = (IEnumerable<Order>)result.ViewData.Model;
            orders.Should().Equal(new List<Order> { _mockOrderData[1] });
        }

        [Test]
        public void Index_WithItemIdFilter_ShouldFilterByItemId()
        {
            var result = (PartialViewResult)controller._orderDetails(null, 1, null, null, null, null, null);

            var orders = (IEnumerable<Order>)result.ViewData.Model;
            orders.Should().Equal(new List<Order> { _mockOrderData[0] });
        }

        [Test]
        public void Index_WithSortOrderOrdererNameDesc_WillSortByOrdererNameDesc()
        {
            var result = (PartialViewResult)controller._orderDetails(null, null, null, null, "name_desc", null, null);

            var reversedSet = new List<Order>(_mockOrderData);
            reversedSet.Reverse();

            var orders = (IEnumerable<Order>)result.ViewData.Model;
            orders.Should().Equal(reversedSet);
        }

        [Test]
        public void Index_WithSortOrderTimeOrderedDesc_WillSortByTimeOrderedDesc()
        {
            var result = (PartialViewResult)controller._orderDetails(null, null, null, null, "date_desc", null, null);

            var reversedSet = new List<Order>(_mockOrderData);
            reversedSet.Reverse();

            var orders = (IEnumerable<Order>)result.ViewData.Model;
            orders.Should().Equal(reversedSet);
        }

        [Test]
        public void Details_WithOrderId_WillReturnOrderWithOrderId()
        {
            var result = (ViewResult)controller.Details(1);

            var order = (Order)result.ViewData.Model;

            order.Should().Be(_mockOrderData[0]);
        }

        [Test]
        public void Edit_WithOrderId_WillReturnOrderWithOrderId()
        {
            var result = (ViewResult)controller.Edit(1);

            var order = (Order)result.ViewData.Model;

            order.Should().Be(_mockOrderData[0]);
        }

        [Test]
        public void DeleteConfirmed_WithItemId_WillDeleteItem()
        {
            var result = (RedirectToRouteResult)controller.DeleteConfirmed(1);

            inMemoryUnitOfWork.OrderRepository.Get().Should().Equal(new List<Order> { _mockOrderData[1] });            
            
            inMemoryUnitOfWork.OrderRepository.Insert(_mockOrderData[0]);
        }
    }
}
