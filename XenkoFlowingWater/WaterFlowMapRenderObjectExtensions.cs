using System.Collections.Generic;
using Stride.Graphics;

namespace XenkoFlowingWater
{
  public static class WaterFlowMapRenderObjectExtensions
  {
    public static IEnumerable<Texture> EnumerateStreamableResources(this WaterFlowMapRenderObject self)
    {
      if (self?.NoiseMapTexture != null) yield return self.NoiseMapTexture;
      if (self?.SkyTexture != null) yield return self.SkyTexture;
      if (self?.WaterFloorTexture != null) yield return self.WaterFloorTexture;
      if (self?.FlowMapTexture != null) yield return self.FlowMapTexture;
      if (self?.NormalTexture1 != null) yield return self.NormalTexture1;
      if (self?.NormalTexture2 != null) yield return self.NormalTexture2;
      if (self?.DiffuseTexture1 != null) yield return self.DiffuseTexture1;
      if (self?.DiffuseTexture2 != null) yield return self.DiffuseTexture2;
    }
  }
}
