namespace Logitar.Kraken.Infrastructure;

public static class EnvironmentHelper
{
  public static bool GetBoolean(string variable, bool defaultValue = false) => TryGetBoolean(variable) ?? defaultValue;
  public static bool? TryGetBoolean(string variable, bool? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return !string.IsNullOrWhiteSpace(value) && bool.TryParse(value.Trim(), out bool booleanValue) ? booleanValue : defaultValue;
  }

  public static T GetEnum<T>(string variable, T defaultValue = default) where T : struct
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out T enumValue) ? enumValue : defaultValue;
  }

  public static int GetInt32(string variable, int defaultValue = 0) => TryGetInt32(variable) ?? defaultValue;
  public static int? TryGetInt32(string variable, int? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return !string.IsNullOrWhiteSpace(value) && int.TryParse(value.Trim(), out int integerValue) ? integerValue : defaultValue;
  }

  public static string GetString(string variable, string defaultValue = "") => TryGetString(variable) ?? defaultValue;
  public static string? TryGetString(string variable, string? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return !string.IsNullOrWhiteSpace(value) ? value.Trim() : defaultValue;
  }

  public static string[] GetStrings(string variable, string[]? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return (!string.IsNullOrWhiteSpace(value) ? JsonSerializer.Deserialize<string[]>(value.Trim()) : defaultValue) ?? [];
  }
}
