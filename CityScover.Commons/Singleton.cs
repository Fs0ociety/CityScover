//
// Ariel
// Version 5.0
// 
// Written by: Riccardo Mariotti
// File update: 19/07/2017
//

using System;
using System.Linq;
using System.Reflection;

namespace CityScover.Commons
{
   /// <summary>
   /// Abstract implementation of singleton pattern.
   /// </summary>
   /// <typeparam name="TInstance">Type of instance.</typeparam>
   public abstract class Singleton<TInstance>
   {
      private static TInstance _instance;
      private static object _syncObject = new object();

      #region Public static properties
      /// <summary>
      /// Get the singleton instance.
      /// </summary>
      public static TInstance Instance
      {
         get
         {
            if (_instance == null)
            {
               lock (_syncObject)
               {
                  if (_instance == null)
                  {
                     ConstructorInfo c = typeof(TInstance)
                        .GetTypeInfo()
                        .DeclaredConstructors
                        .Single(ci => ci.GetParameters().Length == 0);
                     _instance = (TInstance)c.Invoke(Type.EmptyTypes);
                     (_instance as Singleton<TInstance>).InitializeInstance();
                  }
               }
            }
            return _instance;
         }
      }
      #endregion

      #region Protected methods
      protected virtual void InitializeInstance()
      {
      }
      #endregion
   }
}
