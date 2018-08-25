//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 24/08/2018
//

using CityScover.Engine;
using CityScover.Entities;
using CityScover.Utils;
using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace CityScover.Services
{
   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      #region Constructors
      private ConfigurationService()
      {
      }
      #endregion

      #region IConfigurationService implementation
      public Configuration ReadConfigurationFromXml(string filename)
      {
         XmlDocument document = new XmlDocument();
         document.Load(filename);
         Configuration conf = new Configuration();
         conf.Stages = new Collection<Stage>();
         conf.RelaxedConstraintsId = new Collection<byte>();

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
                  case "PointsCount":
                     SetPointCount();

                     void SetPointCount()
                     {
                        string pointsCountXml = childNode.Attributes["value"].Value;
                        if (!ushort.TryParse(pointsCountXml, out ushort pointsCount))
                        {
                           throw new FormatException(nameof(pointsCountXml));
                        }

                        conf.PointsCount = pointsCount;
                     }
                     break;

                  case "Stages":
                     ReadConfigStages();
                     break;

                  case "TourCategory":
                     SetTourCategoryId();

                     void SetTourCategoryId()
                     {
                        string tourCategoryIdXml = childNode.Attributes["id"].Value;
                        if (!byte.TryParse(tourCategoryIdXml, out byte tourCategoryId))
                        {
                           throw new FormatException(nameof(tourCategoryIdXml));
                        }

                        switch (tourCategoryId)
                        {
                           case 1:
                              conf.TourCategory = TourCategoryType.HistoricalAndCultural;
                              break;
                           case 2:
                              conf.TourCategory = TourCategoryType.Culinary;
                              break;
                           case 3:
                              conf.TourCategory = TourCategoryType.Sport;
                              break;
                           default:
                              break;
                        }
                     }
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
                     Stage stage = Stage.GetStageById(stageId);

                     foreach (XmlNode nestedNode in nestedChild.ChildNodes)
                     {
                        algorithmId = int.Parse(nestedNode.Attributes["id"].Value);
                     }

                     AlgorithmType algorithm = Stage.GetAlgorithmTypeById(algorithmId);
                     stage.CurrentAlgorithm = algorithm;
                     conf.Stages.Add(stage);
                  }
               }
            }
         }
         return conf;
      }
      #endregion
   }
}
