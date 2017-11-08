using Irony.Parsing;
using System.Text.RegularExpressions;
using Xunit;

namespace Irony.Tests
{

    public class RegexLiteralTests
    {

        //The following test method and a fix are contributed by ashmind codeplex user
        [Fact]
        public void TestRegExLiteral()
        {
            Parser parser; Token token;

            var term = new RegexLiteral("RegEx");
            parser = TestHelper.CreateParser(term);
            token = parser.ParseInput(@"/abc\\\/de/gm  ");
            Assert.False(token == null, "Failed to produce a token on valid string.");
            Assert.True(term == token.Terminal, "Failed to scan a string - invalid Terminal in the returned token.");
            Assert.False(token.Value == null, "Token Value field is null - should be Regex object.");
            var regex = token.Value as Regex;
            Assert.False(regex == null, "Failed to create Regex object.");
            var match = regex.Match(@"00abc\/de00");
            Assert.True(match.Index == 2, "Failed to match a regular expression");
        }

    }//class
}//namespace
