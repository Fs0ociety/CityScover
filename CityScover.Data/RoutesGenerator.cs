//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale
// File update: 17/11/2018
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
      private static readonly string RootDirectory;
      private static string _filename;

      #region Static Constructors
      static RoutesGenerator()
      {
         RootDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
         RootDirectory = Path.Combine(RootDirectory, "CityScover.Data");
      }
      #endregion

      #region Public static methods
      public static void GenerateRoutes(IEnumerable<InterestPoint> points, int pointsCount)
      {
         if (pointsCount == 0)
         {
            return;
         }
         _filename = RootDirectory + Path.DirectorySeparatorChar.ToString() + 
            "cityscover-routes-" + pointsCount + ".xml";

         if (File.Exists(_filename))
         {
            File.Delete(_filename);
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
            var pointFrom = points.FirstOrDefault(point => point.Id == pointFromId);
            var pointTo = points.FirstOrDefault(point => point.Id == pointToId);

            Location locationFrom = new Location(pointFrom.Latitude, pointFrom.Longitude);
            Location locationTo = new Location(pointTo.Latitude, pointTo.Longitude);

            Distance pointsDistance = locationFrom.DistanceBetween(locationTo, DistanceUnits.Kilometers);

            distance = pointsDistance.Value * 1000.0;
            measureUnit = measureUnits.FirstOrDefault(code => code.Code.Equals("m"));
         }
      }
      #endregion
   }
}
