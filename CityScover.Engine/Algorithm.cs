//
// CityScover
// Version 1.0
//
// Authors: Andrea Ritondale, Andrea Mingardo
// File update: 25/08/2018
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
      private readonly Problem _problem;
      private int _currentStep;
      private AlgorithmStatus _status;
      private bool _acceptImprovementsOnly;
      private Solution _currentSolution;
      private Solution _bestSolution;

      #region Constructors
      internal Algorithm(CityMapGraph workingGraph, Problem problem)
      {
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
      internal abstract bool StopConditions();
      internal void Start()
      {
         _status = AlgorithmStatus.Initializing;
         OnInitializing();
         // TODO: Pubblicazione dell'evento di inizializazione ai subscribers.

         _status = AlgorithmStatus.Running;
         // TODO: Pubblicazione dell'evento di esecuzione ai subscribers.

         while (!StopConditions())
         {
            try
            {
               PerformStep();
               _currentStep++;
            }
            catch
            {
               _status = AlgorithmStatus.Error;
               OnError();
               // TODO: Pubblicazione dell'evento di errore ai subscribers.
            }
         }

         _status = AlgorithmStatus.Terminating;
         OnTerminating();
         // TODO: Pubblicazione dell'evento di inizio terminazione ai subscribers.

         _status = AlgorithmStatus.Terminated;
         OnTerminated();
         // TODO: Pubblicazione dell'evento di avvenuta terminazione ai subscribers.
      }
      #endregion
   }
}
