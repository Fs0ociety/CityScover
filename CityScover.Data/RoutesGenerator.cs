//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale
// File update: 24/09/2018
//

using CityScover.Entities;
using Geocoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace CityScover.Data
{
   public static class RoutesGenerator
   {
      private static string _rootDirectory;
      private static string _filename;

      #region Static Constructors
      static RoutesGenerator()
      {
         _rootDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
         _rootDirectory = Path.Combine(_rootDirectory, "CityScover.Data");
      }
      #endregion

      #region Public static methods
      public static void GenerateRoutes(IEnumerable<InterestPoint> points, int pointsCount)
      {
         _filename = _rootDirectory + Path.DirectorySeparatorChar.ToString() + "cityscover-routes-" + pointsCount + ".xml";

         if (File.Exists(_filename))
         {
            return;
         }

         int routeId = default;
         var measureUnits = CityScoverRepository.MeasureUnits;
         XmlWriterSettings settings = new XmlWriterSettings
         {
            Indent = true
         };

         using (XmlWriter writer = XmlWriter.Create(_filename, settings))
         {
            writer.WriteStartElement("Routes");

            for (int i = 1; i <= pointsCount; i++)
            {
               var pointFromId = points.ElementAt(i - 1).Id;

               for (int j = i + 1; j <= pointsCount + i; j++)
               {
                  if ((j - i) == i)
                  {
                     continue;
                  }

                  var pointToId = points.ElementAt(j - i - 1).Id;

                  writer.WriteStartElement("Route");
                  writer.WriteAttributeString("id", (++routeId).ToString());

                  writer.WriteStartElement("PointFrom");
                  writer.WriteAttributeString("id", pointFromId.ToString());
                  writer.WriteEndElement();

                  writer.WriteStartElement("PointTo");
                  writer.WriteAttributeString("id", pointToId.ToString());
                  writer.WriteEndElement();

                  writer.WriteStartElement("Distance");
                  ComputeDistance(pointFromId, pointToId, out double distance, out MeasureUnit measureUnit);
                  writer.WriteAttributeString("unit", measureUnit.Code);
                  writer.WriteAttributeString("value", distance.ToString());
                  writer.WriteEndElement();

                  // Close "Route" tag
                  writer.WriteEndElement();
               }
            }
            writer.WriteEndElement();
         }

         void ComputeDistance(int pointFromId, int pointToId, out double distance, out MeasureUnit measureUnit)
         {
            var pointFrom = points.Where(point => point.Id == pointFromId).FirstOrDefault();
            var pointTo = points.Where(point => point.Id == pointToId).FirstOrDefault();

            Location locationFrom = new Location(pointFrom.Latitude, pointFrom.Longitude);
            Location locationTo = new Location(pointTo.Latitude, pointTo.Longitude);

            Distance pointsDistance = locationFrom.DistanceBetween(locationTo, DistanceUnits.Kilometers);

            distance = pointsDistance.Value * 1000.0;
            measureUnit = measureUnits.Where(code => code.Code.Equals("m")).FirstOrDefault();
         }
      }
      #endregion
   }
}
