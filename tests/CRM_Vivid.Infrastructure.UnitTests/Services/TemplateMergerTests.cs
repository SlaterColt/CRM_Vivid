using CRM_Vivid.Application.Common.Models;
using CRM_Vivid.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace CRM_Vivid.Infrastructure.UnitTests.Services;

public class TemplateMergerTests
{
  private readonly TemplateMerger _sut; // System Under Test

  public TemplateMergerTests()
  {
    _sut = new TemplateMerger();
  }

  [Fact]
  public void Merge_ShouldReplaceTags_WhenContactHasData()
  {
    // Arrange
    var template = "Hello {{FirstName}} {{LastName}} from {{Organization}}.";
    var contact = new ContactDto
    {
      FirstName = "Slater",
      LastName = "Colt",
      Organization = "LCD Entertainment"
    };

    // Act
    var result = _sut.Merge(template, contact);

    // Assert
    result.Should().Be("Hello Slater Colt from LCD Entertainment.");
  }

  [Fact]
  public void Merge_ShouldReplaceTagsWithEmptyString_WhenContactDataIsNull()
  {
    // Arrange
    var template = "Hi {{FirstName}}, welcome to {{Organization}}.";

    // FIX: Using null! to intentionally force null for robustness testing
    var contact = new ContactDto
    {
      FirstName = null!,
      Organization = null!
    };

    // Act
    var result = _sut.Merge(template, contact);

    // Assert
    result.Should().Be("Hi , welcome to .");
  }

  [Fact]
  public void Merge_ShouldReturnEmpty_WhenTemplateIsEmpty()
  {
    // Arrange
    var contact = new ContactDto { FirstName = "Test" };

    // Act
    var result = _sut.Merge(string.Empty, contact);

    // Assert
    result.Should().Be(string.Empty);
  }

  [Fact]
  public void Merge_ShouldHandleTemplateWithNoTags()
  {
    // Arrange
    var template = "Just a plain text.";
    var contact = new ContactDto { FirstName = "Slater" };

    // Act
    var result = _sut.Merge(template, contact);

    // Assert
    result.Should().Be("Just a plain text.");
  }
}