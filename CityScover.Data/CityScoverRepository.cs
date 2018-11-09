//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/11/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace CityScover.Data
{
   public static class CityScoverRepository
   {
      private static readonly ICollection<MeasureUnit> _measureUnits;
      private static readonly ICollection<InterestPoint> _points;
      private static readonly ICollection<Route> _routes;
      private static readonly string _rootDirectory;

      #region Static Constructors
      static CityScoverRepository()
      {
         _rootDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
         _rootDirectory = Path.Combine(_rootDirectory, "CityScover.Data");

         _measureUnits = new Collection<MeasureUnit>();
         _points = new Collection<InterestPoint>();
         _routes = new Collection<Route>();

         InitializeMeasureUnits();
      }
      #endregion

      #region Private static methods
      private static void InitializeMeasureUnits()
      {
         XmlDocument document = new XmlDocument();
         document.Load(typeof(CityScoverRepository).Assembly
            .GetManifestResourceStream("CityScover.Data.cityscover-measures.xml"));

         foreach (XmlNode node in document.GetElementsByTagName("measureunits"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element || !childNode.Name.Equals("measureunit"))
               {
                  continue;
               }

               string measureUnitCode = childNode.Attributes["code"].Value;
               string measureUnitSymbol = childNode.Attributes["symbol"].Value;
               measureUnitCode = (!string.Empty.Equals(measureUnitCode)) ? measureUnitCode : null;
               measureUnitSymbol = (!string.Empty.Equals(measureUnitSymbol)) ? measureUnitSymbol : null;
               _measureUnits.Add(new MeasureUnit(measureUnitCode, measureUnitSymbol));
            }
         }
      }

      private static void InitializePoints(XmlDocument document, string filename)
      {
         document.Load(typeof(CityScoverRepository).Assembly
            .GetManifestResourceStream("CityScover.Data." + filename));

         foreach (XmlNode node in document.GetElementsByTagName("PointOfInterests"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element || !childNode.Name.Equals("PointOfInterest"))
               {
                  continue;
               }

               string pointId = childNode.Attributes["id"].Value;
               string pointName = childNode.Attributes["name"].Value;
               int intPointId = int.Parse(pointId);
               var pointBuilder = InterestPointBuilder.NewBuilder(intPointId, pointName);

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  switch (nestedChild.Name)
                  {
                     case "Coordinates":
                        SetCoordinates();

                        void SetCoordinates()
                        {
                           string strLatitude = nestedChild.Attributes["latitude"].Value;
                           string strLongitude = nestedChild.Attributes["longitude"].Value;

                           bool success = double.TryParse(strLatitude, out double latitude);
                           success &= double.TryParse(strLongitude, out double longitude);

                           if (!success)
                           {
                              throw new FormatException(nameof(SetCoordinates));
                           }
                           pointBuilder.SetCoordinates(latitude, longitude);
                        }
                        break;
                     
                     case "Category":
                        SetCategory();

                        void SetCategory()
                        {
                           string categoryId = nestedChild.Attributes["id"].Value;
                           TourCategory category = default;
                           switch (categoryId)
                           {
                              case "1":
                                 category = new TourCategory(TourCategoryType.HistoricalAndCultural, "Storico/Culturale");
                                 break;

                              case "2":
                                 category = new TourCategory(TourCategoryType.Culinary, "Gastronomico");
                                 break;

                              case "3":
                                 category = new TourCategory(TourCategoryType.Sport, "Sportivo");
                                 break;

                              default:
                                 category = new TourCategory(TourCategoryType.None, "None");
                                 break;
                           }
                           pointBuilder.SetCategory(category);
                        }
                        break;

                     case "ThematicScore":
                        SetThematicScore();

                        void SetThematicScore()
                        {
                           string scoreValue = nestedChild.Attributes["value"].Value;
                           pointBuilder.SetThematicScore(new ThematicScore(
                              pointBuilder.Category, (!string.Empty.Equals(scoreValue)) ? int.Parse(scoreValue) : 0));
                        }
                        break;

                     case "OpeningTimes":
                        SetOpeningTimes();

                        void SetOpeningTimes()
                        {
                           foreach (XmlNode doubleNestedChild in nestedChild.ChildNodes)
                           {
                              if (doubleNestedChild.NodeType != XmlNodeType.Element)
                              {
                                 continue;
                              }

                              string openingTime = doubleNestedChild.Attributes["from"].Value;
                              string closingTime = doubleNestedChild.Attributes["to"].Value;

                              if (!string.Empty.Equals(openingTime) && !string.Empty.Equals(closingTime))
                              {
                                 try
                                 {
                                    pointBuilder.AddOpeningTime(new IntervalTime(DateTime.Parse(openingTime), DateTime.Parse(closingTime)));
                                 }
                                 catch (FormatException exception)
                                 {
                                    throw new FormatException(exception.Message);
                                 }
                              }
                           }
                        }
                        break;

                     case "TimeVisit":
                        SetTimeVisit();

                        void SetTimeVisit()
                        {
                           string measureUnit = nestedChild.Attributes["unit"].Value;
                           string duration = nestedChild.Attributes["duration"].Value;
                           pointBuilder.SetTimeVisit((!string.Empty.Equals(duration)) ? new TimeSpan(0, int.Parse(duration), 0) : (TimeSpan?)null);
                        }
                        break;

                     default:
                        break;
                  }
               }
               _points.Add(pointBuilder.Build());
            }
         }
      }

      private static void InitializeRoutes(XmlDocument document, IEnumerable<InterestPoint> points)
      {
         string filename = _rootDirectory + Path.DirectorySeparatorChar.ToString() + 
            "cityscover-routes-" + points.Count() + ".xml";
         document.Load(filename);

         foreach (XmlNode node in document.GetElementsByTagName("Routes"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element || !childNode.Name.Equals("Route"))
               {
                  continue;
               }

               string pointId = default;
               double distance = default;
               InterestPoint pointFrom = default;
               InterestPoint pointTo = default;
               string xmlRouteId = childNode.Attributes["id"].Value;
               int routeId = ConvertAttributeId(xmlRouteId);

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  switch (nestedChild.Name)
                  {
                     case "PointFrom":
                        pointId = nestedChild.Attributes["id"].Value;
                        routeId = ConvertAttributeId(pointId);
                        pointFrom = (from p in points where p.Id == routeId select p).FirstOrDefault();
                        break;

                     case "PointTo":
                        pointId = nestedChild.Attributes["id"].Value;
                        routeId = ConvertAttributeId(pointId);
                        pointTo = (from p in points where p.Id == routeId select p).FirstOrDefault();
                        break;

                     case "Distance":
                        var strDistance = nestedChild.Attributes["value"].Value;
                        bool success = double.TryParse(strDistance, out double fDistance);
                        if (!success)
                        {
                           throw new FormatException(nameof(distance));
                        }
                        distance = fDistance;
                        break;

                     default:
                        break;
                  }
               }
               _routes.Add(new Route(routeId, pointFrom, pointTo, distance));
            }
         }

         int ConvertAttributeId(string id)
         {
            bool success = int.TryParse(id, out int result);
            if (!success)
            {
               throw new FormatException(nameof(id));
            }
            return result;
         }
      }
      #endregion

      #region Public static properties
      public static IEnumerable<MeasureUnit> MeasureUnits => _measureUnits;
      public static IEnumerable<InterestPoint> Points => _points;
      public static IEnumerable<Route> Routes => _routes;
      #endregion

      #region Public static methods
      public static void LoadPoints(string filename)
      {
         if (string.IsNullOrEmpty(filename))
         {
            throw new ArgumentNullException(nameof(filename));
         }
         if (_points.Any())
         {
            _points.Clear();
         }

         XmlDocument document = new XmlDocument();
         InitializePoints(document, filename);
      }

      public static void LoadRoutes(IEnumerable<InterestPoint> filteredPoints)
      {
         if (!filteredPoints.Any())
         {
            return;
         }
         if (_routes.Any())
         {
            _routes.Clear();
         }

         XmlDocument document = new XmlDocument();
         InitializeRoutes(document, filteredPoints);
      }
      #endregion
   }
}