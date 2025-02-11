namespace Logitar.Kraken;

internal static class EnvironmentHelper
{
  public static string GetVariable(string variable, string defaultValue)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
  }
}
