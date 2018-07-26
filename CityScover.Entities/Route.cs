namespace CityScover.Entities
{
   public class Route
   {
      #region Public properties
      public int Id { get; set; }
      public InterestPoint PointFrom { get; set; }
      public InterestPoint PointTo { get; set; }
      public ThematicScore Score { get; set; }
      public int Distance { get; set; } 
      #endregion
   }
}
