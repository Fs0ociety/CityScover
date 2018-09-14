using CityScover.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityScover.Tests
{
   #region Classes
   public class Algorithm1
   {
      private ICollection<AlgorithmType> _innerAlgorithmTypes;

      public Algorithm1()
      {
         _innerAlgorithmTypes = new Collection<AlgorithmType>() {
            AlgorithmType.NearestInsertion,
            AlgorithmType.TabuSearch
         };
      }

      #region Protected methods
      public AlgorithmType GetInnerAlgorithmType(AlgorithmType algorithmType)
      {
         return (from type in _innerAlgorithmTypes
                 where algorithmType == type
                 select type).FirstOrDefault();
      }
      #endregion
   }

   public class Algorithm2
   {

   }
   #endregion

   [TestClass]
   public class AlgorithmTest
   {
      [TestMethod]
      public void TestMethod1()
      {
         Algorithm1 fatherAlgorithm = new Algorithm1();
         var type = fatherAlgorithm.GetInnerAlgorithmType(AlgorithmType.LinKernighan);
         Assert.IsTrue(type == AlgorithmType.None);
      }
   }
}
