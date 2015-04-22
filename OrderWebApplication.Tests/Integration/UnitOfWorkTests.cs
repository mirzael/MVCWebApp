using NUnit.Framework;
using OrderWebApplication.Models;
using OrderWebApplication.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using System.Data.Entity;

namespace OrderWebApplication.Tests.Integration
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        public List<Item> workItems;
        public List<Order> workOrders;

        [TestFixtureSetUp]
        public void SetUp()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
            Database.SetInitializer(new DropCreateDatabaseAlways<OrderDBContext>());

            var unitOfWork = new UnitOfWork();
            workItems = new List<Item>();
            workOrders = new List<Order>();

            for (int i = 1; i <= 3; i++) {
                workItems.Add(new Item { ID = i, Name = string.Format("Item{0}", i), Price = 10-(decimal)i });

                workOrders.Add(new Order
                {
                    ID = i,
                    Address = string.Format("Street{0}", i),
                    TimeOrdered = new DateTime(2012,12,12).AddDays(-i),
                    ShippingType = ShippingType.Express,
                    Quantity = i,
                    OrdererName = string.Format("Orderer{0}", i),
                    ItemID = i,
                });
            }

            foreach (Item i in workItems)
            {
                unitOfWork.ItemRepository.Insert(i);
            }

            unitOfWork.Save();

            foreach (Order o in workOrders)
            {
                unitOfWork.OrderRepository.Insert(o);
            }

            unitOfWork.Save();


            foreach (Item i in workItems)
            {
                i.Orders = null;
            }

        }


        [Test]
        public void ItemRepositoryInsert_AddsNewItem()
        {
            var item = new Item
            {
                ID = 0,
                Name = "Woot",
                Price = 12.99M
            };

            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    unitOfWork.ItemRepository.Insert(item);
                    unitOfWork.Save();

                    var items = unitOfWork.ItemRepository.Get(i => i.Name == item.Name).ToList();

                    items.Should().Contain(item);

                    trans.Rollback();
                }
            }

        }

        [Test]
        public void ItemRepositoryEdit_EditsItem()
        {
            string expectedText = "Wambo";
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var item = unitOfWork.ItemRepository.GetById(1);

                    item.Name = expectedText;

                    unitOfWork.ItemRepository.Edit(item);

                    var expected = unitOfWork.ItemRepository.GetById(1);

                    expected.Name.Should().Be(expectedText);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ItemRepositoryDelete_DeletesItem()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    unitOfWork.ItemRepository.Delete(1);
                    unitOfWork.Save();

                    var item = unitOfWork.ItemRepository.GetById(1);

                    item.Should().BeNull();

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ItemRepositoryGet_ReturnsAllItems()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    List<Item> items = unitOfWork.ItemRepository.Get().ToList();

                    items.Should().Equal(workItems, (i1, i2) => i1.ID == i2.ID && i1.Name == i2.Name && i1.Price == i2.Price);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ItemRepositoryGetWithFilter_FiltersItems()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var items = unitOfWork.ItemRepository.Get(i => i.Name == "Item1").ToList();

                    items.Should().Equal(new List<Item> { workItems[0] }, (i1, i2) => i1.ID == i2.ID && i1.Name == i2.Name && i1.Price == i2.Price);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ItemRepositoryGetWithOrder_OrdersItems()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var items = unitOfWork.ItemRepository.Get(orderBy: o => o.OrderBy(i => i.Price)).ToList();

                    var revList = new List<Item>(workItems);
                    revList.Reverse();

                    items.Should().Equal(revList, (i1, i2) => i1.ID == i2.ID && i1.Name == i2.Name && i1.Price == i2.Price);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void ItemRepositoryGetById_ReturnsItem()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var item = unitOfWork.ItemRepository.GetById(workItems[0].ID);

                    item.Should().Match<Item>(i => i.Name == workItems[0].Name && i.ID == workItems[0].ID && i.Price == workItems[0].Price);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryInsert_AddsNewOrder()
        {
            var Order = new Order
            {
                ID = 0,
                Address = "Woot St",
                OrdererName = "Woot Jr",
                Quantity = 2,
                ShippingType = Models.ShippingType.Express,
                TimeOrdered = new DateTime(2011,11,11),
                ItemID = 1, 
            };

            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    unitOfWork.OrderRepository.Insert(Order);
                    unitOfWork.Save();

                    var Orders = unitOfWork.OrderRepository.Get(i => i.OrdererName == Order.OrdererName).ToList();

                    Orders.Should().Contain(Order);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryEdit_EditsOrder()
        {
            string expectedText = "Wambo";
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var Order = unitOfWork.OrderRepository.GetById(1);

                    Order.OrdererName = expectedText;

                    unitOfWork.OrderRepository.Edit(Order);

                    var expected = unitOfWork.OrderRepository.GetById(1);

                    expected.OrdererName.Should().Be(expectedText);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryDelete_DeletesOrder()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    unitOfWork.OrderRepository.Delete(1);
                    unitOfWork.Save();

                    var Order = unitOfWork.OrderRepository.GetById(1);

                    Order.Should().BeNull();

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryGet_ReturnsAllOrders()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    List<Order> Orders = unitOfWork.OrderRepository.Get(includeProperties: "Item").ToList();

                    Orders.Should().Equal(workOrders, (i1, i2) => 
                        i1.ID == i2.ID 
                        && i1.ItemID == i2.ItemID 
                        && i1.OrdererName == i2.OrdererName
                        && i1.Quantity == i2.Quantity
                        && i1.ShippingType == i2.ShippingType
                        && i1.Address == i2.Address
                        && i1.Item.ID == i2.Item.ID
                        && i1.Item.Price == i2.Item.Price
                        && i1.Item.Name == i2.Item.Name);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryGetWithFilter_FiltersOrders()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var Orders = unitOfWork.OrderRepository.Get(i => i.OrdererName == "Orderer1", includeProperties: "Item").ToList();

                    Orders.Should().Equal(new List<Order> { workOrders[0] }, (i1, i2) =>
                        i1.ID == i2.ID
                        && i1.ItemID == i2.ItemID
                        && i1.OrdererName == i2.OrdererName
                        && i1.Quantity == i2.Quantity
                        && i1.ShippingType == i2.ShippingType
                        && i1.Address == i2.Address
                        && i1.Item.ID == i2.Item.ID
                        && i1.Item.Price == i2.Item.Price
                        && i1.Item.Name == i2.Item.Name);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryGetWithOrder_OrdersOrders()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var Orders = unitOfWork.OrderRepository.Get(orderBy: o => o.OrderBy(i => i.TimeOrdered), includeProperties: "Item").ToList();

                    var revList = new List<Order>(workOrders);
                    revList.Reverse();

                    Orders.Should().Equal(revList, (i1, i2) =>
                        i1.ID == i2.ID
                        && i1.ItemID == i2.ItemID
                        && i1.OrdererName == i2.OrdererName
                        && i1.Quantity == i2.Quantity
                        && i1.ShippingType == i2.ShippingType
                        && i1.Address == i2.Address
                        && i1.Item.ID == i2.Item.ID
                        && i1.Item.Price == i2.Item.Price
                        && i1.Item.Name == i2.Item.Name);

                    trans.Rollback();
                }
            }
        }

        [Test]
        public void OrderRepositoryGetById_ReturnsOrder()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var trans = unitOfWork.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var Order = unitOfWork.OrderRepository.GetById(1);

                    Order.Should().Match<Order>(i1 =>
                        i1.ID == workOrders[0].ID
                        && i1.ItemID == workOrders[0].ItemID
                        && i1.OrdererName == workOrders[0].OrdererName
                        && i1.Quantity == workOrders[0].Quantity
                        && i1.ShippingType == workOrders[0].ShippingType
                        && i1.Address == workOrders[0].Address);

                    trans.Rollback();
                }
            }
        }
    }
}
