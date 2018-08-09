namespace CityScover.Engine
{
   public struct StageType
   {
      /// <summary>
      /// TODO
      /// </summary>
      public static readonly StageType InvalidStage =
         new StageType(0, "Invalid Stage");

      /// <summary>
      /// TODO
      /// </summary>
      public static readonly StageType StageOne = 
         new StageType(1, "Greedy Stage");

      /// <summary>
      /// TODO
      /// </summary>
      public static readonly StageType StageTwo = 
         new StageType(2, "Local Search Stage");

      /// <summary>
      /// TODO
      /// </summary>
      public static readonly StageType StageThree = 
         new StageType(3, "Local Search Improvement Stage");

      /// <summary>
      /// TODO
      /// </summary>
      public static readonly StageType StageFour = 
         new StageType(4, "Metaheuristic Stage");

      private StageType(int id, string description)
      {
         Id = id;
         Description = description;
      }

      public int Id { get; private set; }
      public string Description { get; private set; }
      
      public static StageType GetStageById(int id)
      {
         StageType stage = InvalidStage;

         if (id == StageOne.Id)
         {
            stage = StageOne;
         }
         else if (id == StageTwo.Id)
         {
            stage = StageTwo;
         }
         else if (id == StageThree.Id)
         {
            stage = StageThree;
         }
         else
         {
            stage = StageFour;
         }

         return stage;
      }
   }
}