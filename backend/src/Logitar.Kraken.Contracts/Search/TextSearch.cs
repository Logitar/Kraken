namespace Logitar.Kraken.Contracts.Search;

public record TextSearch
{
  public SearchOperator Operator { get; set; }
  public List<SearchTerm> Terms { get; set; } = [];

  public TextSearch(IEnumerable<SearchTerm>? terms = null, SearchOperator @operator = SearchOperator.And)
  {
    Operator = @operator;

    if (terms != null)
    {
      Terms.AddRange(terms);
    }
  }
}
