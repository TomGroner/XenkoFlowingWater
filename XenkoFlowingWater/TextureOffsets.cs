using System;
using Stride.Core.Mathematics;

namespace XenkoFlowingWater
{
  public class TextureOffsets
  {
    public float MaxDistortion;
    public float TextureScale;
    public Vector4 RandomOffsets;
    public float PulseReduction;
    private static Random random = new Random();
    private float cycleSeconds;
    private float cycleProgression = 0f;
    private bool halfWay = false;

    public TextureOffsets(float maxDistortion, float textureScale, float cycleSeconds, float pulseReduction)
    {
      MaxDistortion = maxDistortion;
      TextureScale = textureScale;
      CycleSeconds = cycleSeconds;
      PulseReduction = pulseReduction;
      Reset();
    }

    public float CurrentPhase
    {
      get { return cycleProgression / CycleSeconds; }
    }


    public float CycleSeconds
    {
      get { return cycleSeconds; }
      set
      {
        if (cycleSeconds != value)
        {
          cycleSeconds = value;
          Reset();
        }
      }
    }

    public void Update(float ellapsedSeconds)
    {
      cycleProgression += ellapsedSeconds;

      if (!halfWay && (cycleProgression > (CycleSeconds * 0.5f)))
      {
        halfWay = true;
        RandomOffsets.X = (float)random.NextDouble();
        RandomOffsets.Y = (float)random.NextDouble();
      }
      else if (cycleProgression > CycleSeconds)
      {

        RandomOffsets.Z = (float)random.NextDouble();
        RandomOffsets.W = (float)random.NextDouble();
        cycleProgression %= CycleSeconds;
        halfWay = false;
      }
    }

    private void Reset()
    {
      cycleProgression = 0f;
      halfWay = false;
    }
  }
}
