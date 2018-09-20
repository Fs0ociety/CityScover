//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 20/08/2018
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityScover.Entities
{
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

      public static InterestPointBuilder NewBuilder(int id, string name)
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
