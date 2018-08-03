﻿//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale
// File update: 03/08/2018
//

using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CityScover.Data
{
   public static class RoutesGenerator
   {
      private static string _rootDirectory;
      private static string _filename;

      static RoutesGenerator()
      {
         _rootDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
         _rootDirectory = Path.Combine(_rootDirectory, "CityScover.Data");
      }

      public static void GenerateRoutes(ICollection<InterestPoint> points)
      {
         _filename = _rootDirectory + Path.DirectorySeparatorChar.ToString() + "cityscover-routes.xml";

         if (File.Exists(_filename))
         {
            return;
         }

         int routeId = 0;
         XmlWriterSettings settings = new XmlWriterSettings
         {
            Indent = true
         };

         using (XmlWriter writer = XmlWriter.Create(_filename, settings))
         {
            writer.WriteStartElement("Routes");

            for (int i = 1; i <= points.Count; i++)
            {
               for (int j = i + 1; j <= points.Count + i; j++)
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
                  writer.WriteAttributeString("id", (j-i).ToString());
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
   }
}
