using Stride.Engine;

namespace XenkoFlowingWater
{
  public class XenkoFlowingWaterGame : Game
  {
    protected override void Initialize()
    {
      base.Initialize();

      if (Services.TryGetService<CustomGraphicsSettings>(out var service))
      {
        Services.GetService<CustomGraphicsSettings>().SetMaximizedReziableWindow();
      }
    }
  }
}
