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
         double value1 = 0.333;
         double value2 = 1.0d / 3.0d;
         bool result = value1.Equals3DigitPrecision(value2);
         Assert.AreNotEqual(value1, value2);
         Assert.IsTrue(result == true);
         result = value1.IsGreather3DigitPrecision(value2);
         Assert.IsTrue(result == false);
         result = value2.IsGreather4DigitPrecision(value1);
         Assert.IsTrue(result == true);
      }
   }
}
