//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 17/08/2018
//

using CityScover.Engine;
using CityScover.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CityScover.Services
{
   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      #region IConfigurationService implementation
      public IEnumerable<string> ReadConfigurationPath(string configsPath)
      {
         if (configsPath == null)
         {
            throw new ArgumentNullException(nameof(configsPath));
         }

         if (!Directory.Exists(configsPath))
         {
            throw new DirectoryNotFoundException(nameof(configsPath));
         }

         return Directory.GetFiles(configsPath);
      }

      public Configuration ReadConfigurationFromXml(string filename)
      {
         XmlDocument document = new XmlDocument();
         document.Load(filename);
         Configuration conf = new Configuration();

         foreach (XmlNode node in document.GetElementsByTagName("Configuration"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element)
               {
                  continue;
               }

               switch (childNode.Name)
               {
                  case "Stages":
                     ReadConfigStages();
                     break;

                  case "TourCategory":
                     int tourCategoryId = int.Parse(childNode.Attributes["id"].Value);
                     conf.TourCategoryId = tourCategoryId;
                     break;

                  case "ArrivalTime":
                     string arrivalTime = childNode.Attributes["value"].Value;
                     conf.Arrivaltime = TimeSpan.Parse(arrivalTime);
                     break;

                  case "TourDuration":
                     string tourDuration = childNode.Attributes["value"].Value;
                     conf.TourDuration = new TimeSpan(int.Parse(tourDuration), 0, 0);
                     break;

                  default:
                     throw new Exception(nameof(childNode.Name));
               }

               void ReadConfigStages()
               {
                  foreach (XmlNode nestedChild in childNode.ChildNodes)
                  {
                     int stageId = int.Parse(nestedChild.Attributes["id"].Value);
                     int algorithmId = 0;
                     StageType stage = StageType.GetStageById(stageId);

                     foreach (XmlNode nestedNode in nestedChild.ChildNodes)
                     {
                        algorithmId = int.Parse(nestedNode.Attributes["id"].Value);
                     }

                     AlgorithmType algorithm = StageType.GetAlgorithmTypeById(algorithmId);
                     stage.CurrentAlgorithm = algorithm;
                     conf.AddStage(stage);
                  }
               }
            }
         }

         return conf;
      }
      #endregion
   }
}
