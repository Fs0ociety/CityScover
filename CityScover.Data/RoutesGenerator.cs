//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale
// File update: 20/09/2018
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
      public static void GenerateRoutes(IEnumerable<InterestPoint> points, ushort pointsCount)
      {
         _filename = _rootDirectory + Path.DirectorySeparatorChar.ToString() + "cityscover-routes-" + pointsCount + ".xml";

         if (File.Exists(_filename))
         {
            return;
         }

         int routeId = default;
         XmlWriterSettings settings = new XmlWriterSettings
         {
            Indent = true
         };

         using (XmlWriter writer = XmlWriter.Create(_filename, settings))
         {
            writer.WriteStartElement("Routes");

            for (int i = 1; i <= pointsCount; i++)
            {
               for (int j = i + 1; j <= pointsCount + i; j++)
               {
                  if ((j - i) == i)
                  {
                     continue;
                  }

                  writer.WriteStartElement("Route");
                  writer.WriteAttributeString("id", (++routeId).ToString());

                  writer.WriteStartElement("PointFrom");
                  writer.WriteAttributeString("id", i.ToString());
                  writer.WriteEndElement();

                  writer.WriteStartElement("PointTo");
                  writer.WriteAttributeString("id", (j - i).ToString());
                  writer.WriteEndElement();

                  writer.WriteStartElement("Distance");
                  writer.WriteAttributeString("unitOfMeasure", 4.ToString());

                  ComputeDistance(i, (j - i), out string distance);

                  // TODO: Set the appropriate measure unit code depending on value of distance.

                  writer.WriteAttributeString("value", distance);
                  writer.WriteEndElement();
                  writer.WriteEndElement();
               }
            }
            writer.WriteEndElement();
         }

         void ComputeDistance(int indexPointFrom, int indexPointTo, out string distance)
         {
            var pointFrom = points.Where(point => point.Id == indexPointFrom).FirstOrDefault();
            var pointTo = points.Where(point => point.Id == indexPointTo).FirstOrDefault();

            Location locationFrom = new Location(pointFrom.Latitude, pointFrom.Longitude);
            Location locationTo = new Location(pointTo.Latitude, pointTo.Longitude);

            Distance pointsDistance = locationFrom.DistanceBetween(locationTo, DistanceUnits.Kilometers);

            distance = (pointsDistance.Value * 1000 > 1000) 
               ? pointsDistance.Value.ToString() 
               : (pointsDistance.Value * 1000).ToString();
         }
      }
      #endregion
   }
}
