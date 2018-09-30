using System;

namespace CityScover.Commons
{
   /// <summary>
   /// DoubleExtensions utility class for comparing double values with different Epsilon tolerances.
   /// </summary>
   public static class DoubleExtensions
   {
      const double _tolerance_3digits = 0.001;
      const double _tolerance_4digits = 0.0001;
      const double _tolerance_5digits = 0.00001;
      const double _tolerance_6digits = 0.000001;
      const double _tolerance_7digits = 0.0000001;

      public static bool Equals3DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) < _tolerance_3digits;
      }

      public static bool Equals4DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) < _tolerance_4digits;
      }

      public static bool Equals5DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) < _tolerance_5digits;
      }

      public static bool Equals6DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) < _tolerance_6digits;
      }

      public static bool Equals7DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) < _tolerance_7digits;
      }

      public static bool IsGreather3DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) > _tolerance_3digits;
      }

      public static bool IsGreather4DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) > _tolerance_4digits;
      }

      public static bool IsGreather5DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) > _tolerance_5digits;
      }

      public static bool IsGreather6DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) > _tolerance_6digits;
      }

      public static bool IsGreather7DigitPrecision(this double left, double right)
      {
         return Math.Abs(left - right) > _tolerance_7digits;
      }
   }
}
