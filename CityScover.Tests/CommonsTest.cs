using CityScover.Commons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityScover.Tests
{
   [TestClass]
   public class CommonsTest
   {
      [TestMethod]
      public void TestDoubleExtensions()
      {
         double value1 = 0.333333;
         double value2 = 1.0d / 3.0d;
         bool result = DoubleExtensions.Equals4DigitPrecision(value1, value2);
         Assert.AreNotEqual(value1, value2);
         Assert.IsTrue(result);
      }
   }
}
