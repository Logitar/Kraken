using Logitar.Kraken.Contracts.Messages;

namespace Logitar.Kraken.Core.Messages;

public record Variables
{
  private readonly Dictionary<string, string> _variables;

  public Variables() : this(capacity: 0)
  {
  }

  public Variables(int capacity)
  {
    _variables = new(capacity);
  }

  public Variables(IEnumerable<Variable> variables) : this(variables.Count())
  {
    foreach (Variable variable in variables)
    {
      if (!string.IsNullOrWhiteSpace(variable.Key) && !string.IsNullOrWhiteSpace(variable.Value))
      {
        _variables[variable.Key.Trim()] = variable.Value.Trim();
      }
    }
  }

  public IReadOnlyDictionary<string, string> AsDictionary() => _variables.AsReadOnly();

  public string Resolve(string key) => _variables.TryGetValue(key.Trim(), out string? value) ? value : key;
}
