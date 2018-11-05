//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 05/11/2018
//

using CityScover.Commons;
using CityScover.Engine;
using CityScover.Engine.Configs;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Console;

namespace CityScover.Services
{
   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      private readonly int _maxStagesCount;
      private int? _problemSize = default;
      private double? _walkingSpeed = default;
      private DateTime? _arrivalTime = default;
      private TimeSpan? _tourDuration = default;
      private bool? _algorithmMonitoring = default;
      private TourCategoryType _tourCategory = default;
      private Collection<Stage> _stages = new Collection<Stage>();

      #region Constructors
      private ConfigurationService()
      {
         _maxStagesCount = 3;
      }
      #endregion

      #region Private methods

      #region Display Configuration
      private void DisplayConfiguration(Configuration configuration)
      {
         WriteLine("\t============================================================");
         WriteLine("\t GENERAL CONFIGURATION INFORMATIONS\n");
         WriteLine($"\t Tour category:          \"{configuration.TourCategory}\"");
         WriteLine($"\t Starting point:         \"{configuration.StartingPointId} (Hotel Carlton)\"");
         WriteLine($"\t Problem size:           \"{_problemSize?.ToString() ?? Regex.Match(configuration.PointsFilename, @"\d+").Value} points of interest\"");
         WriteLine($"\t Tourist walking speed:  \"{configuration.WalkingSpeed * 3.6} Km/h\"");
         WriteLine($"\t Arrival time:           \"{configuration.ArrivalTime.ToString("g")}\"");
         WriteLine($"\t Tour duration:          \"{configuration.TourDuration.Hours} hours\"");
         WriteLine($"\t Algorithm monitoring:   \"{configuration.AlgorithmMonitoring}\"");
         WriteLine("\t============================================================");
         WriteLine("\t CONFIGURATION'S STAGES\n");

         foreach (var stage in configuration.Stages)
         {
            string stageDescription = string.Empty;
            string description = stage.Description.ToString();
            stageDescription = description.Substring(0, 5);
            stageDescription += " " + description.Substring(5);

            WriteLine($"\t     [{stageDescription.ToUpper()}]\n");
            WriteLine($"\t     Algorithm family:    \"{stage.Category}\"");

            DisplayStageFlow(stage.Flow);
            WriteLine("\t    --------------------------------------------------------\n");
         }
         WriteLine("\t============================================================\n");
      }

      private void DisplayStageFlow(StageFlow flow, string tabulator = "\t")
      {
         WriteLine($"{tabulator}     Current algorithm:   \"{flow.CurrentAlgorithm}\"");
         WriteLine($"{tabulator}     Max iterations:      \"{flow.RunningCount}\"");

         if (flow.MaximumNodesToEvaluate != default)
         {
            WriteLine($"{tabulator}     Maximum nodes to consider:  \"{flow.MaximumNodesToEvaluate}\"");
         }
         if (flow.CanExecuteImprovements != default)
         {
            WriteLine($"{tabulator}     Can {flow.CurrentAlgorithm} executes improvements:  \"{flow.CanExecuteImprovements}\"");
         }
         if (flow.LkImprovementThreshold != default)
         {
            WriteLine($"{tabulator}     Lin Kernighan improvement threshold:  \"{flow.LkImprovementThreshold}\"");
         }
         if (flow.MaxIterationsWithoutImprovements != default)
         {
            WriteLine($"{tabulator}     Maximum iterations without improvement:  \"{flow.MaxIterationsWithoutImprovements}\"");
         }
         if (flow.MaximumDeadlockIterations != default)
         {
            WriteLine($"{tabulator}     Deadlock condition:  \"{flow.MaximumDeadlockIterations}\"");
         }
         if (flow.HndTmaxThreshold != default)
         {
            WriteLine($"{tabulator}     TMax threshold:  \"" +
               $"{flow.HndTmaxThreshold.Hours + ((flow.HndTmaxThreshold.Hours == 1) ? " hour" : " hours")} and " +
               $"{flow.HndTmaxThreshold.Minutes} minutes.\"");
         }

         if (flow.ChildrenFlows.Any())
         {
            WriteLine($"\n{tabulator}     \"{flow.CurrentAlgorithm} inner algorithms\"");
            WriteLine($"{tabulator}     " + "{");
            DisplayStageFlow(flow.ChildrenFlows.FirstOrDefault(), "\t    ");
            WriteLine($"{tabulator}     " + "}");
         }
      }
      #endregion

      #region Run configuration menu
      private async Task RunConfigurationMenu(Configuration configuration)
      {
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
            await solverService.Run(configuration);
         }
      }
      #endregion

      #region Menu available configurations
      private async Task ShowAvailableConfigurationMenu()
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
               configChoice = ReadLine().Trim();

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

            Configuration configuration = Configurations.ElementAt(--choiceValue);
            DisplayConfiguration(configuration);
            await RunConfigurationMenu(configuration);
         }
      }
      #endregion

      #region Menu custom configurations
      private async Task ShowCustomConfigurationMenu()
      {
         string choice = string.Empty;
         int option = default;
         bool canProceed = default;
         bool canCreateConfiguration = default;
         bool isExiting = false;

         WriteLine();
         WriteLine("*************************************************************************");
         WriteLine("                      NEW CONFIGURATION'S SETTINGS                       ");
         WriteLine("*************************************************************************");
         WriteLine();

         while (!isExiting)
         {
            if (IsConfigurationSettingsCompleted())
            {
               canCreateConfiguration = true;
               break;
            }

            do
            {
               WriteLine();
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

               canProceed = int.TryParse(choice, out option)
                  && option >= 1 && option <= 8;

               if (!canProceed)
               {
                  WriteLine($"Insert a value between 1 and 8.\n");
               }
            } while (!canProceed);

            isExiting = SetConfigurationParameter(option);
            await Task.Delay(700).ConfigureAwait(continueOnCapturedContext: false);
         }

         if (canCreateConfiguration)
         {
            WriteLine("\nCreation of new configuration in progress...!\n");
            await Task.Delay(TimeSpan.FromSeconds(1))
               .ConfigureAwait(continueOnCapturedContext: false);
            Configuration config = CreateConfiguration();
            Configurations.Add(config);
            WriteLine("New custom configuration created successfully and added to available configurations!\n");

            do
            {
               WriteLine("<1> Display new configuration settings.");
               WriteLine("<2> Run the newly custom configuration.");
               WriteLine("<3> Back.\n");
               Write("Select an option: ");
               choice = ReadLine().Trim();

               switch (choice)
               {
                  case "1":
                     DisplayConfiguration(config);
                     break;
                  case "2":
                     await RunConfigurationMenu(config);
                     break;
                  case "3":
                     break;
                  default:
                     WriteLine("Selected invalid option. " +
                        "Enter an option between 1 and 3.\n");
                     break;
               }
            } while (choice != "3");
         }
      }

      private bool IsConfigurationSettingsCompleted()
      {
         return _tourCategory != TourCategoryType.None &&
               _problemSize.HasValue && _walkingSpeed.HasValue &&
               _arrivalTime.HasValue && _tourDuration.HasValue &&
               _algorithmMonitoring.HasValue && _stages.Count == _maxStagesCount;
      }

      private bool SetConfigurationParameter(int option)
      {
         bool isExiting = default;

         switch (option)
         {
            case 1:
               _problemSize = GetProblemSize();
               break;
            case 2:
               _tourCategory = GetTourCategory();
               break;
            case 3:
               _walkingSpeed = GetWalkingSpeed();
               break;
            case 4:
               _arrivalTime = GetArrivalTime();
               break;
            case 5:
               _tourDuration = GetTourDuration();
               break;
            case 6:
               _algorithmMonitoring = GetAlgorithmMonitoring();
               break;
            case 7:
               _stages = GetAlgorithmStages();
               break;
            case 8:
               isExiting = true;
               break;
            default:
               break;
         }

         return isExiting;
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
            WriteLine($"\n{nodesCount} nodes set!\n");
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
            WriteLine($"\n\"{tourCategory}\" tour set!\n");
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
            Write("Set the \"walking speed\" of the tourist in km/h. Valid range is [1 - 6]. " +
               "[Press \"Enter\" key to go back.]: ");
            walkingSpeedStr = ReadLine().Trim();
            canProceed = double.TryParse(walkingSpeedStr, out var walkSpeed)
               || walkSpeed >= 1 && walkSpeed <= 6;

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
            WriteLine($"\n\"Walking speed\" set to {walkingSpeed.Value} Km/h.\n");
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
            WriteLine($"\n\"Arrival time\" at the Hotel set at: {arrivalTime}\n");
         }

         return arrivalTime;
      }
      #endregion

      #region Tour Duration menu
      private TimeSpan? GetTourDuration()
      {
         string tourDurationStr = string.Empty;
         bool canProceed = default;
         string[] formats = { "g", "G", "%h" };
         TimeSpan? tourDuration = default;
         CultureInfo culture = CultureInfo.CurrentCulture;

         WriteLine("\n-----> TOUR DURATION <-----\n");
         do
         {
            Write("Insert the duration of the tour. Valid format: [hh:mm OR h:m] " +
               "[Press \"Enter\" key to go back.]\n");
            tourDurationStr = ReadLine().Trim();
            if (tourDurationStr == string.Empty)
            {
               break;
            }

            canProceed = TimeSpan.TryParseExact(tourDurationStr, formats, culture, out var interval)
               && interval.Hours >= 1;

            if (canProceed)
            {
               tourDuration = interval;
            }
            else
            {
               WriteLine("Invalid \"Tour duration\". The duration must be greater than or equals to 1.");
            }

         } while (!canProceed);

         if (tourDuration.HasValue)
         {
            WriteLine($"\nTour duration set to: {tourDuration.Value.Hours} hours " +
               $"and {tourDuration.Value.Minutes} minutes\n");
         }

         return tourDuration;
      }
      #endregion

      #region Algorithm Monitoring menu
      private bool? GetAlgorithmMonitoring()
      {
         string choice = string.Empty;
         bool canProceed = default;
         bool? toMonitoring = default;

         WriteLine("\n-----> ALGORITHM MONITORING <-----\n");
         do
         {
            WriteLine("Do you want to monitor algorithms's executions? [y/N]");
            choice = ReadLine().Trim();

            if (choice == "y" || choice == "Y")
            {
               toMonitoring = true;
            }
            else if (choice == "n" || choice == "N")
            {
               toMonitoring = false;
            }
            else
            {
               WriteLine("Invalid choice. Enter \"y | Y\" or \"n | N\"");
            }

            canProceed = toMonitoring.HasValue;
         } while (!canProceed);

         WriteLine($"Monitoring of the algorithms set to {toMonitoring}\n");
         return toMonitoring;
      }
      #endregion

      #region Menu stages
      private Collection<Stage> GetAlgorithmStages()
      {
         Collection<Stage> stages = new Collection<Stage>();
         WriteLine("\n********** { STAGES'S CONFIGURATION } **********\n");

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
         Stage stage = new Stage();
         WriteLine($"\n********** (SETTINGS STAGE {stageId}) **********\n");

         if (stageId == 1)
         {
            SetFirstStage(stageId, stage);
         }
         else if (stageId == 2)
         {
            SetSecondStage(stageId, stage);
         }
         else if (stageId == 3)
         {
            SetThirdStage(stageId, stage);
         }

         return stage;
      }

      private void SetFirstStage(int stageId, Stage stage)
      {
         AlgorithmType algorithm;
         WriteLine($"Select the Greedy algorithm of stage number {stageId}.\n");
         stage.Description = StageType.StageOne;
         stage.Category = AlgorithmFamily.Greedy;
         algorithm = GetGreedyAlgorithm();
         stage.Flow.CurrentAlgorithm = algorithm;
      }

      private void SetSecondStage(int stageId, Stage stage)
      {
         bool canProceed = default;
         string response = string.Empty;
         AlgorithmType algorithm;
         WriteLine($"Select a Local Search algorithm for stage {stageId}.\n");
         stage.Description = StageType.StageTwo;
         stage.Category = AlgorithmFamily.LocalSearch;
         algorithm = GetLocalSearchAlgorithm();

         if (algorithm != AlgorithmType.None)
         {
            do
            {
               stage.Flow.CurrentAlgorithm = algorithm;
               WriteLine($"Do you want to set an improvement algorithm for stage {stageId}? [y/N]: ");
               response = ReadLine().Trim();

               canProceed = response == "y" || response == "Y" ||
                  response == "n" || response == "N";

               if (!canProceed)
               {
                  WriteLine("Entered invalid string. Insert only \"y|Y\" or \"n|N\".\n");
               }
            } while (!canProceed);

            if (response == "y" || response == "Y")
            {
               WriteLine($"Select an improvement algorithm for stage {stageId}.\n");
               AlgorithmType improvementAlgorithm = GetImprovementAlgorithm();

               if (improvementAlgorithm != AlgorithmType.None)
               {
                  byte runningCount = GetAlgorithmIterations();
                  var (maxIterationsWithoutImprovements, improvementThreshold) = GetLocalSearchParameters(runningCount);
                  stage.Flow.ChildrenFlows.Add(new StageFlow(improvementAlgorithm, runningCount));
                  stage.Flow.MaxIterationsWithoutImprovements = maxIterationsWithoutImprovements;
                  stage.Flow.LkImprovementThreshold = improvementThreshold;
               }
               else
               {
                  ResetStage(stage);
               }
            }
         }
      }

      private void SetThirdStage(int stageId, Stage stage)
      {
         AlgorithmType algorithm;
         WriteLine($"Select a MetaHeuristic algorithm for stage {stageId}.\n");
         stage.Description = StageType.StageThree;
         stage.Category = AlgorithmFamily.MetaHeuristic;
         algorithm = GetMetaHeuristicAlgorithm();

         if (algorithm != AlgorithmType.None)
         {
            stage.Flow.CurrentAlgorithm = algorithm;
            WriteLine($"Select a Local Search algorithm for stage {stageId}.\n");
            AlgorithmType lsAlgorithm = GetLocalSearchAlgorithm();

            if (lsAlgorithm != AlgorithmType.None)
            {
               byte runningCount = GetAlgorithmIterations();
               var (maximumDeadlockIterations, canExecuteImprovements) = GetMetaHeuristicParameters(runningCount);
               stage.Flow.ChildrenFlows.Add(new StageFlow(lsAlgorithm, runningCount));
               stage.Flow.MaximumDeadlockIterations = maximumDeadlockIterations;
               stage.Flow.CanExecuteImprovements = canExecuteImprovements;

               if (canExecuteImprovements)
               {
                  WriteLine($"Select an improvement algorithm for stage {stageId}.\n");
                  AlgorithmType improvementAlgorithm = GetImprovementAlgorithm();
                  if (improvementAlgorithm != AlgorithmType.None)
                  {
                     runningCount = GetAlgorithmIterations();
                     stage.Flow.ChildrenFlows.First().ChildrenFlows.Add(new StageFlow(improvementAlgorithm, runningCount));
                  }
                  else
                  {
                     ResetStage(stage);
                  }
               }
            }
         }
      }

      private AlgorithmType GetGreedyAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = AlgorithmType.None;

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
         AlgorithmType algorithm = AlgorithmType.None;

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

      private AlgorithmType GetMetaHeuristicAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = AlgorithmType.None;

         WriteLine("<1> Tabù Search");
         WriteLine("<2> Back\n");
         choice = ReadLine().Trim();

         switch (choice)
         {
            case "1":
               algorithm = AlgorithmType.TabuSearch;
               break;
            default:
               break;
         }

         return algorithm;
      }

      private AlgorithmType GetImprovementAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = AlgorithmType.None;

         WriteLine("<1> Lin Kernighan");
         WriteLine("<2> Back\n");
         choice = ReadLine().Trim();

         switch (choice)
         {
            case "1":
               algorithm = AlgorithmType.LinKernighan;
               break;
            default:
               break;
         }

         return algorithm;
      }

      private (byte, ushort) GetLocalSearchParameters(byte maxIterations)
      {
         const string firstParamStr = "maximum iterations without improvements";
         const string secondParamStr = "improvement threshold";
         const ushort maximumImprovementThreshold = 500;
         byte maxIterationsWithoutImprovements = default;
         ushort improvementThreshold = default;
         bool canProceed = default;

         WriteLine("\n********** (TUNING PARAMETERS) **********\n");
         do
         {
            Write($"Set the \"{firstParamStr.ToUpper()}\" to trigger the improvement algorithm. " +
               $"Range: [1 - {maxIterations - 1}]: ");
            string firstParam = ReadLine().Trim();

            canProceed = byte.TryParse(firstParam, out byte firstParamValue);
            canProceed = firstParamValue > 0 && firstParamValue < maxIterations;

            if (canProceed)
            {
               maxIterationsWithoutImprovements = firstParamValue;
            }
            else
            {
               WriteLine($"\nInvalid parameters settings.\n" +
                  $"Valid range for the parameter \"{firstParamStr.ToUpper()}\": [1 - {maxIterations - 1}].\n");
            }
         } while (!canProceed);

         canProceed = default;

         do
         {
            Write($"Set the \"{secondParamStr.ToUpper()}\". Range: [1 - 500]: ");
            string secondParam = ReadLine().Trim();

            canProceed = ushort.TryParse(secondParam, out ushort secondParamValue);
            canProceed = secondParamValue > 0 && secondParamValue <= maximumImprovementThreshold;

            if (canProceed)
            {
               improvementThreshold = secondParamValue;
            }
            else
            {
               WriteLine($"\nInvalid parameters settings.\n" +
                  $"Valid range for the parameter \"{secondParam.ToUpper()}\": [1 - {maximumImprovementThreshold}]\n");
            }

         } while (!canProceed);

         return (maxIterationsWithoutImprovements, improvementThreshold);
      }

      private (byte, bool) GetMetaHeuristicParameters(byte maxIterations)
      {
         const string firstParamStr = "maximum deadlock iterations";
         byte maximumDeadlockIterations = default;
         bool canExecuteImprovements = default;
         bool canProceed = default;

         WriteLine("\n********** (TUNING PARAMETERS) **********\n");
         do
         {
            Write($"Set the \"{firstParamStr.ToUpper()}\" to stop the MetaHeuristic algorithm. " +
               $"Range: [1 - {maxIterations - 1}]: ");
            string firstParam = ReadLine().Trim();

            canProceed = byte.TryParse(firstParam, out byte firstParamValue) &&
               firstParamValue > 0 && firstParamValue < maxIterations;

            if (canProceed)
            {
               maximumDeadlockIterations = firstParamValue;
            }
            else
            {
               WriteLine($"Invalid value for \"{firstParamStr.ToUpper()}\" parameter.\n" +
                  $"Insert a value between 1 and {maxIterations - 1}.\n");
            }
         } while (!canProceed);

         canProceed = default;
         string response = string.Empty;

         do
         {
            Write("Do you want MetaHeuristic algorithm can executes improvements? [y/N]: ");
            response = ReadLine().Trim();

            canProceed = response == "y" || response == "Y" ||
               response == "n" || response == "N";

            if (!canProceed)
            {
               WriteLine("Entered invalid string. Insert only \"y|Y\" or \"n|N\".\n");
            }
         } while (!canProceed);

         canExecuteImprovements = response == "y" || response == "Y";
         return (maximumDeadlockIterations, canExecuteImprovements);
      }

      private byte GetAlgorithmIterations()
      {
         const string paramDescription = "maximum number of iterations";
         const byte runninCountThreshold = 50;
         byte runningCount = default;
         bool canProceed = default;

         do
         {
            Write($"Insert the \"{paramDescription.ToUpper()}\" for the algorithm. (Max: 50 iterations): ");
            string value = ReadLine().Trim();

            canProceed = byte.TryParse(value, out byte maxIterations) &&
               maxIterations > 0 && maxIterations <= runninCountThreshold;

            if (canProceed)
            {
               runningCount = maxIterations;
            }
            else
            {
               WriteLine($"\nInvalid value for \"{paramDescription.ToUpper()}\" parameter.\n" +
                  $"Insert a value between 1 and {runninCountThreshold}.\n");
            }
         } while (!canProceed);

         return runningCount;
      }

      private void ResetStage(Stage stage)
      {
         stage.Category = AlgorithmFamily.None;
         stage.Description = StageType.InvalidStage;
         stage.Flow.CurrentAlgorithm = AlgorithmType.None;
         stage.Flow.ChildrenFlows.Clear();
         stage.Flow.CanExecuteImprovements = true;
         stage.Flow.RunningCount = default;
         stage.Flow.LkImprovementThreshold = default;
         stage.Flow.MaximumDeadlockIterations = default;
         stage.Flow.MaxIterationsWithoutImprovements = default;
      }
      #endregion
      #endregion

      #endregion

      #region IConfigurationService implementation
      public ICollection<Configuration> Configurations => RunningConfigs.Configurations;

      public async Task ShowConfigurationsMenu()
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
                     await ShowAvailableConfigurationMenu();
                     break;

                  case "2":
                     await ShowCustomConfigurationMenu();
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

      public Configuration CreateConfiguration()
      {
         return new Configuration()
         {
            CurrentProblem = ProblemFamily.TeamOrienteering,
            TourCategory = _tourCategory,
            PointsFilename = @"cityscover-points-" + _problemSize.Value,
            StartingPointId = 1,
            WalkingSpeed = _walkingSpeed.Value / 3.6,  // in m/s.
            ArrivalTime = _arrivalTime.Value,
            TourDuration = _tourDuration.Value,
            AlgorithmMonitoring = _algorithmMonitoring.Value,
            Stages = _stages
         };
      }
      #endregion
   }
}