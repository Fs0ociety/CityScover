using System;
using System.Collections.Generic;

namespace CityScover.Utils
{
   /// <summary>
   /// Classe che contiene metodi di estensione in supporto ai tipi nativi di .NET Framework
   /// </summary>
   public static class CollectionHelper
   {
      /// <summary>
      /// Analogo al metodo di estensione ufficiale di Microsoft contenuto nel namespace System.Linq
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="source"></param>
      /// <returns></returns>
      public static int Count<T>(this IEnumerable<T> source)
      {
         if (source == null)
         {
            throw new ArgumentNullException(nameof(source));
         }

         ICollection<T> c = source as ICollection<T>;
         if (c != null)
            return c.Count;

         int result = 0;
         using (IEnumerator<T> enumerator = source.GetEnumerator())
         {
            while (enumerator.MoveNext())
               result++;
         }
         return result;
      }
   }
}
