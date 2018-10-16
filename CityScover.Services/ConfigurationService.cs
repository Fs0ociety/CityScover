//
// CityScover
// Version 1.0
//
// @authors
// Andrea Ritondale
// Andrea Mingardo
// 
// File update: 16/10/2018
//

using CityScover.Commons;
using CityScover.Engine;
using CityScover.Engine.Configs;
using System;
using System.Collections.Generic;
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
         bool result = default;

         do
         {
            Write("Do you want to run this configuration? [y/N]: ");
            choice = ReadLine().Trim(' ').ToLower();
            result = choice == "y" || choice == "n";
            if (!result)
            {
               WriteLine($"String \"{choice}\" is not valid. Enter \"y\" or \"N\".\n");
            }
         } while (!result);

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
         bool result = default;
         int choiceValue = default;

         while (true)
         {
            do
            {
               WriteLine("Configurations available:\n");
               for (int confIndex = 0; confIndex < Configurations.Count; ++confIndex)
               {
                  WriteLine($"> Configuration {confIndex + 1}");
               }
               WriteLine($"> Go back [Press \"Enter\" key]\n");

               Write("Show a configuration: ");
               configChoice = ReadLine();

               if (configChoice == string.Empty)
               {
                  return;
               }
               result = int.TryParse(configChoice, out choiceValue) &&
                  Enumerable.Range(1, Configurations.Count).Contains(choiceValue);
               if (!result)
               {
                  WriteLine($"Enter a value between {1} - {Configurations.Count}.\n");
               }

            } while (!result);

            DisplayConfiguration(Configurations.ElementAt(--choiceValue));
         }
      }

      private void ShowCustomConfigurationMenu()
      {
         throw new NotImplementedException();
      }
      #endregion

      #region IConfigurationService implementation
      public ICollection<Configuration> Configurations => RunningConfigs.Configurations;

      public void ShowConfigurationsMenu()
      {
         WriteLine("************************* Configurations Menu *************************\n");

         string choice = string.Empty;
         bool result = default;
         bool isExiting = default;

         while (true)
         {
            do
            {
               WriteLine("Choose a following option:\n");
               WriteLine("> 1. Run a configuration from those available.");
               WriteLine("> 2. Create a custom configuration.");
               WriteLine("> 3. Exit.\n");
               Write("> ");
               choice = ReadLine();

               result = int.TryParse(choice, out int value) &&
                  Enumerable.Range(1, 3).Contains(value);
               if (!result)
               {
                  WriteLine("Enter a value between [1-3].\n");
               }

            } while (!result);

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
            }

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
