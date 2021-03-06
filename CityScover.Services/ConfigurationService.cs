﻿//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 09/12/2018
//

using CityScover.Commons;
using CityScover.Engine;
using CityScover.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CityScover.Engine.Configurations;
using static System.Console;

namespace CityScover.Services
{
   public class ConfigurationService : Singleton<ConfigurationService>, IConfigurationService
   {
      private readonly int _maxStagesCount;
      private readonly int _tourCategoriesCount;
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
         _tourCategoriesCount = 3;
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
            WriteLine("\n\t    --------------------------------------------------------\n");
         }
         WriteLine("\t============================================================\n");
      }

      private void DisplayStageFlow(StageFlow flow, string tabulator = "\t")
      {
         WriteLine($"{tabulator}     Current algorithm:   \"{flow.CurrentAlgorithm}\"");

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.MaxIterations))
         {
            int maxIterations = flow.AlgorithmParameters[ParameterCodes.MaxIterations];
            if (maxIterations != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Max iterations:      \"{maxIterations}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.CanDoImprovements))
         {
            bool canDoImprovements = flow.AlgorithmParameters[ParameterCodes.CanDoImprovements];
            if (canDoImprovements != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Can do improvements: \"{canDoImprovements}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.GreedyMaxNodesToAdd))
         {
            int maxNodesToAdd = flow.AlgorithmParameters[ParameterCodes.GreedyMaxNodesToAdd];
            if (maxNodesToAdd != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Maximum nodes to consider:  \"{maxNodesToAdd}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.RelaxedConstraints))
         {
            Collection<string> relaxedConstraints = flow.AlgorithmParameters[ParameterCodes.RelaxedConstraints];
            if (relaxedConstraints != default)
            {
               var constraints = relaxedConstraints
                  .Aggregate((currConstraint, nextConstraint) => currConstraint += "," + nextConstraint);
               WriteLine($"{tabulator}     " +
                  $"Relaxed constraints: \"" +                  
                  $"{constraints}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.ObjectiveFunctionScoreWeight))
         {
            double objectiveFunctionScoreWeight = flow.AlgorithmParameters[ParameterCodes.ObjectiveFunctionScoreWeight];
            if (objectiveFunctionScoreWeight != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Objective function score weight:  \"{objectiveFunctionScoreWeight}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.LocalSearchImprovementThreshold))
         {
            int lkImprovementThreshold = flow.AlgorithmParameters[ParameterCodes.LocalSearchImprovementThreshold];
            if (lkImprovementThreshold != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Lin Kernighan improvement threshold:  \"{lkImprovementThreshold}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.LocalSearchMaxRunsWithNoImprovements))
         {
            int maxIterationsWithNoImprovements = flow.AlgorithmParameters[ParameterCodes.LocalSearchMaxRunsWithNoImprovements];
            if (maxIterationsWithNoImprovements != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Maximum iterations without improvement:  \"{maxIterationsWithNoImprovements}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.TabuDeadlockIterations))
         {
            int maxDeadlockIterations = flow.AlgorithmParameters[ParameterCodes.TabuDeadlockIterations];
            if (maxDeadlockIterations != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Deadlock condition:  \"{maxDeadlockIterations}\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.HciTimeThresholdToTmax))
         {
            TimeSpan hndTmaxThreshold = flow.AlgorithmParameters[ParameterCodes.HciTimeThresholdToTmax];
            if (hndTmaxThreshold != default)
            {
               WriteLine($"{tabulator}     " +
                  $"TMax threshold:      \"" +
                  $"{hndTmaxThreshold.Hours + " hour" + ((hndTmaxThreshold.Hours == 1) ? string.Empty : "s ")} and " +
                  $"{hndTmaxThreshold.Minutes} minutes.\"");
            }
         }

         if (flow.AlgorithmParameters.ContainsKey(ParameterCodes.HcuTimeWalkThreshold))
         {
            TimeSpan hndTimeWalkThreshold = flow.AlgorithmParameters[ParameterCodes.HcuTimeWalkThreshold];
            if (hndTimeWalkThreshold != default)
            {
               WriteLine($"{tabulator}     " +
                  $"Time Walk threshold: \"" +
                  $"{hndTimeWalkThreshold.Hours + " hour" + ((hndTimeWalkThreshold.Hours == 1) ? string.Empty : "s ")} and " +
                  $"{hndTimeWalkThreshold.Minutes} minutes.\"");
            }
         }

         if (flow.ChildrenFlows.Any())
         {
            WriteLine($"\n{tabulator}     \"{flow.CurrentAlgorithm} inner algorithms\"");
            string tab = tabulator + "   ";
            foreach (var children in flow.ChildrenFlows)
            {
               WriteLine($"{tabulator}     " + "{");
               DisplayStageFlow(children, tab);
               WriteLine($"{tabulator}     " + "}");
            }
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
               WriteLine("<| 1 |> Set the \"PROBLEM SIZE\"\n");
               WriteLine("<| 2 |> Set the \"TOUR CATEGORY\"\n");
               WriteLine("<| 3 |> Set the \"WALKING SPEED\"\n");
               WriteLine("<| 4 |> Set the \"ARRIVAL TIME TO HOTEL\"\n");
               WriteLine("<| 5 |> Set the \"TOUR DURATION\"\n");
               WriteLine("<| 6 |> Set the \"ALGORITHM'S STAGES\"\n");
               WriteLine("<| 7 |> Back\n");
               Write("Select an option: ");
               choice = ReadLine().Trim();

               canProceed = int.TryParse(choice, out option)
                  && option >= 1 && option <= 7;

               if (!canProceed)
               {
                  WriteLine($"Insert a value between 1 and 7.\n");
               }
            } while (!canProceed);

            isExiting = SetConfigurationParameter(option);
            if (!isExiting)
            {
               await Task.Delay(700).ConfigureAwait(continueOnCapturedContext: false);
            }
            else
            {
               ResetParameters();
            }
         }

         if (canCreateConfiguration)
         {
            //_lambdaWeight = GetLambdaWeight();
            _algorithmMonitoring = GetAlgorithmMonitoring();
            WriteLine("\nCreation of new configuration in progress...!\n");
            await Task.Delay(1000).ConfigureAwait(continueOnCapturedContext: false);
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
                     ResetParameters();
                     break;
                  default:
                     WriteLine("Selected invalid option. " +
                        "Enter an option between 1 and 3.\n");
                     break;
               }
            } while (choice != "3");
         }
      }

      private void ResetParameters()
      {
         _problemSize = default;
         _tourCategory = default;
         _walkingSpeed = default;
         _arrivalTime = default;
         _tourDuration = default;
         _algorithmMonitoring = default;
         _stages.Clear();
      }

      private bool IsConfigurationSettingsCompleted()
      {
         return _tourCategory != TourCategoryType.None &&
               _problemSize.HasValue && _walkingSpeed.HasValue &&
               _arrivalTime.HasValue && _tourDuration.HasValue &&
               _stages.Count == _maxStagesCount;
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
               _stages = GetAlgorithmStages();
               break;
            case 7:
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
            WriteLine("1. 15 nodes (5 for each tour category)");
            WriteLine("2. 30 nodes (10 for each tour category)");
            WriteLine("3. 45 nodes (15 for each tour category)");
            WriteLine("4. 60 nodes (20 for each tour category)");
            WriteLine("5. 75 nodes (25 for each tour category)");
            WriteLine("6. 90 nodes (30 for each tour category)");
            WriteLine("7. Back\n");

            Write("Enter a problem size [number of nodes]: ");
            choice = ReadLine().Trim();
            canProceed = int.TryParse(choice, out _);

            switch (choice)
            {
               case "1":
                  nodesCount = 15;
                  break;
               case "2":
                  nodesCount = 30;
                  break;
               case "3":
                  nodesCount = 45;
                  break;
               case "4":
                  nodesCount = 60;
                  break;
               case "5":
                  nodesCount = 75;
                  break;
               case "6":
                  nodesCount = 90;
                  break;
               case "7":
                  break;
               default:
                  canProceed = false;
                  WriteLine("Enter a value between [1-7].\n");
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
            WriteLine("1. Historical and Cultural");
            WriteLine("2. Culinary");
            WriteLine("3. Sport");
            WriteLine("4. Back\n");

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

         WriteLine("\n-----> TOUR DURATION <-----\n");
         do
         {
            Write("Insert the duration of the tour. Valid format: [hh:mm OR h:m] " +
               "[Press \"Enter\" key to go back.]: ");
            tourDurationStr = ReadLine().Trim();
            if (tourDurationStr == string.Empty)
            {
               break;
            }

            canProceed = TimeSpan.TryParseExact(tourDurationStr, formats, CultureInfo.CurrentCulture, out var interval)
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

      #region Lambda weight menu
      //private double? GetLambdaWeight()
      //{
      //   string valueStr = string.Empty;
      //   double? lambda = default;
      //   bool canProceed = default;

      //   WriteLine("\n-----> CONVEX COMBINATION: [z = lambda*x + (1 - lambda)*y] <-----\n");
      //   do
      //   {
      //      Write("Insert the value between 0 and 1 of the lambda weight typed in floating point format: ");
      //      valueStr = ReadLine().Trim();

      //      if (valueStr.Contains("."))
      //      {
      //         valueStr = valueStr.Replace('.', ',');
      //      }
      //      canProceed = double.TryParse(valueStr, out double lambdaValue) &&
      //         lambdaValue >= 0 && lambdaValue <= 1;

      //      if (canProceed)
      //      {
      //         lambda = lambdaValue;
      //      }
      //      else
      //      {
      //         WriteLine("Invalid lambda value. Valid range is [0 - 1]\n");
      //      }
      //   } while (!canProceed);

      //   if (lambda.HasValue)
      //   {
      //      WriteLine($"\n\"Lambda weight\" set to {lambda.Value}\n");
      //   }

      //   return lambda;
      //}
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
            Write("Do you want to monitor algorithms's executions? [y/N]: ");
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
               WriteLine("Invalid choice. Enter \"y\" or \"Y\" or \"n\" or \"N\"");
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
         bool canProceed = default;
         string response = string.Empty;
         AlgorithmType algorithm;

         if (!_problemSize.HasValue)
         {
            WriteLine("You must set the problem size before setting the greedy algorithm!\n");
            return;
         }

         WriteLine($"Select the Greedy algorithm of stage number {stageId}.\n");
         stage.Description = StageType.StageOne;
         stage.Category = AlgorithmFamily.Greedy;
         algorithm = GetGreedyAlgorithm();

         if (algorithm != AlgorithmType.None)
         {
            stage.Flow.CurrentAlgorithm = algorithm;
            SetRelaxedConstraints(stage.Flow);
            SetObjectiveFunctionWeight(stage.Flow);

            do
            {
               Write("Do you want the Greedy algorithm takes into account improvements? [y/N]: ");
               response = ReadLine().Trim();

               canProceed = response == "y" || response == "Y" ||
                  response == "n" || response == "N";

               if (!canProceed)
               {
                  WriteLine("Entered invalid string. Insert only \"y\" or \"Y\" or \"n\" or \"N\".\n");
               }
            } while (!canProceed);

            if (response == "y" || response == "Y")
            {
               int maxNodesToAdd = GetGreedyParameters();
               var (tMaxThreshold, timeWalkThreshold) = GetCustomAlgorithmParameters();
               stage.Flow.AlgorithmParameters[ParameterCodes.CanDoImprovements] = true;               
               stage.Flow.AlgorithmParameters[ParameterCodes.GreedyMaxNodesToAdd] = maxNodesToAdd;
               StageFlow stageFlow = new StageFlow(AlgorithmType.HybridCustomInsertion);
               stageFlow.AlgorithmParameters[ParameterCodes.HciTimeThresholdToTmax] = tMaxThreshold;
               stageFlow.AlgorithmParameters[ParameterCodes.HcuTimeWalkThreshold] = timeWalkThreshold;
               SetRelaxedConstraints(stageFlow);
               SetObjectiveFunctionWeight(stageFlow);
               stage.Flow.ChildrenFlows.Add(stageFlow);
            }
            else
            {
               stage.Flow.AlgorithmParameters[ParameterCodes.CanDoImprovements] = false;
            }
         }
         else
         {
            ResetStage(stage);
         }
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
            SetRelaxedConstraints(stage.Flow);
            SetObjectiveFunctionWeight(stage.Flow);
            do
            {
               stage.Flow.CurrentAlgorithm = algorithm;
               stage.Flow.AlgorithmParameters[ParameterCodes.CanDoImprovements] = true;
               Write($"Do you want to set an improvement algorithm for stage {stageId}? [y/N]: ");
               response = ReadLine().Trim();

               canProceed = response == "y" || response == "Y" ||
                  response == "n" || response == "N";

               if (!canProceed)
               {
                  WriteLine("Entered invalid string. Insert only \"y\" or \"Y\" or \"n\" or \"N\".\n");
               }
            } while (!canProceed);

            if (response == "y" || response == "Y")
            {
               SetStageImprovementSettings(stageId, stage);
            }
         }
         else
         {
            ResetStage(stage);
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
               int runningCount = GetAlgorithmIterations();
               var (maxDeadlockIterations, tenureFactor) = GetMetaHeuristicParameters(runningCount);
               stage.Flow.AlgorithmParameters[ParameterCodes.TabuDeadlockIterations] = maxDeadlockIterations;
               stage.Flow.AlgorithmParameters[ParameterCodes.TabuTenureFactor] = tenureFactor;
               StageFlow stageFlow = new StageFlow(lsAlgorithm);
               stageFlow.AlgorithmParameters[ParameterCodes.MaxIterations] = runningCount;
               SetRelaxedConstraints(stageFlow);
               SetObjectiveFunctionWeight(stageFlow);
               bool canExecuteImprovements = GetCanDoImprovements();

               if (canExecuteImprovements)
               {
                  stageFlow.AlgorithmParameters[ParameterCodes.CanDoImprovements] = true;
                  stage.Flow.ChildrenFlows.Add(stageFlow);
                  SetStageImprovementSettings(stageId, stage);
               }
               else
               {
                  stageFlow.AlgorithmParameters[ParameterCodes.CanDoImprovements] = false;
               }
            }
            else
            {
               ResetStage(stage);
            }
         }
         else
         {
            ResetStage(stage);
         }
      }

      private void SetRelaxedConstraints(StageFlow flow)
      {
         bool canProceed = default;
         string response = string.Empty;

         do
         {
            Write("Do you want to relax some problem constraints for this algorithm execution? [y/N]: ");
            response = ReadLine().Trim();

            canProceed = response == "y" || response == "Y" ||
               response == "n" || response == "N";

            if (!canProceed)
            {
               WriteLine("Entered invalid string. Insert only \"y\" or \"Y\" or \"n\" or \"N\".\n");
            }
         } while (!canProceed);

         if (response == "y" || response == "Y")
         {
            SetRelaxedConstraintsInternal(flow);
         }
      }

      private void SetRelaxedConstraintsInternal(StageFlow flow)
      {
         bool canProceed = default;
         string choice = string.Empty;
         Collection<string> relaxedConstraints = new Collection<string>();
         do
         {
            WriteLine($"Select one or more constraint which you want to relax.\n");
            WriteLine("1. Time Windows");
            WriteLine("2. Back\n");

            choice = ReadLine().Trim();
            switch (choice)
            {
               case "1":
                  relaxedConstraints.Add(Utils.TimeWindowsConstraint);
                  canProceed = true;
                  break;
               default:
                  break;
            }
         } while (!canProceed);

         if (relaxedConstraints.Any())
         {
            flow.AlgorithmParameters[ParameterCodes.RelaxedConstraints] = relaxedConstraints;
            var constraints = relaxedConstraints
                  .Aggregate((currConstraint, nextConstraint) => currConstraint += " , " + nextConstraint);
            WriteLine($"\nConstraints relaxed are: {constraints}.\n");
         }
      }

      private void SetObjectiveFunctionWeight(StageFlow flow)
      {
         bool canProceed = default;
         string response = string.Empty;

         do
         {
            Write($"Do you want to change the default value for objective function (lambda) weight (Default value is {Utils.ObjectiveFunctionWeightDefault}) ? [y/N]: ");
            response = ReadLine().Trim();

            canProceed = response == "y" || response == "Y" ||
               response == "n" || response == "N";

            if (!canProceed)
            {
               WriteLine("Entered invalid string. Insert only \"y\" or \"Y\" or \"n\" or \"N\".\n");
            }
         } while (!canProceed);

         if (response == "y" || response == "Y")
         {
            SetObjectiveFunctionWeightInternal(flow);
         }
      }

      private void SetObjectiveFunctionWeightInternal(StageFlow flow)
      {
         bool canProceed = default;
         string valueStr = string.Empty;
         double? lambda = default;
         Collection<string> relaxedConstraints = new Collection<string>();
         WriteLine("\n-----> CONVEX COMBINATION: [z = lambda*x + (1 - lambda)*y] <-----\n");
         do
         {
            WriteLine($"Insert the value between 0 and 1 of the lambda weight typed in floating point format.\n" +
               $"x = Score term.\ny = Distance term.\n\nExamples: 0.5 means that both terms have the same weight into cost computation.\n" +
               $"1 means that only the score term is taken into consideration.\n");
            valueStr = ReadLine().Trim();

            if (valueStr.Contains("."))
            {
               valueStr = valueStr.Replace('.', ',');
            }
            canProceed = double.TryParse(valueStr, out double lambdaValue) &&
               lambdaValue >= 0 && lambdaValue <= 1;

            if (canProceed)
            {
               lambda = lambdaValue;
            }
            else
            {
               WriteLine("Invalid lambda value. Valid range is [0 - 1]\n");
            }
         } while (!canProceed);

         if (lambda.HasValue)
         {
            flow.AlgorithmParameters[ParameterCodes.ObjectiveFunctionScoreWeight] = lambda;
            WriteLine($"\n\"Lambda weight\" set to {lambda.Value}\n");
         }
      }

      private void SetStageImprovementSettings(int stageId, Stage stage)
      {
         string response = string.Empty;
         Write($"How many improvement algorithms do you want to run for stage {stageId}? [Max: 2]: ");
         response = ReadLine().Trim();

         if (int.TryParse(response, out int improvements) &&
            improvements >= 1 && improvements <= 2)
         {
            if (improvements == 1)
            {
               WriteLine();
               AlgorithmType improvementAlgorithm = GetImprovementAlgorithm();
               if (improvementAlgorithm != AlgorithmType.None)
               {
                  SetImprovementAlgorithmParams(stage, improvementAlgorithm);
               }
            }
            else
            {
               for (int i = 1; i <= improvements; i++)
               {
                  WriteLine($"\nSet the improvement algorithm number {i}.\n");
                  AlgorithmType improvementAlgorithm = GetImprovementAlgorithm();
                  if (improvementAlgorithm != AlgorithmType.None)
                  {
                     SetImprovementAlgorithmParams(stage, improvementAlgorithm);
                  }
               }
            }
         }
         else
         {
            ResetStage(stage);
         }
      }

      private void SetImprovementAlgorithmParams(Stage stage, AlgorithmType improvementAlgorithm)
      {
         if (improvementAlgorithm == AlgorithmType.LinKernighan)
         {
            int runningCount = GetAlgorithmIterations();
            var (maxRunsWithNoImprovements, improvementThreshold) = GetLocalSearchParameters(runningCount);

            if (stage.Description == StageType.StageTwo)
            {
               stage.Flow.AlgorithmParameters[ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = maxRunsWithNoImprovements;
               stage.Flow.AlgorithmParameters[ParameterCodes.LocalSearchImprovementThreshold] = improvementThreshold;
               StageFlow childrenFlow = new StageFlow(improvementAlgorithm);
               childrenFlow.AlgorithmParameters[ParameterCodes.MaxIterations] = runningCount;
               SetRelaxedConstraints(childrenFlow);
               SetObjectiveFunctionWeight(childrenFlow);
               stage.Flow.ChildrenFlows.Add(childrenFlow);
            }
            else if (stage.Description == StageType.StageThree)
            {
               foreach (var childFlow in stage.Flow.ChildrenFlows)
               {
                  childFlow.AlgorithmParameters[ParameterCodes.LocalSearchMaxRunsWithNoImprovements] = maxRunsWithNoImprovements;
                  childFlow.AlgorithmParameters[ParameterCodes.LocalSearchImprovementThreshold] = improvementThreshold;
                  StageFlow nephewFlow = new StageFlow(improvementAlgorithm);
                  nephewFlow.AlgorithmParameters[ParameterCodes.MaxIterations] = runningCount;
                  SetRelaxedConstraints(nephewFlow);
                  SetObjectiveFunctionWeight(nephewFlow);
                  childFlow.ChildrenFlows.Add(nephewFlow);
               }
            }
         }
         else if (improvementAlgorithm == AlgorithmType.HybridCustomInsertion)
         {
            var (tMaxThreshold, timeWalkThreshold) = GetCustomAlgorithmParameters();

            if (stage.Description == StageType.StageTwo)
            {
               StageFlow stageFlow = new StageFlow(improvementAlgorithm);
               SetRelaxedConstraints(stageFlow);
               SetObjectiveFunctionWeight(stageFlow);
               stageFlow.AlgorithmParameters[ParameterCodes.MaxIterations] = 1;
               stageFlow.AlgorithmParameters[ParameterCodes.HciTimeThresholdToTmax] = tMaxThreshold;
               stageFlow.AlgorithmParameters[ParameterCodes.HcuTimeWalkThreshold] = timeWalkThreshold;
               stage.Flow.ChildrenFlows.Add(stageFlow);
            }
            else if (stage.Description == StageType.StageThree)
            {
               foreach (var childFlow in stage.Flow.ChildrenFlows)
               {
                  StageFlow stageFlow = new StageFlow(improvementAlgorithm);
                  SetRelaxedConstraints(stageFlow);
                  SetObjectiveFunctionWeight(stageFlow);
                  stageFlow.AlgorithmParameters[ParameterCodes.MaxIterations] = 1;
                  stageFlow.AlgorithmParameters[ParameterCodes.HciTimeThresholdToTmax] = tMaxThreshold;
                  stageFlow.AlgorithmParameters[ParameterCodes.HcuTimeWalkThreshold] = timeWalkThreshold;
                  childFlow.ChildrenFlows.Add(stageFlow);
               }
            }
         }
      }

      private AlgorithmType GetGreedyAlgorithm()
      {
         string choice = string.Empty;
         AlgorithmType algorithm = AlgorithmType.None;

         WriteLine("1. Nearest Neighbor");
         WriteLine("2. Nearest Neighbor Knapsack");
         WriteLine("3. Cheapest Insertion");
         WriteLine("4. Back\n");
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

         WriteLine("1. Two Opt");
         WriteLine("2. Back\n");
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

         WriteLine("1. Lin Kernighan");
         WriteLine("2. Hybrid Nearest Distance");
         WriteLine("3. Back\n");
         choice = ReadLine().Trim();

         switch (choice)
         {
            case "1":
               algorithm = AlgorithmType.LinKernighan;
               break;
            case "2":
               algorithm = AlgorithmType.HybridCustomInsertion;
               break;
            default:
               break;
         }

         return algorithm;
      }

      private bool GetCanDoImprovements()
      {
         bool canProceed = default;
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

         return response == "y" || response == "Y";
      }

      private int GetGreedyParameters()
      {
         const string paramDescription = "maximum nodes to take into account";
         int maxNodesToAdd = default;
         bool canProceed = default;
         string valueStr = string.Empty;

         WriteLine("\n********** (TUNING PARAMETERS) **********\n");
         do
         {
            Write($"Insert the \"{paramDescription.ToUpper()}\" (Valid range [5 - {(int)_problemSize / _tourCategoriesCount}]): ");
            valueStr = ReadLine().Trim();

            canProceed = int.TryParse(valueStr, out int value) &&
               value > 5 && value <= (int)_problemSize / _tourCategoriesCount;

            if (canProceed)
            {
               maxNodesToAdd = value;
            }
            else
            {
               WriteLine($"\nInvalid value for \"{paramDescription.ToUpper()}\".\n" +
                  $"Insert a value greater than 5 and less than or equals to {(int)_problemSize / _tourCategoriesCount}.\n");
            }

         } while (!canProceed);

         return maxNodesToAdd;
      }

      private (int, int) GetLocalSearchParameters(int maxIterations)
      {
         const string firstParamStr = "maximum iterations without improvements";
         const string secondParamStr = "improvement threshold";
         const int maximumImprovementThreshold = 500;
         int maxRunsWithNoImprovements = default;
         int improvementThreshold = default;
         bool canProceed = default;

         WriteLine("\n********** (TUNING PARAMETERS) **********\n");

         if (maxIterations > 1)
         {
            do
            {
               Write($"Set the \"{firstParamStr.ToUpper()}\" to trigger the improvement algorithm. " +
                  $"Range: [1 - {maxIterations - 1}]: ");
               string firstParam = ReadLine().Trim();

               canProceed = int.TryParse(firstParam, out int firstParamValue);
               canProceed = firstParamValue > 0 && firstParamValue < maxIterations;

               if (canProceed)
               {
                  maxRunsWithNoImprovements = firstParamValue;
               }
               else
               {
                  WriteLine($"\nInvalid parameters settings.\n" +
                     $"Valid range for the parameter \"{firstParamStr.ToUpper()}\": [1 - {maxIterations - 1}].\n");
               }
            } while (!canProceed);

            canProceed = default;
         }

         do
         {
            Write($"Set the \"{secondParamStr.ToUpper()}\". Range: [1 - 500]: ");
            string secondParam = ReadLine().Trim();

            canProceed = int.TryParse(secondParam, out int secondParamValue);
            canProceed = secondParamValue > 0 && secondParamValue <= maximumImprovementThreshold;

            if (canProceed)
            {
               improvementThreshold = secondParamValue;
            }
            else
            {
               WriteLine($"\nInvalid parameters settings.\n" +
                  $"Valid range for the parameter: [1 - {maximumImprovementThreshold}]\n");
            }

         } while (!canProceed);

         return (maxRunsWithNoImprovements, improvementThreshold);
      }

      private (int, int) GetMetaHeuristicParameters(int maxIterations)
      {
         const string firstParamStr = "maximum deadlock iterations";
         const string secondParamStr = "tabu tenure factor";
         int tenureFactor = default;
         int maxDeadlockIterations = default;
         bool canProceed = default;

         WriteLine("\n********** (TUNING PARAMETERS) **********\n");

         if (maxIterations > 1)
         {
            do
            {
               Write($"Set the \"{firstParamStr.ToUpper()}\" to stop the MetaHeuristic algorithm. " +
                  $"Range: [1 - {maxIterations - 1}]: ");
               string firstParam = ReadLine().Trim();

               canProceed = int.TryParse(firstParam, out int firstParamValue) &&
                  firstParamValue > 0 && firstParamValue < maxIterations;

               if (canProceed)
               {
                  maxDeadlockIterations = firstParamValue;
               }
               else
               {
                  WriteLine($"Invalid value for \"{firstParamStr.ToUpper()}\" parameter.\n" +
                     $"Insert a value between 1 and {maxIterations - 1}.\n");
               }
            } while (!canProceed);

            canProceed = default;
         }

         do
         {
            Write($"Set the \"{secondParamStr.ToUpper()}\" to calculate the TABU TENURE. " +
               $"Valid values: [2, 4, 8]: ");
            string secondParam = ReadLine().Trim();

            canProceed = int.TryParse(secondParam, out int secondParamValue) &&
               secondParamValue == 2 || secondParamValue == 4 || secondParamValue == 8;

            if (canProceed)
            {
               tenureFactor = secondParamValue;
            }
            else
            {
               WriteLine($"\nInvalid value {secondParamValue} for \"{secondParamStr.ToUpper()}\" parameter. " +
                  $"Valid values: [2, 4, 8].\n");
            }
         } while (!canProceed);

         return (maxDeadlockIterations, tenureFactor);
      }

      private (TimeSpan, TimeSpan) GetCustomAlgorithmParameters()
      {
         const string tMaxDescription = "time threshold to tmax";
         const string timeWalkDescription = "time walk threshold for a route";
         TimeSpan tMaxThreshold = default;
         TimeSpan timeWalkThreshold = default;
         bool canProceed = default;

         do
         {
            Write($"Insert the \"{tMaxDescription.ToUpper()}\" for custom algorithm in the format hours and minutes [H:min]: ");
            string value = ReadLine().Trim();
            canProceed = TimeSpan.TryParse(value, out tMaxThreshold);

            if (tMaxThreshold.Hours == 0 && tMaxThreshold.Minutes == 0)
            {
               canProceed = false;
            }
         } while (!canProceed);

         canProceed = default;

         do
         {
            Write($"Insert the \"{timeWalkDescription.ToUpper()}\" for custom algorithm in the format hours and minutes [H:min]: ");
            string value = ReadLine().Trim();
            canProceed = TimeSpan.TryParse(value, out timeWalkThreshold);

            if (timeWalkThreshold.Hours == 0 && timeWalkThreshold.Minutes == 0)
            {
               canProceed = false;
            }

         } while (!canProceed);

         return (tMaxThreshold, timeWalkThreshold);
      }

      private int GetAlgorithmIterations()
      {
         const string paramDescription = "maximum number of iterations";
         const int runninCountThreshold = 50;
         int runningCount = default;
         bool canProceed = default;

         do
         {
            Write($"Insert the \"{paramDescription.ToUpper()}\" for the algorithm. (Max: 50 iterations): ");
            string value = ReadLine().Trim();

            canProceed = int.TryParse(value, out int maxIterations) &&
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
         stage.Flow.AlgorithmParameters.Clear();
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
            PointsFilename = @"cityscover-points-" + _problemSize.Value + ".xml",
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