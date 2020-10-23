using Stride.Core.Mathematics;

namespace XenkoFlowingWater
{
  public class CustomGraphicsSettings
  {
    private int? PrimaryScreenWidth;
    private int? PrimaryScreenHeight;

    public CustomGraphicsSettings(XenkoFlowingWaterGame game, int? maxScreenWidth, int? maxScreenHeight)
    {
      Game = game;
      PrimaryScreenWidth = maxScreenWidth;
      PrimaryScreenHeight = maxScreenHeight;
    }

    public XenkoFlowingWaterGame Game { get; }

    public void SetMaximizedReziableWindow()
    {
      Game.Window.AllowUserResizing = true;
      Game.Window.BeginScreenDeviceChange(true);
      Game.Window.EndScreenDeviceChange(PrimaryScreenWidth.Value, PrimaryScreenHeight.Value);
      Game.Window.Position = new Int2(0, 0);
    }
  }
}
