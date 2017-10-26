using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{
    public class LineContinuationTests {

    [Fact]
    public void TestContinuationTerminal_Simple() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation", "\\"));
      token = parser.ParseInput("\\\r\t");
      Assert.True(token.Category == TokenCategory.Outline, "Failed to read simple line continuation terminal");
    }

    [Fact]
    public void TestContinuationTerminal_Default() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("_\r\n\t");
      Assert.True(token.Category == TokenCategory.Outline, "Failed to read default line continuation terminal");

      token = parser.ParseInput("\\\v    ");
      Assert.True(token.Category == TokenCategory.Outline, "Failed to read default line continuation terminal");
    }

    [Fact]
    public void TestContinuationTerminal_Complex() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation", @"\continue", @"\cont", "++CONTINUE++"));
      token = parser.ParseInput("\\cont   \r\n    ");
      Assert.True(token.Category == TokenCategory.Outline, "Failed to read complex line continuation terminal");

      token = parser.ParseInput("++CONTINUE++\t\v");
      Assert.True(token.Category == TokenCategory.Outline, "Failed to read complex line continuation terminal");
    }

    [Fact]
    public void TestContinuationTerminal_Incomplete() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("\\   garbage");
      Assert.True(token.Category == TokenCategory.Error, "Failed to read incomplete line continuation terminal");

      parser = TestHelper.CreateParser(new LineContinuationTerminal("LineContinuation"));
      token = parser.ParseInput("_");
      Assert.True(token.Category == TokenCategory.Error, "Failed to read incomplete line continuation terminal");
    }
  }
}
