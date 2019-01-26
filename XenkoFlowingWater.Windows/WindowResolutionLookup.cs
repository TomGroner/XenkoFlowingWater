using System.Linq;
using System.Windows.Forms;

namespace XenkoFlowingWater.Windows
{
  public static class WindowResolutionLookup
  {
    public static int FallbackMaxWidth = 800;
    public static int FallbackMaxHeight = 600;

    public static bool DetermineMaximumResolution(out int maxWidth, out int maxHeight)
    {
      var primaryScreen = Screen.AllScreens.FirstOrDefault(screen => screen.Primary) ?? Screen.AllScreens.First();
      maxWidth = primaryScreen != null ? primaryScreen.WorkingArea.Width : FallbackMaxWidth;
      maxHeight = primaryScreen != null ? primaryScreen.WorkingArea.Height : FallbackMaxHeight;
      return primaryScreen != null;
    }
  }
}
