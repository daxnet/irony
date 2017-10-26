using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{

    public class StringLiteralTests  {


    //handy option for stringLiteral tests: we use single quotes in test strings, and they are replaced by double quotes here 
    private static string ReplaceQuotes(string input) {
      return input.Replace("'", "\"");
    }

    //The following test method and a fix are contributed by ashmind codeplex user
    [Fact]
    public void TestString_QuoteJustBeforeEof() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(new StringLiteral("String", "'"));
      token = parser.ParseInput(@"'");
      Assert.True(TokenCategory.Error == token.Terminal.Category, "Incorrect string was not parsed as syntax error.");
    }


    [Fact]
    public void TestString_Python() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(TerminalFactory.CreatePythonString("String"));
      //1. Single quotes
      token = parser.ParseInput(@"'00\a\b\t\n\v\f\r\'\\00'  ");
      Assert.True((string)token.Value == "00\a\b\t\n\v\f\r\'\\00", "Failed to process escaped characters.");
      token = parser.ParseInput("'abcd\nefg'  ");
      Assert.True(token.IsError(), "Failed to detect erroneous multi-line string.");
      token = parser.ParseInput("'''abcd\nefg'''  ");
      Assert.True((string)token.Value == "abcd\nefg", "Failed to process line break in triple-quote string.");
      token = parser.ParseInput(@"'''abcd\" + "\n" + "efg'''  ");
      Assert.True((string)token.Value == "abcd\nefg", "Failed to process escaped line-break char.");
      token = parser.ParseInput(@"r'00\a\b\t\n\v\f\r00'  ");
      Assert.True((string)token.Value == @"00\a\b\t\n\v\f\r00", "Failed to process string with disabled escapes.");
      
      //2. Double quotes - we use TryMatchDoubles which replaces single quotes with doubles and then calls TryMatch
      token = parser.ParseInput(ReplaceQuotes(@"'00\a\b\t\n\v\f\r\'\\00'  "));
      Assert.True((string)token.Value == "00\a\b\t\n\v\f\r\"\\00", "Failed to process escaped characters.");
      token = parser.ParseInput(ReplaceQuotes("'abcd\nefg'  "));
      Assert.True(token.IsError(), "Failed to detect erroneous multi-line string. (Double quotes)");
      token = parser.ParseInput(ReplaceQuotes("'''abcd\nefg'''  "));
      Assert.True((string)token.Value == "abcd\nefg", "Failed to process line break in triple-quote string. (Double quotes)");
      token = parser.ParseInput(ReplaceQuotes(@"'''abcd\" + "\n" + "efg'''  "));
      Assert.True((string)token.Value == "abcd\nefg", "Failed to process escaped line-break char. (Double quotes)");
      token = parser.ParseInput(ReplaceQuotes(@"r'00\a\b\t\n\v\f\r00'  "));
      Assert.True((string)token.Value == @"00\a\b\t\n\v\f\r00", "Failed to process string with disabled escapes. (Double quotes)");
    }//method

    [Fact]
    public void TestString_CSharp() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(TerminalFactory.CreateCSharpString("String"));

      token = parser.ParseInput('"' + @"abcd\\" + '"' + "  ");
      Assert.True((string)token.Value == @"abcd\", "Failed to process double escape char at the end of the string.");

      token = parser.ParseInput('"' + @"abcd\\\" + '"' + "efg" + '"' + "   ");
      Assert.True((string)token.Value == @"abcd\" + '"' + "efg" , @"Failed to process '\\\ + double-quote' inside the string.");

      //with Escapes
      token = parser.ParseInput(ReplaceQuotes(@"'00\a\b\t\n\v\f\r\'\\00'  "));
      Assert.True((string)token.Value == "00\a\b\t\n\v\f\r\"\\00", "Failed to process escaped characters.");
      token = parser.ParseInput(ReplaceQuotes("'abcd\nefg'  "));
      Assert.True(token.IsError(), "Failed to detect erroneous multi-line string.");
      //with disabled escapes
      token = parser.ParseInput(ReplaceQuotes(@"@'00\a\b\t\n\v\f\r00'  "));
      Assert.True((string)token.Value == @"00\a\b\t\n\v\f\r00", "Failed to process @-string with disabled escapes.");
      token = parser.ParseInput(ReplaceQuotes("@'abc\ndef'  "));
      Assert.True((string)token.Value == "abc\ndef", "Failed to process @-string with linebreak.");
      //Unicode and hex
      token = parser.ParseInput(ReplaceQuotes(@"'abc\u0040def'  "));
      Assert.True((string)token.Value == "abc@def", "Failed to process unicode escape \\u.");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\U00000040def'  "));
      Assert.True((string)token.Value == "abc@def", "Failed to process unicode escape \\u.");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\x0040xyz'  "));
      Assert.True((string)token.Value == "abc@xyz", "Failed to process hex escape (4 digits).");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\x040xyz'  "));
      Assert.True((string)token.Value == "abc@xyz", "Failed to process hex escape (3 digits).");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\x40xyz'  "));
      Assert.True((string)token.Value == "abc@xyz", "Failed to process hex escape (2 digits).");
      //octals
      token = parser.ParseInput(ReplaceQuotes(@"'abc\0601xyz'  ")); //the last digit "1" should not be included in octal number
      Assert.True((string)token.Value == "abc01xyz", "Failed to process octal escape (3 + 1 digits).");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\060xyz'  "));
      Assert.True((string)token.Value == "abc0xyz", "Failed to process octal escape (3 digits).");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\60xyz'  "));
      Assert.True((string)token.Value == "abc0xyz", "Failed to process octal escape (2 digits).");
      token = parser.ParseInput(ReplaceQuotes(@"'abc\0xyz'  "));
      Assert.True((string)token.Value == "abc\0xyz", "Failed to process octal escape (1 digit).");
    }

    [Fact]
    public void TestString_CSharpChar() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(TerminalFactory.CreateCSharpChar("Char"));
      token = parser.ParseInput("'a'  ");
      Assert.True((char)token.Value == 'a', "Failed to process char.");
      token = parser.ParseInput(@"'\n'  ");
      Assert.True((char)token.Value == '\n', "Failed to process new-line char.");
      token = parser.ParseInput(@"''  ");
      Assert.True(token.IsError(), "Failed to recognize empty quotes as invalid char literal.");
      token = parser.ParseInput(@"'abc'  ");
      Assert.True(token.IsError(), "Failed to recognize multi-char sequence as invalid char literal.");
      //Note: unlike strings, c# char literals don't allow the "@" prefix
    }

    [Fact]
    public void TestString_VB() {
      Parser parser; Token token;

      parser = TestHelper.CreateParser(TerminalFactory.CreateVbString("String"));
      //VB has no escapes - so make sure term doesn't catch any escapes
      token = parser.ParseInput(ReplaceQuotes(@"'00\a\b\t\n\v\f\r\\00'  "));
      Assert.True((string)token.Value == @"00\a\b\t\n\v\f\r\\00", "Failed to process string with \\ characters.");
      token = parser.ParseInput(ReplaceQuotes("'abcd\nefg'  "));
      Assert.True(token.IsError(), "Failed to detect erroneous multi-line string.");
      token = parser.ParseInput(ReplaceQuotes("'abcd''efg'  ")); 
      Assert.True((string)token.Value == "abcd\"efg", "Failed to process a string with doubled double-quote char.");
      //Test char suffix "c"
      token = parser.ParseInput(ReplaceQuotes("'A'c  ")); 
      Assert.True((char)token.Value == 'A', "Failed to process a character");
      token = parser.ParseInput(ReplaceQuotes("''c  "));
      Assert.True(token.IsError(), "Failed to detect an error for an empty char.");
      token = parser.ParseInput(ReplaceQuotes("'ab'C  "));
      Assert.True(token.IsError(), "Failed to detect error in multi-char sequence.");
    }

  }//class
}//namespace
