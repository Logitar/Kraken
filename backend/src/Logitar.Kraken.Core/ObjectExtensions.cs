namespace Logitar.Kraken.Core;

internal static class ObjectExtensions
{
  public static T DeepClone<T>(this T value) where T : notnull
  {
    Type type = value.GetType();
    string json = JsonSerializer.Serialize(value, type);
    return (T?)JsonSerializer.Deserialize(json, type) ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }
}
