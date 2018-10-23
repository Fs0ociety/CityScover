//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 23/10/2018
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
    * 
    * How to parse a string into a nullable int
    * https://stackoverflow.com/questions/45030/how-to-parse-a-string-into-a-nullable-int
    * 
    * How to use int.TryParse with nullable int? [duplicate]
    * https://stackoverflow.com/questions/3390750/how-to-use-int-tryparse-with-nullable-int/3390929
    */

   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      private readonly int _maxStagesCount = 3;

      #region Constructors
      private ConfigurationService()
      {
         _maxStagesCount = 3;
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

      #region Existing Configurations menu
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
      #endregion

      private void ShowCustomConfigurationMenu()
      {
         string choice = string.Empty;
         int choiceValue = default;
         bool canProceed = default;
         bool settingsCompleted = default;
         bool isExiting = false;

         WriteLine();
         WriteLine("*************************************************************************");
         WriteLine("                      NEW CONFIGURATION'S SETTINGS                       ");
         WriteLine("*************************************************************************");
         WriteLine();

         while (!isExiting)
         {
            do
            {
               WriteLine("<1> Set Problem size");
               WriteLine("<2> Set Tour Category");
               WriteLine("<3> Set Walking Speed");
               WriteLine("<4> Set Arrival Time");
               WriteLine("<5> Set Tour duration");
               WriteLine("<6> Set Algorithm monitoring");
               WriteLine("<7> Set Algorithm's stages");
               WriteLine("<8> Back\n");
               Write("Select an option: ");
               choice = ReadLine().Trim();
               canProceed = int.TryParse(choice, out choiceValue)
                  && choiceValue >= 1 && choiceValue <= 8;

               if (!canProceed)
               {
                  WriteLine($"Insert a value between 1 and 8.\n");
               }
            } while (!canProceed);

            object[] configurationParams = default;
            /* TODO
             * inserire i seguenti campi della configurazione come campi privati di classe 
             * per la successiva creazione della configurazione
             */
            int? problemSize = default;
            TourCategoryType tourCategory = default;
            double? walkingSpeed = default;
            DateTime? arrivalTime = default;
            TimeSpan? tourDuration = default;
            bool algorithmMonitoring = default;
            Collection<Stage> stages = new Collection<Stage>();

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
                  arrivalTime = GetArrivalTime();
                  break;
               case 5:
                  tourDuration = GetTourDuration();
                  break;
               case 6:
                  algorithmMonitoring = GetAlgorithmMonitoring();
                  break;
               case 7:
                  stages = GetAlgorithmStages();
                  break;
               case 8:
                  isExiting = true;
                  break;
               default:
                  break;
            }

            // ... TODO ...
         }
      }

      #region Problem Size menu
      private static int? GetProblemSize()
      {
         string choice = string.Empty;
         bool canProceed = default;
         int? nodesCount = default;

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
            WriteLine("<9> Back\n");

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
                  break;
               default:
                  canProceed = false;
                  WriteLine("Enter a value between [1-8].\n");
                  break;
            }
         } while (!canProceed);

         if (nodesCount.HasValue)
         {
            WriteLine($"{nodesCount} nodes set!");
         }

         return nodesCount;
      }
      #endregion

      #region Tour Category menu
      private TourCategoryType GetTourCategory()
      {
         string choice = string.Empty;
         bool canProceed = default;
         TourCategoryType tourCategory = default;

         WriteLine("\n-----> TOUR CATEGORY <-----\n");
         do
         {
            WriteLine("<1> Historical and Cultural");
            WriteLine("<2> Culinary");
            WriteLine("<3> Sport");
            WriteLine("<4> Back\n");

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

         if (tourCategory != TourCategoryType.None)
         {
            WriteLine($"\"{tourCategory}\" tour set!");
         }

         return tourCategory;
      }
      #endregion

      #region Walking Speed menu
      private double? GetWalkingSpeed()
      {
         string walkingSpeedStr = string.Empty;
         bool canProceed = default;
         double? walkingSpeed = default;

         WriteLine("\n-----> WALKING SPEED <-----\n");
         do
         {
            Write("Set the \"walking speed\" of the tourist in km/h. Valid range is [1 - 8]. " +
               "[Press \"Enter\" key to go back.]: ");
            walkingSpeedStr = ReadLine().Trim();
            canProceed = double.TryParse(walkingSpeedStr, out var walkSpeed)
               || walkSpeed >= 1 && walkSpeed <= 10;

            if (walkingSpeedStr == string.Empty)
            {
               break;
            }
            if (canProceed)
            {
               walkingSpeed = walkSpeed;
            }
            else
            {
               WriteLine($"Invalid speed value. Insert the speed without \"km/h\" string. " +
                  $"Valid range is [1 - 10]\n");
            }
         } while (!canProceed);

         if (walkingSpeed.HasValue)
         {
            WriteLine($"\n\"Walking speed\" set to {walkingSpeed.Value} Km/h.");
         }

         return walkingSpeed;
      }
      #endregion

      #region Arrival Time menu
      private DateTime? GetArrivalTime()
      {
         string arrivalTimeStr = string.Empty;
         bool canProceed = default;
         DateTime? arrivalTime = default;

         WriteLine("\n-----> HOTEL ARRIVAL TIME <-----\n");
         do
         {
            Write("Insert the \"Arrival time\" to Hotel in the format (DD/MM/AAAA OR h:m) " +
               "[Press \"Enter\" key to go back.]: ");
            arrivalTimeStr = ReadLine().Trim();
            if (arrivalTimeStr == string.Empty)
            {
               break;
            }
            canProceed = DateTime.TryParse(arrivalTimeStr, out DateTime arrTime)
               && arrTime.Hour > 0;

            if (canProceed)
            {
               arrivalTime = arrTime;
            }
            else
            {
               WriteLine($"Invalid \"Arrival time\". Valid format: [dd/mm/aaaa OR hh:mm]\n");
            }

         } while (!canProceed);

         if (arrivalTime.HasValue)
         {
            WriteLine($"\n\"Arrival\" time at the Hotel set at: {arrivalTime}");
         }

         return arrivalTime;
      }
      #endregion

      #region Tour Duration menu
      private TimeSpan? GetTourDuration()
      {
         string tourDurationStr = string.Empty;
         bool canProceed = default;
         string format = "h\\:mm";
         TimeSpan? tourDuration = default;

         WriteLine("\n-----> TOUR DURATION <-----\n");
         do
         {
            Write("Insert the duration of the tour in hours: " +
               "[Press \"Enter\" key to go back.]\n");
            tourDurationStr = ReadLine().Trim();
            if (tourDurationStr == string.Empty)
            {
               break;
            }

            canProceed = TimeSpan.TryParseExact(tourDurationStr, format, null, out var duration)
               && duration.Hours >= 1;

            if (canProceed)
            {
               tourDuration = duration;
            }
            else
            {
               WriteLine("Invalid \"Tour duration\". The duration must be greater than or equals to 1.");
            }

         } while (!canProceed);

         if (tourDuration.HasValue)
         {
            WriteLine($"\nTour duration set to: {tourDuration.Value.Hours} hours " +
               $"and {tourDuration.Value.Minutes} minutes");
         }

         return tourDuration;
      }
      #endregion

      #region Algorithm Monitoring menu
      private bool GetAlgorithmMonitoring()
      {
         string choice = string.Empty;
         bool canProceed = default;
         bool toMonitoring = default;

         WriteLine("\n-----> ALGORITHM MONITORING <-----\n");
         do
         {
            WriteLine("Do you want to monitor algorithms's executions? [y/N]");
            choice = ReadLine().Trim();
            canProceed = choice == "y" || choice == "Y" ||
               choice == "n" || choice == "N";

            if (canProceed)
            {
               toMonitoring = true;
            }
            else
            {
               WriteLine("Invalid choice. Enter \"Y or y\" or \"N or n\"");
            }
         } while (!canProceed);

         return toMonitoring;
      }
      #endregion

      #region Menu stages
      private Collection<Stage> GetAlgorithmStages()
      {
         Collection<Stage> stages = new Collection<Stage>();
         WriteLine("\n-----> STAGES'S CONFIGURATION <-----\n");

         for (int stageCount = 1; stageCount <= _maxStagesCount; ++stageCount)
         {
            Stage stage = GetStageSettings(stageCount);
            if (stage.Flow.CurrentAlgorithm == AlgorithmType.None)
            {
               break;
            }
            stages.Add(stage);
         }

         return stages;
      }

      private Stage GetStageSettings(int stageId)
      {
         string choice = string.Empty;
         bool canProceed = default;
         Stage stage = new Stage();
         WriteLine($"\n-----> STAGE {stageId} SETTINGS <-----\n");

         if (stageId == 1)
         {
            WriteLine($"Set the Greedy algorithm of stage number {stageId}.\n");
            stage.Description = StageType.StageOne;
            stage.Category = AlgorithmFamily.Greedy;
            AlgorithmType algorithm = GetGreedyAlgorithm();
            stage.Flow.CurrentAlgorithm = algorithm;
         }
         else if (stageId == 2)
         {
            WriteLine($"Set the Local Search algorithm of stage number {stageId}.\n");
            stage.Description = StageType.StageTwo;
            stage.Category = AlgorithmFamily.LocalSearch;
            stage.Flow.CurrentAlgorithm = GetLocalSearchAlgorithm();
            if (stage.Flow.CurrentAlgorithm != AlgorithmType.None)
            {
               var (improvementThreshold, maxIterationsWithoutImprovements) = GetLocalSearchParameters();
               stage.Flow.ImprovementThreshold = improvementThreshold;
               stage.Flow.MaxIterationsWithoutImprovements = maxIterationsWithoutImprovements;
               WriteLine($"Do you want to set an improvement algorithm for stage number {stageId}? [y/N]");
               string response = ReadLine().Trim();
               if (response == "y" || response == "Y")
               {
                  SetInnerAlgorithm(stageId);
               }
            }
         }
         else if (stageId == 3)
         {
            WriteLine($"Set the MetaHeuristic algorithm of stage number {stageId}.\n");
            stage.Description = StageType.StageThree;
            stage.Category = AlgorithmFamily.MetaHeuristic;
         }
         else
         {
            // ...
         }

         return stage;
      }

      private static AlgorithmType GetGreedyAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = default;

         WriteLine("<1> Nearest Neighbor");
         WriteLine("<2> Nearest Neighbor Knapsack");
         WriteLine("<3> Cheapest Insertion");
         WriteLine("<4> Back\n");
         choice = ReadLine().Trim();

         switch (choice)
         {
            case "1":
               algorithm = AlgorithmType.NearestNeighbor;
               break;
            case "2":
               algorithm = AlgorithmType.NearestNeighborKnapsack;
               break;
            case "3":
               algorithm = AlgorithmType.CheapestInsertion;
               break;
            default:
               break;
         }
         return algorithm;
      }

      private AlgorithmType GetLocalSearchAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = default;

         WriteLine("<1> Two Opt");
         WriteLine("<2> Back\n");
         choice = ReadLine().Trim();

         switch (choice)
         {
            case "1":
               algorithm = AlgorithmType.TwoOpt;
               break;
            default:
               break;
         }
         return algorithm;
      }

      private (byte, byte) GetLocalSearchParameters()
      {
         byte maxIterationsWithoutImprovements = default;
         byte improvementThreshold = default;
         bool canProceed = default;

         WriteLine("\n-----> TUNING PARAMETERS <-----\n");
         do
         {
            Write("Set the \"Maximum iterations without improvements\" to trigger the improvement algorithm " +
               "[Range: 1 - 255]: ");
            string firstParam = ReadLine().Trim();
            Write("Set the \"improvement threshold\" " +
               "[Range: 1 - 255]: ");
            string secondParam = ReadLine().Trim();

            canProceed = byte.TryParse(firstParam, out byte firstParamValue);
            canProceed = byte.TryParse(secondParam, out byte secondParamValue);
            canProceed |= firstParamValue > 0 && secondParamValue > 0;

            if (canProceed)
            {
               maxIterationsWithoutImprovements = firstParamValue;
               improvementThreshold = secondParamValue;
            }
            else
            {
               WriteLine("Invalid parameters settings. Valid range: [1 - 255]\n");
            }
         } while (!canProceed);

         return (improvementThreshold, maxIterationsWithoutImprovements);
      }

      private void SetInnerAlgorithm(int stageId)
      {
         // ... TODO ...
      }
      #endregion

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
