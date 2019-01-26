namespace XenkoFlowingWater.Windows
{
  internal class XenkoFlowingWaterApp
  {
    private static void Main(string[] args)
    {
      using (var game = new XenkoFlowingWaterGame())
      {
        if (WindowResolutionLookup.DetermineMaximumResolution(out var width, out var height))
        {
          game.Services.AddService(new CustomGraphicsSettings(game, width, height));
        }

        game.Run();
      }
    }
  }
}