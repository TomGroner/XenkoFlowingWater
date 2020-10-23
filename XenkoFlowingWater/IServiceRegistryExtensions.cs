using Stride.Core;

namespace XenkoFlowingWater
{
  public static class IServiceRegistryExtensions
  {
    public static bool TryGetService<T>(this IServiceRegistry services, out T service) where T : class
    {
      return (service = services.GetService<T>()) != null;
    }
  }
}
