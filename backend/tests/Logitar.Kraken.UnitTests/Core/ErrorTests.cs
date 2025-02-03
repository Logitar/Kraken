namespace Logitar.Kraken.Core;

[Trait(Traits.Category, Categories.Unit)]
public class ErrorTests
{
  [Fact(DisplayName = "It should construct the correct error with data.")]
  public void Given_Data_When_ctor_Then_ErrorConstructed()
  {
    Error error = new("code", "message",
    [
      new KeyValuePair<string, object?>("Test", "Hello World!"),
      new KeyValuePair<string, object?>("Note", 100.0),
      new KeyValuePair<string, object?>("Note", 78.0)
    ]);
    Assert.Equal("code", error.Code);
    Assert.Equal("message", error.Message);

    Assert.Equal(2, error.Data.Count);
    Assert.Equal("Hello World!", error.Data["Test"]);
    Assert.Equal(78.0, error.Data["Note"]);
  }

  [Fact(DisplayName = "It should construct the correct error without data.")]
  public void Given_NoData_When_ctor_Then_ErrorConstructed()
  {
    Error error = new("code", "message");
    Assert.Equal("code", error.Code);
    Assert.Equal("message", error.Message);
    Assert.Empty(error.Data);
  }
}
