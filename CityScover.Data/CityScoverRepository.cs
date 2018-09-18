//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 19/09/2018
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
         document.Load(typeof(CityScoverRepository).Assembly.GetManifestResourceStream("CityScover.Data.cityscover-measures.xml"));

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

      private static void InitializePoints(XmlDocument document, ushort pointsCount)
      {
         document.Load(typeof(CityScoverRepository).Assembly.GetManifestResourceStream("CityScover.Data.cityscover-points-" + pointsCount + ".xml"));

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
               
               // Create a new entity of InterestPoint
               var pointBuilder = InterestPointBuilder.newBuilder(intPointId, pointName);

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  switch (nestedChild.Name)
                  {
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
                           pointBuilder.SetThematicScore(new ThematicScore(pointBuilder.Category, (!scoreValue.Equals(string.Empty)) ? int.Parse(scoreValue) : 0));                           
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
                                    pointBuilder.AddOpeningTime(new IntervalTime(TimeSpan.Parse(openingTime), TimeSpan.Parse(closingTime)));
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
                           // TODO: Capire come valorizzare l'unita' di misura per il tempo di visita del luogo d'interesse.
                           string measureUnit = nestedChild.Attributes["unitOfMeasure"].Value;
                           string duration = nestedChild.Attributes["duration"].Value;
                           // TODO: controllare unita' di misura del tempo (ore o minuti) e creare l'attributo TimeVisit in modo opportuno.
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

      private static void InitializeRoutes(XmlDocument document, ushort pointsCount)
      {
         string filename = _rootDirectory + Path.DirectorySeparatorChar.ToString() + "cityscover-routes-" + pointsCount + ".xml";
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
               string routeId = childNode.Attributes["id"].Value;
               int intRouteId = ConvertAttributeId(routeId);

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  switch (nestedChild.Name)
                  {
                     case "PointFrom":
                        pointId = nestedChild.Attributes["id"].Value;
                        intRouteId = ConvertAttributeId(pointId);
                        pointFrom = (from p in _points where p.Id == intRouteId select p).FirstOrDefault();                        
                        break;

                     case "PointTo":
                        pointId = nestedChild.Attributes["id"].Value;
                        intRouteId = ConvertAttributeId(pointId);
                        pointTo = (from p in _points where p.Id == intRouteId select p).FirstOrDefault();
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
               _routes.Add(new Route(intRouteId, pointFrom, pointTo, distance));
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
      public static void LoadPoints(ushort pointsCount)
      {
         XmlDocument document = new XmlDocument();
         InitializePoints(document, pointsCount);
      }
      public static void LoadRoutes(ushort pointsCount)
      {
         XmlDocument document = new XmlDocument();
         InitializeRoutes(document, pointsCount);
      }
      #endregion
   }
}
