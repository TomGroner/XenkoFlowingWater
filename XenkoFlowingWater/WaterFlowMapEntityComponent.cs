using System.ComponentModel;
using Xenko.Core;
using Xenko.Engine;
using Xenko.Engine.Design;
using Xenko.Graphics;
using Xenko.Rendering.Compositing;

namespace XenkoFlowingWater
{
  [DataContract(nameof(WaterFlowMapEntityComponent))]
  [Display(nameof(WaterFlowMapEntityComponent), Expand = ExpandRule.Once)]
  [DefaultEntityComponentRenderer(typeof(WaterFlowMapEntityProcessor))]
  [ComponentOrder(100)]
  public class WaterFlowMapEntityComponent : ActivableEntityComponent
  {
    [DataMember(0)]
    [DefaultValue(RenderGroup.Group0)]
    public RenderGroup RenderGroup { get; set; }

    [DataMember(1)]
    public int Size { get; set; } = 5;

    [DataMember(2)]
    public int TesselationX { get; set; } = 5;

    [DataMember(3)]
    public int TesselationY { get; set; } = 5;

    [DataMember(2)]
    public float TextureScale { get; set; } = 1;

    [DataMember(10)]
    public LightComponent Sun { get; set; }

    [DataMember(20)]
    public SceneCameraSlotId CameraSlot { get; set; }

    [DataMember(30)]
    public Texture NoiseMapTexture { get; set; }

    [DataMember(40)]
    public Texture SkyTexture { get; set; }

    [DataMember(41)]
    public Texture WaterFloorTexture { get; set; }

    [DataMember(50)]
    public Texture FlowMapTexture { get; set; }

    [DataMember(60)]
    public Texture NormalTexture1 { get; set; }

    [DataMember(61)]
    public Texture NormalTexture2 { get; set; }

    [DataMember(70)]
    public Texture DiffuseTexture1 { get; set; }

    [DataMember(71)]
    public Texture DiffuseTexture2 { get; set; }

    [DataMember(80)]
    public float WaterTransparency { get; set; } = 0.85f;

    [DataMember(90)]
    public float DisplacementSpeed { get; set; } = 0.25f;

    [DataMember(91)]
    public float DisplacementAmplitude { get; set; } = 0.15f;

    [DataMember(100)]
    public bool UseCaustics { get; set; }
  }
}
