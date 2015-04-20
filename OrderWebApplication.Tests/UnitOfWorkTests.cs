using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using OrderWebApplication.Repository;

namespace OrderWebApplication.Tests
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        [Test]
        public void RemoveWarning()
        {
            unitOfWork.OrderRepository.Get();
        }


    }
}
