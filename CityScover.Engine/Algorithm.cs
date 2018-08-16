//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 16/08/2018
//

using CityScover.Engine.Workers;
using System;

namespace CityScover.Engine
{
   /// <summary>
   /// TODO
   /// </summary>
   internal abstract class Algorithm
   {
      private int _currentStep;
      private AlgorithmStatus _status;
      private bool _acceptImprovementsOnly;
      // Campo che rappresenta il grafo su cui lavora l'algoritmo (Da valutare, non definitivo)
      private CityGraphWorker _currentGraph;
      private readonly Problem _problem;

      #region Constructors
      internal Algorithm(CityGraphWorker workingGraph, Problem problem)
      {
         _currentGraph = workingGraph ?? throw new ArgumentNullException(nameof(workingGraph));
         _problem = problem ?? throw new ArgumentNullException(nameof(problem));
         _acceptImprovementsOnly = true;
      }
      #endregion

      #region Internal properties
      internal int CurrentStep
      {
         get => _currentStep;
         private set
         {
            if (_currentStep != value)
            {
               _currentStep = value;
            }
         }
      }
      internal AlgorithmStatus Status
      {
         get => _status;
         private set
         {
            if (_status != value)
            {
               _status = value;
            }
         }
      }

      internal bool AcceptImprovementsOnly
      {
         get => _acceptImprovementsOnly;
         private set
         {
            if (_acceptImprovementsOnly != value)
            {
               _acceptImprovementsOnly = value;
            }
         }
      }
      #endregion

      #region Internal methods
      internal bool Execute(int stepsCount)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region Internal abstract methods
      internal abstract void OnInitializing();
      internal abstract void PerformStep();
      internal abstract void OnTerminating();
      internal abstract void OnTerminated();
      internal abstract void OnError();
      #endregion
   }
}
