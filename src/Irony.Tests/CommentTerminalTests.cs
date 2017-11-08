using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{
    public class CommentTerminalTests
    {
        [Fact]
        public void TestCommentTerminal()
        {
            Parser parser; Token token;

            parser = TestHelper.CreateParser(new CommentTerminal("Comment", "/*", "*/"));
            token = parser.ParseInput("/* abc  */");
            Assert.True(token.Category == TokenCategory.Comment, "Failed to read comment");

            parser = TestHelper.CreateParser(new CommentTerminal("Comment", "//", "\n"));
            token = parser.ParseInput("// abc  \n   ");
            Assert.True(token.Category == TokenCategory.Comment, "Failed to read line comment");

        }//method

    }//class
}//namespace
