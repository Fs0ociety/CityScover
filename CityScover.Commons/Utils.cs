//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 25/11/2018
//


namespace CityScover.Commons
{
   /// <summary>
   /// Utility class available for all CityScover projects.
   /// </summary>
   public static class Utils
   {
      public const int DelayTask = 250;
      public const string TMaxConstraint = "TMax";
      public const string TimeWindowsConstraint = "Time Windows";
      public const double ObjectiveFunctionWeightDefault = 1.0;

      /// <summary>
      /// Analogo al metodo di estensione ufficiale di Microsoft contenuto nel namespace System.Linq
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source"></param>
      /// <returns></returns>
      //public static int Count<T>(this IEnumerable<T> source)
      //{
      //   if (source == null)
      //   {
      //      throw new ArgumentNullException(nameof(source));
      //   }

      //   ICollection<T> c = source as ICollection<T>;
      //   if (c != null)
      //      return c.Count;

      //   int result = 0;
      //   using (IEnumerator<T> enumerator = source.GetEnumerator())
      //   {
      //      while (enumerator.MoveNext())
      //         result++;
      //   }
      //   return result;
      //}

      //public static int? ToNullableInt(this string s)
      //{
      //   if (int.TryParse(s, out int i))
      //   {
      //      return i;
      //   }
      //   return null;
      //}

      //public static double? ToNullableDouble(this string s)
      //{
      //   if (double.TryParse(s, out double i))
      //   {
      //      return i;
      //   }
      //   return null;
      //}
   }
}
