using Microsoft.VisualStudio.TestTools.UnitTesting;
using RainbowWine.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UnitTestRainBowwine
{
    public class OrdersControllerTest
    {

        [TestMethod]
        public void Index()
        {
            // Arrange
            OrdersController ordersController = new OrdersController();
            // Act
            ActionResult actionResult = ordersController.SetApprovedCashFree() as ActionResult;
            // Assert
            Assert.IsNotNull(actionResult);
        }
       
    }
}
