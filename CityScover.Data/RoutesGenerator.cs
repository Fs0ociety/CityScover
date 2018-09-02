//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale
// File update: 02/09/2018
//

using CityScover.Entities;
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
         int ptsCount = points.Count();
         XmlWriterSettings settings = new XmlWriterSettings
         {
            Indent = true
         };

         using (XmlWriter writer = XmlWriter.Create(_filename, settings))
         {
            writer.WriteStartElement("Routes");

            for (int i = 1; i <= ptsCount; i++)
            {
               for (int j = i + 1; j <= ptsCount + i; j++)
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
                  writer.WriteAttributeString("value", 200.ToString());
                  writer.WriteEndElement();
                  writer.WriteEndElement();
               }
            }
            writer.WriteEndElement();
         }
      }
      #endregion
   }
}
