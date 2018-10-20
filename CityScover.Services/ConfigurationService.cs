//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 18/10/2018
//

using CityScover.Commons;
using CityScover.Engine;
using CityScover.Engine.Configs;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Console;

namespace CityScover.Services
{
   /*
    * Online sources
    * 
    * How to elegantly check if a number is within a range?
    * https://stackoverflow.com/questions/3188672/how-to-elegantly-check-if-a-number-is-within-a-range
    * 
    * How do I identify if a string is a number?
    * https://stackoverflow.com/questions/894263/how-do-i-identify-if-a-string-is-a-number
    * 
    * Procedura: determinare se una stringa rappresenta un valore numerico
    * https://docs.microsoft.com/it-it/dotnet/csharp/programming-guide/strings/how-to-determine-whether-a-string-represents-a-numeric-value
    * 
    * Strip seconds from datetime
    * https://stackoverflow.com/questions/31578289/strip-seconds-from-datetime
    */

   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      #region Constructors
      private ConfigurationService()
      {
      }
      #endregion

      #region Private methods
      private void DisplayConfiguration(Configuration configuration)
      {
         WriteLine("\t============================================================");
         WriteLine("\t GENERAL CONFIGURATION INFORMATIONS\n");
         WriteLine($"\t Tour category:          \"{configuration.TourCategory}\"");
         WriteLine($"\t Starting point:         \"{configuration.StartingPointId} (Hotel Carlton)\"");
         WriteLine($"\t Problem size:           \"{configuration.PointsCount} points of interest\"");
         WriteLine($"\t Tourist walking speed:  \"{configuration.WalkingSpeed * 3.6} Km/h\"");
         WriteLine($"\t Arrival time:           \"{configuration.ArrivalTime.ToString("g")}\"");
         WriteLine($"\t Tour duration:          \"{configuration.TourDuration.Hours} hours\"");
         WriteLine($"\t Algorithm monitoring:   \"{configuration.AlgorithmMonitoring}\"");
         WriteLine("\t============================================================");
         WriteLine("\t CONFIGURATION'S STAGES\n");

         foreach (var stage in configuration.Stages)
         {
            WriteLine($"\t     Description:         \"{stage.Description}\"");
            WriteLine($"\t     Algorithm family:    \"{stage.Category}\"");
            WriteLine($"\t     Current algorithm:   \"{stage.Flow.CurrentAlgorithm}\"");
            WriteLine($"\t     Max iterations:      \"{stage.Flow.RunningCount}\"");
            WriteLine($"\t     Deadlock condition:  \"{stage.Flow.MaximumDeadlockIterations}\"");
            if (!stage.Flow.ChildrenFlows.Any())
            {
               WriteLine("\t    --------------------------------------------------------");
            }
            else
            {
               WriteLine($"\n\t     \"{stage.Flow.CurrentAlgorithm} inner algorithms\"");
               foreach (var childrenFlow in stage.Flow.ChildrenFlows)
               {
                  WriteLine("\t     {");
                  WriteLine($"\t        Current algorithm:   \"{childrenFlow.CurrentAlgorithm}\"");
                  WriteLine($"\t        Max iterations:      \"{childrenFlow.RunningCount}\"");
                  WriteLine($"\t        Deadlock condition:  \"{childrenFlow.MaximumDeadlockIterations}\"");
                  WriteLine("\t     }");
                  WriteLine("\t    --------------------------------------------------------");
               }
            }
         }
         WriteLine("\t============================================================\n");

         string choice = string.Empty;
         bool canProceed = default;

         do
         {
            Write("Do you want to run this configuration? [y/N]: ");
            choice = ReadLine().Trim().ToLower();
            canProceed = choice == "y" || choice == "n";

            if (!canProceed)
            {
               WriteLine($"String \"{choice}\" is not valid. Enter \"y\" or \"N\".\n");
            }
         } while (!canProceed);

         if (choice == "y")
         {
            WriteLine("Execution of configuration in progress...");
            ISolverService solverService = SolverService.Instance;
            solverService.Run(configuration);
         }
      }

      private void ShowAvailableConfigurationMenu()
      {
         string configChoice = string.Empty;
         bool canProceed = default;
         int choiceValue = default;

         while (true)
         {
            do
            {
               WriteLine();
               WriteLine("*************************************************************************");
               WriteLine("                         AVAILABLE CONFIGURATIONS                        ");
               WriteLine("*************************************************************************");
               WriteLine();

               for (int confIndex = 0; confIndex < Configurations.Count; ++confIndex)
               {
                  WriteLine($"> Configuration {confIndex + 1}");
               }
               WriteLine($"> Back [Press \"Enter\" key]\n");

               Write("Enter the configuration to display: ");
               configChoice = ReadLine();

               if (configChoice == string.Empty)
               {
                  return;
               }
               canProceed = int.TryParse(configChoice, out choiceValue) &&
                  Enumerable.Range(1, Configurations.Count).Contains(choiceValue);
               if (!canProceed)
               {
                  WriteLine($"Enter a value between {1} - {Configurations.Count}.\n");
               }
            } while (!canProceed);

            DisplayConfiguration(Configurations.ElementAt(--choiceValue));
         }
      }

      private void ShowCustomConfigurationMenu()
      {
         string choice = string.Empty;
         int choiceValue = default;
         bool canProceed = default;
         bool settingsCompleted = default;

         do
         {
            WriteLine();
            WriteLine("*************************************************************************");
            WriteLine("                      NEW CONFIGURATION'S SETTINGS                       ");
            WriteLine("*************************************************************************");
            WriteLine();
            WriteLine("<1> Set Problem size");
            WriteLine("<2> Set Tour Category");
            WriteLine("<3> Set Walking Speed");
            WriteLine("<4> Set Arrival Time");
            WriteLine("<5> Set Tour duration");
            WriteLine("<6> Set Algorithm parameters");
            WriteLine("<7> Set Algorithm's stages");
            WriteLine("<8> Back");
            Write("Select an option: ");
            choice = ReadLine().Trim();
            canProceed = int.TryParse(choice, out choiceValue)
               && choiceValue >= 1 && choiceValue <= 7;

            if (!canProceed)
            {
               WriteLine($"Insert a value between 1 and 8.\n");
            }
         } while (!canProceed);

         object[] configurationParams = default;
         int problemSize = default;
         TourCategoryType tourCategory = default;
         double walkingSpeed = default;
         DateTime arrivalTime = default;
         TimeSpan tourDuration = default;
         bool algorithmMonitoring = default;
         Collection<Stage> stages = new Collection<Stage>();
         settingsCompleted = problemSize > 0 &&
            tourCategory != TourCategoryType.None &&
            walkingSpeed > 0.0;
         canProceed = settingsCompleted;

         switch (choiceValue)
         {
            case 1:
               problemSize = GetProblemSize();
               break;
            case 2:
               tourCategory = GetTourCategory();
               break;
            case 3:
               walkingSpeed = GetWalkingSpeed();
               break;
            case 4:

               break;
            case 5:
               break;
            case 6:
               break;
            case 7:
               break;
            default:
               break;
         }

         if (problemSize < 0)
         {
            return;
         }

         if (tourCategory == TourCategoryType.None)
         {
            return;
         }
      }

      private TourCategoryType GetTourCategory()
      {
         TourCategoryType tourCategory = default;
         string choice = string.Empty;
         bool canProceed = default;

         do
         {
            WriteLine("\n-----> TOUR CATEGORY <-----\n");
            WriteLine("<1> Historical and Cultural");
            WriteLine("<2> Culinary");
            WriteLine("<3> Sport");
            WriteLine("<4> Go Back\n");

            Write("Select the Tour category: ");
            choice = ReadLine().Trim();
            canProceed = int.TryParse(choice, out _);

            switch (choice)
            {
               case "1":
                  tourCategory = TourCategoryType.HistoricalAndCultural;
                  break;
               case "2":
                  tourCategory = TourCategoryType.Culinary;
                  break;
               case "3":
                  tourCategory = TourCategoryType.Sport;
                  break;
               case "4":
                  tourCategory = TourCategoryType.None;
                  break;
            }
         } while (!canProceed);

         return tourCategory;
      }

      private static int GetProblemSize()
      {
         string choice = string.Empty;
         bool canProceed = default;
         int nodesCount = default;

         do
         {
            WriteLine("\n-----> PROBLEM SIZE <-----\n");
            WriteLine("<1> 15 nodes");
            WriteLine("<2> 20 nodes");
            WriteLine("<3> 30 nodes");
            WriteLine("<4> 45 nodes");
            WriteLine("<5> 60 nodes");
            WriteLine("<6> 75 nodes");
            WriteLine("<7> 90 nodes");
            WriteLine("<8> Over 100 nodes");
            WriteLine("<9> Go Back\n");

            Write("Enter a problem size [number of nodes]: ");
            choice = ReadLine().Trim();
            canProceed = int.TryParse(choice, out _);

            switch (choice)
            {
               case "1":
                  nodesCount = 15;
                  break;
               case "2":
                  nodesCount = 20;
                  break;
               case "3":
                  nodesCount = 30;
                  break;
               case "4":
                  nodesCount = 45;
                  break;
               case "5":
                  nodesCount = 60;
                  break;
               case "6":
                  nodesCount = 75;
                  break;
               case "7":
                  nodesCount = 90;
                  break;
               case "8":
                  Write("Insert a custom number of nodes: ");
                  choice = ReadLine().Trim();
                  nodesCount = int.Parse(choice);
                  break;
               case "9":
                  nodesCount = -1;
                  break;

               default:
                  canProceed = false;
                  WriteLine("Enter a value between [1-8].\n");
                  break;
            }
         } while (!canProceed);

         return nodesCount;
      }

      private double GetWalkingSpeed()
      {
         string choice = string.Empty;
         bool canProceed = default;
         double walkingSpeed = default;

         do
         {
            WriteLine("\n-----> WALKING SPEED <-----\n");
            Write("Enter the \"walking speed\" of the tourist in km/h. Valid range [1 - 10]: ");
            // Inserire opzione per ritornare indietro nel menu.
            choice = ReadLine().Trim();
            canProceed = double.TryParse(choice, out walkingSpeed) && walkingSpeed >= 1 && walkingSpeed <= 10;
            if (!canProceed)
            {
               WriteLine($"Invalid speed value. Valid range is [1 - 10] \n");
            }
         } while (!canProceed);

         return walkingSpeed;
      }

      private DateTime GetArrivalTime()
      {
         string choice = string.Empty;
         bool canProceed = default;

         throw new NotImplementedException();
      }

      private TimeSpan GetTourDuration()
      {
         string choice = string.Empty;
         bool canProceed = default;

         throw new NotImplementedException();
      }

      private bool GetAlgorithmMonitoring()
      {
         string choice = string.Empty;
         bool canProceed = default;

         throw new NotImplementedException();
      }

      private ICollection<Stage> GetStages()
      {
         string choice = string.Empty;
         bool canProceed = default;

         throw new NotImplementedException();
      }
      #endregion

      #region IConfigurationService implementation
      public ICollection<Configuration> Configurations => RunningConfigs.Configurations;

      public void ShowConfigurationsMenu()
      {
         WriteLine();
         WriteLine("*************************************************************************");
         WriteLine("                         CONFIGURATIONS MENU                             ");
         WriteLine("*************************************************************************");
         WriteLine();

         string choice = string.Empty;
         bool canProceed = default;
         bool isExiting = default;

         while (true)
         {
            do
            {
               WriteLine("Choose a following option:\n");
               WriteLine("<1> Run a configuration from those available.");
               WriteLine("<2> Create a custom configuration.");
               WriteLine("<3> Exit.\n");
               Write("Enter a choice: ");
               choice = ReadLine().Trim();
               canProceed = int.TryParse(choice, out _);

               switch (choice)
               {
                  case "1":
                     ShowAvailableConfigurationMenu();
                     break;

                  case "2":
                     ShowCustomConfigurationMenu();
                     break;

                  case "3":
                     isExiting = true;
                     break;

                  default:
                     canProceed = false;
                     WriteLine("Enter a value between [1-3].\n");
                     break;
               }
            } while (!canProceed);

            if (isExiting)
            {
               break;
            }
         }
      }

      public Configuration CreateConfiguration(params object[] configParams)
      {
         throw new System.NotImplementedException();
      }
      #endregion
   }
}
