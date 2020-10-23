using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Graphics.GeometricPrimitives;
using Stride.Rendering;

namespace XenkoFlowingWater
{
  public class WaterFlowMapRenderObject : RenderObject
  {
    private const float SkyScrollSpeed = 0.01f;
    public static readonly VertexDeclaration VertexDeclaration = VertexPositionNormalTexture.Layout;
    public static readonly PrimitiveType PrimitiveType = PrimitiveType.TriangleStrip;

    public WaterFlowMapRenderObject(int size, RenderGroup rendergroup)
    {
      Size = System.Math.Max(1, size);
      RenderGroup = rendergroup;
      SkyTextureOffset = new Vector2(0.0f, 0.0f);
      NormalTextureFlow = new TextureOffsets(0.4f, 0.5f, 10.5f, 0.5f);
      DiffuseTextureFlow = new TextureOffsets(0.4f, 0.5f, 10.0f, 0.5f);
    }

    // Component-link fields
    public int Size;
    public int TesselationX;
    public int TesselationY;
    public LightComponent Sun;
    public Texture NoiseMapTexture;
    public Texture SkyTexture;
    public Texture WaterFloorTexture;
    public Texture FlowMapTexture;
    public Texture NormalTexture1;
    public Texture NormalTexture2;
    public Texture DiffuseTexture1;
    public Texture DiffuseTexture2;
    public float WaterTransparency = 0.85f;
    public float DisplacementSpeed = 0.25f;
    public float DisplacementAmplitude = 0.15f;
    public float TextureScale;
    public bool UseCaustics;

    // Water fields
    public Vector2 SkyTextureOffset;
    public TextureOffsets NormalTextureFlow;
    public TextureOffsets DiffuseTextureFlow;

    // Rendering properties
    public Matrix World;
    public Color3 SunColor = Color.White.ToColor3();
    public CameraComponent Camera;
    public GeometricPrimitive SurfaceGeometry;

    public bool IsInitialized
    {
      get => SurfaceGeometry != null;
    }

    public WaterFlowMapRenderObject Copy(WaterFlowMapEntityComponent component)
    {
      TextureScale = System.Math.Max(1.0f, component.TextureScale);
      Sun = component.Sun;
      NoiseMapTexture = component.NoiseMapTexture;
      SkyTexture = component.SkyTexture;
      WaterFloorTexture = component.WaterFloorTexture;
      FlowMapTexture = component.FlowMapTexture;
      NormalTexture1 = component.NormalTexture1;
      NormalTexture2 = component.NormalTexture2;
      DiffuseTexture1 = component.DiffuseTexture1;
      DiffuseTexture2 = component.DiffuseTexture2;
      WaterTransparency = component.WaterTransparency;
      DisplacementSpeed = component.DisplacementSpeed;
      DisplacementAmplitude = component.DisplacementAmplitude;
      World = component.Entity.Transform.WorldMatrix;
      UseCaustics = component.UseCaustics;

      if (component.Size != Size || component.TesselationX != TesselationX || component.TesselationY != TesselationY)
      {
        Size = component.Size;
        TesselationX = component.TesselationX;
        TesselationY = component.TesselationY;
        RemoveGeometry();
      }

      return this;
    }

    public void Prepare(GraphicsDevice graphicsDevice)
    {
      if (!IsInitialized)
      {
        CreateGeometricPrimitiveGeometry(graphicsDevice);
      }
    }

    public void Update(GameTime time)
    {
      var ellapsedSeconds = (float)time.Elapsed.TotalSeconds;
      SkyTextureOffset.X = (SkyTextureOffset.X + SkyScrollSpeed * ellapsedSeconds) % 1.0f;
      NormalTextureFlow.Update(ellapsedSeconds);
      DiffuseTextureFlow.Update(ellapsedSeconds);
    }

    public void Draw(RenderDrawContext context, EffectInstance effect)
    {
      SurfaceGeometry.Draw(context.GraphicsContext, effect);
    }

    private void CreateGeometricPrimitiveGeometry(GraphicsDevice graphicsDevice)
    {
      if (SurfaceGeometry != null) return;
      if (TesselationX == 0 || TesselationY == 0 || Size == 0) return;
      SurfaceGeometry = new WaterSurfaceGeometryBuilder().BuildWaterSurface(graphicsDevice, TesselationX, TesselationY, Size, false);
      SurfaceGeometry.PipelineState.State.SetDefaults();
      SurfaceGeometry.PipelineState.State.BlendState = BlendStates.AlphaBlend;
      SurfaceGeometry.PipelineState.State.RasterizerState.CullMode = CullMode.None;
    }

    private void RemoveGeometry()
    {
      SurfaceGeometry?.Dispose();
      SurfaceGeometry = null;
    }
  }
}
