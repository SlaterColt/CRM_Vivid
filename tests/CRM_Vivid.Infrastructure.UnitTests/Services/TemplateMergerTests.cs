using CRM_Vivid.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CRM_Vivid.Infrastructure.UnitTests.Services;

public class TemplateMergerTests
{
  private readonly TemplateMerger _merger;

  public TemplateMergerTests()
  {
    _merger = new TemplateMerger();
  }

  [Fact]
  public void Merge_ShouldReplaceSingleBraces_WhenKeysExist()
  {
    // Arrange
    var template = "Hello {FirstName}, welcome to {Organization}.";
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", "Slater" },
            { "Organization", "LCD" }
        };

    // Act
    var result = _merger.Merge(template, placeholders);

    // Assert
    result.Should().Be("Hello Slater, welcome to LCD.");
  }

  [Fact]
  public void Merge_ShouldReplaceDoubleBraces_WhenKeysExist()
  {
    // Arrange
    var template = "Hello {{FirstName}}.";
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", "Slater" }
        };

    // Act
    var result = _merger.Merge(template, placeholders);

    // Assert
    result.Should().Be("Hello Slater.");
  }

  [Fact]
  public void Merge_ShouldIgnoreMissingKeys()
  {
    // Arrange
    var template = "Hello {UnknownKey}.";
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", "Slater" }
        };

    // Act
    var result = _merger.Merge(template, placeholders);

    // Assert
    result.Should().Be("Hello {UnknownKey}.");
  }

  [Fact]
  public void Merge_ShouldHandleNullValuesGracefully()
  {
    // Arrange
    var template = "Hello {FirstName}.";
    var placeholders = new Dictionary<string, string>
        {
            { "FirstName", string.Empty }
        };

    // Act
    var result = _merger.Merge(template, placeholders);

    // Assert
    result.Should().Be("Hello .");
  }
}