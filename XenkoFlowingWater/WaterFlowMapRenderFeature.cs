using System;
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Streaming;

namespace XenkoFlowingWater
{
  public class WaterFlowMapRenderFeature : RootRenderFeature
  {
    private MutablePipelineState pipelineState;
    private DynamicEffectInstance FlowMapWaterShader;

    public override Type SupportedRenderObjectType
    {
      get => typeof(WaterFlowMapRenderObject);
    }

    public DynamicEffectInstance FlowingWaterEffect { get; set; }

    public WaterFlowMapRenderFeature()
    {
      SortKey = 0;
    }

    protected override void InitializeCore()
    {
      FlowMapWaterShader = new DynamicEffectInstance("WaterFlowMapShader");
      FlowMapWaterShader.Initialize(Context.Services);

      // Create the pipeline state and set properties that won't change
      pipelineState = new MutablePipelineState(Context.GraphicsDevice);
      pipelineState.State.SetDefaults();
      pipelineState.State.InputElements = WaterFlowMapRenderObject.VertexDeclaration.CreateInputElements();
      pipelineState.State.PrimitiveType = WaterFlowMapRenderObject.PrimitiveType;
      pipelineState.State.BlendState = BlendStates.Default;
      pipelineState.State.RasterizerState.CullMode = CullMode.None;
    }

    public override void Prepare(RenderDrawContext context)
    {
      base.Prepare(context);

      foreach (var renderObject in RenderObjects)
      {
        if (renderObject is WaterFlowMapRenderObject waterObject)
        {
          foreach (var streamableResource in waterObject.EnumerateStreamableResources())
          {
            Context.StreamingManager?.StreamResources(streamableResource, StreamingOptions.LoadAtOnce);
          }

          waterObject.Prepare(context.GraphicsDevice);
        }
      }
    }

    public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage)
    {
      base.Draw(context, renderView, renderViewStage);
    }

    public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage, int startIndex, int endIndex)
    {
      var graphicsDevice = context.GraphicsDevice;
      var graphicsContext = context.GraphicsContext;
      var commandList = context.GraphicsContext.CommandList;
      var camera = context.RenderContext.GetCurrentCamera();

      if (camera == null)
      {
        return;
      }

      FlowMapWaterShader.UpdateEffect(graphicsDevice);

      for (var index = startIndex; index < endIndex; index++)
      {
        var renderNodeReference = renderViewStage.SortedRenderNodes[index].RenderNode;
        var renderNode = GetRenderNode(renderNodeReference);
        var renderObject = (WaterFlowMapRenderObject)renderNode.RenderObject;

        if (!renderObject.IsInitialized)
        {
          continue;
        }

        var normalPhase = renderObject.NormalTextureFlow.CurrentPhase;
        var diffusePhase = renderObject.DiffuseTextureFlow.CurrentPhase;

        FlowMapWaterShader.Parameters.Set(TransformationKeys.WorldViewProjection, renderObject.World * renderView.ViewProjection);
        FlowMapWaterShader.Parameters.Set(TransformationKeys.World, renderObject.World);
        FlowMapWaterShader.Parameters.Set(GlobalKeys.Time, (float)context.RenderContext.Time.Total.TotalSeconds);
        FlowMapWaterShader.Parameters.Set(GlobalKeys.TimeStep, (float)context.RenderContext.Time.Elapsed.TotalSeconds);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.SkyTexture, renderObject.SkyTexture);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.WaterFloorTexture, renderObject.WaterFloorTexture);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NoiseTexture, renderObject.NoiseMapTexture);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.FlowMapTexture, renderObject.FlowMapTexture);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DiffuseTexture0, renderObject.DiffuseTexture1);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DiffuseTexture1, renderObject.DiffuseTexture2);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NormalTexture0, renderObject.NormalTexture1);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NormalTexture1, renderObject.NormalTexture2);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.SunColor, renderObject.SunColor);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.CameraPosition, camera.Entity.Transform.Position);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DirectionToLight, Vector3.Normalize(new Vector3(2f, 2f, 4f)));
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.WaterTransparency, renderObject.WaterTransparency);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.SkyTextureOffset, renderObject.SkyTextureOffset);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NormalOffsets, renderObject.NormalTextureFlow.RandomOffsets);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DiffuseOffsets, renderObject.DiffuseTextureFlow.RandomOffsets);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NormalPhase, new Vector2((normalPhase + 0.5f) % 1, normalPhase));
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DiffusePhase, new Vector2((normalPhase + 0.5f) % 1, normalPhase));
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.NormalPulseReduction, renderObject.NormalTextureFlow.PulseReduction);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DiffusePulseReduction, renderObject.DiffuseTextureFlow.PulseReduction);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.TextureScale, renderObject.TextureScale);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DisplacementSpeed, renderObject.DisplacementSpeed);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.DisplacementAmplitude, renderObject.DisplacementAmplitude);
        FlowMapWaterShader.Parameters.Set(WaterFlowMapShaderKeys.UseCaustics, renderObject.UseCaustics ? 1 : 0);

        // Prepare pipeline state
        pipelineState.State.RootSignature = FlowMapWaterShader.RootSignature;
        pipelineState.State.EffectBytecode = FlowMapWaterShader.Effect.Bytecode;
        pipelineState.State.Output.CaptureState(commandList);
        pipelineState.Update();
        commandList.SetPipelineState(pipelineState.CurrentState);

        renderObject.Draw(context, FlowMapWaterShader);
      }
    }
  }
}
