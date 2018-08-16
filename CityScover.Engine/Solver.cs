//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 09/08/2018
//

using CityScover.Engine.Configurations;
using CityScover.Utils;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   public sealed class Solver : Singleton<Solver>
   {
      private IEnumerable<Configuration> _configurations;

      #region Constructors
      private Solver()
      {
         // Il Solver crea il problema e lo trasmette all'ExecutionTracer.
         //Problem p = new Problem();
      }
      #endregion

      #region Internal methods (Factory methods)
      internal static ExecutionTracer CreateExecutionTracer()
      {
         throw new NotImplementedException();
      }

      internal static SolverConstraintHelper CreateSolverConstraintHelper()
      {
         throw new NotImplementedException();
      }

      internal static SolverEvaluationHelper CreateSolverEvaluationHelper()
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Private methods
      private AlgorithmType GetAlgorithmTypeById(int algorithmId)
      {
         // CODICE PROVVISORIO: Valutare trasferimento in CityScover.Utils
         AlgorithmType algorithm = AlgorithmType.None;

         switch (algorithmId)
         {
            case (int)AlgorithmType.NearestNeighbor:
               algorithm = AlgorithmType.NearestNeighbor;
               break;
            default:
               break;
         }

         return algorithm;
         //throw new NotImplementedException();
      }

      private void InitializeConfigurations()
      {
         // CODICE PROVVISORIO: Valutare trasferimento in CityScover.Utils
         const string resourceStream = "CityScover.Engine.Configurations.solver-config-1.xml";
         XmlDocument document = new XmlDocument();
         document.Load(typeof(Solver).Assembly.GetManifestResourceStream(resourceStream));

         foreach (XmlNode node in document.GetElementsByTagName("Configuration"))
         {
            foreach (XmlNode childNode in node.ChildNodes)
            {
               if (childNode.NodeType != XmlNodeType.Element || !childNode.Name.Equals("Stages"))
               {
                  continue;
               }

               foreach (XmlNode nestedChild in childNode.ChildNodes)
               {
                  int stageId = int.Parse(nestedChild.Attributes["id"].Value);
                  var stage = StageType.GetStageById(stageId);
                  int algorithmId = 0;

                  foreach (XmlNode nestedNode in nestedChild.ChildNodes)
                  {
                     algorithmId = int.Parse(nestedNode.Attributes["id"].Value);
                  }

                  var algorithm = GetAlgorithmTypeById(algorithmId);
                  Configuration conf = new Configuration();
                  //conf.AddStage(stage, AlgorithmType.NearestNeighbor);
               }
            }
         }
      }
      #endregion

      #region Public methods
      public void Run()
      {
         throw new NotImplementedException(nameof(Run));
      }
      #endregion

      #region Overrides
      protected override void InitializeInstance()
      {
         // TODO: Leggere i files di configurazione dall'oppurtuna cartella.
         // Popolamento della collezione delle configurazioni formate dalla coppia stages-algoritmi.
         // Valutare spostamento dell'inizializzazione delle configurazioni in CityScover.Utils.
         InitializeConfigurations();
      }
      #endregion
   }
}
