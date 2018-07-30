//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 30/07/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace CityScover.Data
{
   public static class CityScoverRepository
   {
      private static readonly ICollection<MeasureUnit> _measureUnits;
      private static readonly ICollection<InterestPoint> _points;
      private static readonly ICollection<InterestPoint> _routes;

      #region Constructors
      static CityScoverRepository()
      {
         _measureUnits = new Collection<MeasureUnit>();
         _points = new Collection<InterestPoint>();
         //_routes = new Collection<Route>();

         InitializeData();
      }
      #endregion

      #region Private static methods
      private static void InitializeData()
      {
         InitializeMeasureUnits();
         InitializePoints();
         //InitializePoints2();
         //InitializeRoutes();
      }

      public static void InitializeMeasureUnits()
      {
         XmlDocument document = new XmlDocument();
         document.Load(typeof(CityScoverRepository).Assembly.GetManifestResourceStream("CityScover.Data.cityscover-points.xml"));
         // TODO
      }

      private static void InitializePoints()
      {
         XmlDocument document = new XmlDocument();
         document.Load(typeof(CityScoverRepository).Assembly.GetManifestResourceStream("CityScover.Data.cityscover-points.xml"));

         foreach (XmlNode node in document.GetElementsByTagName("PointOfInterests"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element || !childNode.Name.Equals("PointOfInterest"))
               {
                  continue;
               }

               // Create a new InterestPoint entity
               InterestPoint point = new InterestPoint();
               point.Name = childNode.Attributes["name"].Value;

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  switch (nestedChild.Name)
                  {
                     case "Category":
                        SetCategory();

                        void SetCategory()
                        {
                           string categoryId = nestedChild.Attributes["id"].Value;
                           point.Category = new TourCategory
                           {
                              Id = (!categoryId.Equals(string.Empty)) ? int.Parse(categoryId) : (int?)null
                           };

                           switch (point.Category.Id)
                           {
                              case 1:
                                 point.Category.Description = "Storico/Culturale";
                                 break;
                              case 2:
                                 point.Category.Description = "Gastronimico";
                                 break;
                              case 3:
                                 point.Category.Description = "Sportivo";
                                 break;
                              default:
                                 break;
                           }
                        }
                        break;

                     case "ThematicScore":
                        SetThematicScore();

                        void SetThematicScore()
                        {
                           string scoreValue = nestedChild.Attributes["value"].Value;

                           point.Score = new ThematicScore()
                           {
                              Category = point.Category,
                              Value = (!scoreValue.Equals(string.Empty)) ? int.Parse(scoreValue) : (int?)null
                           };
                        }
                        break;

                     case "OpeningTimes":
                        SetOpeningTimes();

                        void SetOpeningTimes()
                        {
                           point.OpeningTimes = new Collection<IntervalTime>();

                           foreach (XmlNode doubleNestedChild in nestedChild.ChildNodes)
                           {
                              if (doubleNestedChild.NodeType != XmlNodeType.Element)
                              {
                                 continue;
                              }

                              IntervalTime intervalTime = new IntervalTime();
                              if (!doubleNestedChild.Attributes["from"].Value.Equals(string.Empty) &&
                                 !doubleNestedChild.Attributes["to"].Value.Equals(string.Empty))
                              {
                                 string openingTime = doubleNestedChild.Attributes["from"].Value;
                                 string closingTime = doubleNestedChild.Attributes["to"].Value;

                                 try
                                 {
                                    intervalTime.OpeningTime = TimeSpan.Parse(openingTime);
                                    intervalTime.ClosingTime = TimeSpan.Parse(closingTime);
                                 }
                                 catch (FormatException exception)
                                 {
                                    throw new FormatException(exception.Message);
                                 }
                              }
                              point.OpeningTimes.Add(intervalTime);
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
                           point.TimeVisit = (!duration.Equals(string.Empty)) ? new TimeSpan(0, int.Parse(duration), 0) : (TimeSpan?)null;
                        }
                        break;

                     default:
                        break;
                  }
               }
               _points.Add(point);
            }
         }
      }

      private static void InitializePoints2()
      {
         XmlReaderSettings settings = new XmlReaderSettings();
         settings.IgnoreComments = true;
         settings.IgnoreComments = true;

         using (XmlReader reader = XmlReader.Create(
            typeof(CityScoverRepository).Assembly.GetManifestResourceStream("CityScover.Data.cityscover-points.xml"), settings))
         {
            while (reader.Read())
            {
               if (reader.NodeType != XmlNodeType.Element || !reader.Name.Equals("PointOfInterests"))
               {
                  continue;
               }

               while (reader.Read())
               {
                  if (reader.NodeType != XmlNodeType.Element || !reader.Name.Equals("PointOfInterest"))
                  {
                     continue;
                  }

                  // Create a new InterestPoint entity
                  InterestPoint point = new InterestPoint();
                  point.Name = reader.GetAttribute("name");

                  while (reader.Read())
                  {
                     switch (reader.Name)
                     {
                        case "Category":
                           break;

                        case "ThematicScore":
                           break;

                        case "OpeningTimes":
                           // TODO
                           break;

                        case "TimeVisit":
                           break;

                        default:
                           break;
                     }
                  }
               }
            }
         }
      }

      private static void InitializeRoutes()
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Public static properties
      public static IEnumerable<MeasureUnit> MeasureUnits => _measureUnits;
      public static IEnumerable<InterestPoint> Points => _points;
      //public static IEnumerable<Route> Routes => _routes;
      #endregion
   }
}
