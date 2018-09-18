//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 18/08/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Entities
{
   public struct InterestPoint
   {
      #region Constructors
      internal InterestPoint(int id, string name, TourCategory category, ThematicScore score, TimeSpan? timeVisit)
      {
         Id = id;
         Name = name;
         Category = category;
         Score = score;
         TimeVisit = timeVisit;
         OpeningTimes = new Collection<IntervalTime>();
      } 
      #endregion

      #region Public properties
      /// <summary>
      /// Identificativo univoco del punto d'interesse.
      /// </summary>
      public int Id { get; }

      /// <summary>
      /// Descrizione generica del punto d'interesse.
      /// </summary>
      public string Name { get; private set; }

      /// <summary>
      /// Categoria di appartenenza del punto d'interesse.
      /// </summary>
      public TourCategory Category { get; }
      
      /// <summary>
      /// Punteggio tematico assegnato al punto d'interesse.
      /// </summary>
      public ThematicScore Score { get; }
      
      /// <summary>
      /// Orari di apertura del punto d'interesse.
      /// </summary>
      public ICollection<IntervalTime> OpeningTimes { get; private set; }
      
      /// <summary>
      /// Tempo necessario per visitare il punto d'interesse.
      /// </summary>
      public TimeSpan? TimeVisit { get; }
      #endregion

      #region Public methods
      public InterestPoint DeepCopy()
      {
         InterestPoint copy = (InterestPoint)MemberwiseClone();
         copy.Name = string.Copy(Name);
         copy.OpeningTimes = new Collection<IntervalTime>();
         foreach (var openingTime in OpeningTimes)
         {
            copy.OpeningTimes.Add(openingTime.DeepCopy());
         }
         return copy;
      } 
      #endregion
   }

   /// <summary>
   /// Builder pattern
   /// </summary>
   public class InterestPointBuilder
   {
      private int _id;
      private string _name;
      private TourCategory _category;
      private ThematicScore _score;
      private ICollection<IntervalTime> _openingTimes;
      private TimeSpan? _timeVisit;

      #region Constructors
      private InterestPointBuilder(int id, string name)
      {
         _id = id;
         _name = name;
         _openingTimes = new Collection<IntervalTime>();
      }

      public static InterestPointBuilder newBuilder(int id, string name)
      {
         return new InterestPointBuilder(id, name);
      }
      #endregion

      #region Public properties
      public TourCategory Category { get { return _category; } } 
      #endregion

      #region Public methods
      public InterestPointBuilder SetName(string name)
      {
         _name = name;
         return this;
      }

      public InterestPointBuilder SetCategory(TourCategory category)
      {
         _category = category;
         return this;
      }

      public InterestPointBuilder SetThematicScore(ThematicScore score)
      {
         _score = score;
         return this;
      }

      public void AddOpeningTime(IntervalTime time)
      {
         _openingTimes.Add(time);
      }

      public InterestPointBuilder SetTimeVisit(TimeSpan? timeVisit)
      {
         _timeVisit = timeVisit;
         return this;
      }
      
      public InterestPoint Build()
      {
         var newPoint = new InterestPoint(_id, _name, _category, _score, _timeVisit);
         foreach (var openingTime in _openingTimes)
         {
            newPoint.OpeningTimes.Add(openingTime);
         }
         return newPoint;
      }
      #endregion
   }
}