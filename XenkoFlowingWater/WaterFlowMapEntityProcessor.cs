using Xenko.Core.Annotations;
using Xenko.Engine;
using Xenko.Games;
using Xenko.Rendering;

namespace XenkoFlowingWater
{
  public class WaterFlowMapEntityProcessor : EntityProcessor<WaterFlowMapEntityComponent, WaterFlowMapRenderObject>, IEntityComponentRenderProcessor
  {
    private const float SkyScrollSpeed = 0.01f;

    public VisibilityGroup VisibilityGroup { get; set; }

    protected override WaterFlowMapRenderObject GenerateComponentData([NotNull] Entity entity, [NotNull] WaterFlowMapEntityComponent component)
    {
      return new WaterFlowMapRenderObject(component.Size, component.RenderGroup).Copy(component);
    }

    protected override bool IsAssociatedDataValid(Entity entity, WaterFlowMapEntityComponent component, WaterFlowMapRenderObject associatedData)
    {
      return component.RenderGroup == associatedData.RenderGroup;
    }

    public override void Update(GameTime time)
    {
      foreach (var data in ComponentDatas)
      {
        var component = data.Key;
        var renderObject = data.Value;

        if (component.Enabled)
        {
          renderObject.Update(time);
        }
      }

      base.Update(time);
    }

    public override void Draw(RenderContext context)
    {
      foreach (var data in ComponentDatas)
      {
        var component = data.Key;
        var renderObject = data.Value;

        if (component.Enabled)
        {
          renderObject.Copy(component);
          VisibilityGroup.RenderObjects.Add(renderObject);
        }
        else
        {
          VisibilityGroup.RenderObjects.Remove(renderObject);
        }
      }
    }
  }
}
