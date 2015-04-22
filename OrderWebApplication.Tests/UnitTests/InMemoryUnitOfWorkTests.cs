using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using OrderWebApplication.Repository;
using OrderWebApplication.Models;

namespace OrderWebApplication.Tests
{
    [TestFixture]
    public class InMemoryUnitOfWorkTests
    {
        public IUnitOfWork unitOfWork;
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
                    Quantity = 1, 
                    ShippingType = ShippingType.Express, 
                    TimeOrdered = new DateTime(2012,12,12),
                    Item = _mockItemData[1]
                }
            };


            unitOfWork = new InMemoryUnitOfWork();
        }

        [SetUp]
        public void SetupData()
        {
            var items = unitOfWork.ItemRepository.Get().ToList();

            for (int i = 0; i < items.Count; i++)
            {
                unitOfWork.ItemRepository.Delete(items[i].ID);
            }

            var orders = unitOfWork.OrderRepository.Get().ToList();

            for (int i = 0; i < orders.Count; i++)
            {
                unitOfWork.OrderRepository.Delete(orders[i].ID);
            }


            unitOfWork.ItemRepository.Insert(_mockItemData[0]);
            unitOfWork.ItemRepository.Insert(_mockItemData[1]);
            unitOfWork.OrderRepository.Insert(_mockOrderData[0]);
            unitOfWork.OrderRepository.Insert(_mockOrderData[1]);
        }

        [Test]
        public void ItemRepositoryGet_GetsAllItems()
        {
            var items = unitOfWork.ItemRepository.Get();
            items.Should().Equal(_mockItemData);
        }

        [Test]
        public void ItemRepositoryGet_WithFilterFiltersItems()
        {
            var items = unitOfWork.ItemRepository.Get(i => i.Name == "Item1");

            items.Should().Equal(new List<Item> { _mockItemData[0] });
        }

        [Test]
        public void ItemRepositoryGet_WithOrderOrdersItems()
        {
            var items = unitOfWork.ItemRepository.Get(orderBy: o => o.OrderBy(i => i.Price));

            var expected = new List<Item>(_mockItemData);
            expected.Reverse();

            items.Should().Equal(expected);
        }

        [Test]
        public void ItemRepositoryEdit_WithItemIdEditsItem()
        {
            var expected = new Item { ID = 1, Price = 10.33M, Name = "EditedItem" };

            unitOfWork.ItemRepository.Edit(expected);

            var result = unitOfWork.ItemRepository.GetById(expected.ID);

            result.Should().Be(expected);
        }

        [Test]
        public void ItemRepositoryInsert_AddsNewItem()
        {
            var expected = new Item { ID = 3, Price = 14M, Name = "New Item" };

            unitOfWork.ItemRepository.Insert(expected);
            unitOfWork.Save();

            var result = unitOfWork.ItemRepository.GetById(3);

            result.Should().Be(expected);
        }

        [Test]
        public void ItemRepositoryDelete_DeletesItem()
        {
            unitOfWork.ItemRepository.Delete(1);

            var result = unitOfWork.ItemRepository.GetById(1);
            result.Should().BeNull();
        }

        [Test]
        public void OrderRepositoryGet_GetsAllOrders()
        {
            var Orders = unitOfWork.OrderRepository.Get();
            Orders.Should().Equal(_mockOrderData);
        }

        [Test]
        public void OrderRepositoryGet_WithFilterFiltersOrders()
        {
            var Orders = unitOfWork.OrderRepository.Get(i => i.OrdererName == "Alphonse");

            Orders.Should().Equal(new List<Order> { _mockOrderData[0] });
        }

        [Test]
        public void OrderRepositoryGet_WithOrderOrdersOrders()
        {
            var Orders = unitOfWork.OrderRepository.Get(orderBy: o => o.OrderBy(i => i.Quantity));

            var expected = new List<Order>(_mockOrderData);
            expected.Reverse();

            Orders.Should().Equal(expected);
        }

        [Test]
        public void OrderRepositoryEdit_WithOrderIdEditsOrder()
        {
            var expected = new Order { ID = 1, Quantity = 3, OrdererName = "Nobody", Address = "Add Road", ShippingType = ShippingType.Express, TimeOrdered = DateTime.Now};

            unitOfWork.OrderRepository.Edit(expected);

            var result = unitOfWork.OrderRepository.GetById(expected.ID);

            result.Should().Be(expected);
        }

        [Test]
        public void OrderRepositoryInsert_AddsNewOrder()
        {
            var expected = new Order { ID = 3, Quantity = 366, OrdererName = "MIA", Address = "Nonya", ShippingType = ShippingType.FirstClass, TimeOrdered = DateTime.Now };

            unitOfWork.OrderRepository.Insert(expected);
            unitOfWork.Save();

            var result = unitOfWork.OrderRepository.GetById(3);

            result.Should().Be(expected);
        }

        [Test]
        public void OrderRepositoryDelete_DeletesOrder()
        {
            unitOfWork.OrderRepository.Delete(1);

            var result = unitOfWork.OrderRepository.GetById(1);
            result.Should().BeNull();
        }
    }
}
