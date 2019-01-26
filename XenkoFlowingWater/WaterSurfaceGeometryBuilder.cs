using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Graphics.GeometricPrimitives;

namespace XenkoFlowingWater
{
  public class WaterSurfaceGeometryBuilder
  {
    public GeometricPrimitive BuildWaterSurface(GraphicsDevice graphicsDevice, int tessellationX, int tessellationY, float size, bool generateBackFace)
    {
      var data = GenerateTerrainGeometry(tessellationX, tessellationY, size, generateBackFace);
      return new GeometricPrimitive(graphicsDevice, data);
    }

    private GeometricMeshData<VertexPositionNormalTexture> GenerateTerrainGeometry(int tessellationX, int tessellationY, float size, bool generateBackFace)
    {
      if (tessellationX < size) tessellationX = (int)size;
      if (tessellationY < size) tessellationY = (int)size;

      var lineWidth = tessellationX + 1;
      var lineHeight = tessellationY + 1;
      var vertices = new VertexPositionNormalTexture[lineWidth * lineHeight * (generateBackFace ? 2 : 1)];
      var indices = new int[tessellationX * tessellationY * 6 * (generateBackFace ? 2 : 1)];
      var deltaX = size / tessellationX;
      var deltaY = size / tessellationY;

      size /= 2.0f;

      int vertexCount = 0;
      int indexCount = 0;

      // Create vertices
      var normal = new Vector3(0.0f, 1.0f, 0.0f);
      var uv = new Vector2(1f, 1f);

      for (int y = 0; y < (tessellationY + 1); y++)
      {
        for (int x = 0; x < (tessellationX + 1); x++)
        {
          var position = new Vector3(-size + deltaX * x, 0.0f, -size + deltaY * y);
          var texCoord = new Vector2(uv.X * x / tessellationX, uv.Y * y / tessellationY);

          vertices[vertexCount++] = new VertexPositionNormalTexture(position, normal, texCoord);
        }
      }

      // Create indices
      for (int y = 0; y < tessellationY; y++)
      {
        for (int x = 0; x < tessellationX; x++)
        {
          // Six indices (two triangles) per face.
          int vbase = lineWidth * y + x;
          indices[indexCount++] = (vbase + 1);
          indices[indexCount++] = (vbase + 1 + lineWidth);
          indices[indexCount++] = (vbase + lineWidth);
          indices[indexCount++] = (vbase + 1);
          indices[indexCount++] = (vbase + lineWidth);
          indices[indexCount++] = (vbase);
        }
      }

      if (generateBackFace)
      {
        var numVertices = lineWidth * lineHeight;

        for (int y = 0; y < (tessellationY + 1); y++)
        {
          for (int x = 0; x < (tessellationX + 1); x++)
          {
            var baseVertex = vertices[vertexCount - numVertices];
            var position = new Vector3(baseVertex.Position.X, baseVertex.Position.Y, baseVertex.Position.Z);
            var texCoord = new Vector2(uv.X * x / tessellationX, uv.Y * y / tessellationY);
            vertices[vertexCount++] = new VertexPositionNormalTexture(position, -baseVertex.Normal, texCoord);
          }
        }

        for (int y = 0; y < tessellationY; y++)
        {
          for (int x = 0; x < tessellationX; x++)
          {
            int vbase = lineWidth * (y + tessellationY + 1) + x;
            indices[indexCount++] = (vbase + 1);
            indices[indexCount++] = (vbase + lineWidth);
            indices[indexCount++] = (vbase + 1 + lineWidth);
            indices[indexCount++] = (vbase + 1);
            indices[indexCount++] = (vbase);
            indices[indexCount++] = (vbase + lineWidth);
          }
        }
      }

      return new GeometricMeshData<VertexPositionNormalTexture>(vertices, indices, false) { Name = "WaterSurface" };
    }
  }
}
