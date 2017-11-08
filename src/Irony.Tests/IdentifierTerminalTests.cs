using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{
    public class IdentifierTerminalTests
    {

        [Fact]
        public void TestIdentifier_CSharp()
        {
            Parser parser; Token token;

            parser = TestHelper.CreateParser(TerminalFactory.CreateCSharpIdentifier("Identifier"));
            token = parser.ParseInput("x ");
            Assert.True(token.Terminal.Name == "Identifier", "Failed to parse identifier");
            Assert.True((string)token.Value == "x", "Failed to parse identifier");
            token = parser.ParseInput("_a01 ");
            Assert.True(token.Terminal.Name == "Identifier", "Failed to parse identifier starting with _");
            Assert.True((string)token.Value == "_a01", "Failed to parse identifier starting with _");

            token = parser.ParseInput("0abc ");
            Assert.True(token.IsError(), "Erroneously recognized an identifier.");

            token = parser.ParseInput(@"_\u0061bc ");
            Assert.True(token.Terminal.Name == "Identifier", "Failed to parse identifier starting with _");
            Assert.True((string)token.Value == "_abc", "Failed to parse identifier containing escape sequence \\u");

            token = parser.ParseInput(@"a\U00000062c_ ");
            Assert.True(token.Terminal.Name == "Identifier", "Failed to parse identifier starting with _");
            Assert.True((string)token.Value == "abc_", "Failed to parse identifier containing escape sequence \\U");
        }//method

        [Fact]
        public void TestIdentifier_CaseRestrictions()
        {
            Parser parser; Token token;

            var id = new IdentifierTerminal("identifier");
            id.CaseRestriction = CaseRestriction.None;
            parser = TestHelper.CreateParser(id);

            token = parser.ParseInput("aAbB");
            Assert.True(token != null, "Failed to scan an identifier aAbB.");

            id.CaseRestriction = CaseRestriction.FirstLower;
            parser = TestHelper.CreateParser(id);
            token = parser.ParseInput("BCD");
            Assert.True(token.IsError(), "Erroneously recognized an identifier BCD with FirstLower restriction.");
            token = parser.ParseInput("bCd ");
            Assert.True(token != null && token.ValueString == "bCd", "Failed to scan identifier bCd with FirstLower restriction.");

            id.CaseRestriction = CaseRestriction.FirstUpper;
            parser = TestHelper.CreateParser(id);
            token = parser.ParseInput("cDE");
            Assert.True(TokenCategory.Error == token.Category, "Erroneously recognized an identifier cDE with FirstUpper restriction.");
            token = parser.ParseInput("CdE");
            Assert.True(token != null && token.ValueString == "CdE", "Failed to scan identifier CdE with FirstUpper restriction.");

            id.CaseRestriction = CaseRestriction.AllLower;
            parser = TestHelper.CreateParser(id);
            token = parser.ParseInput("DeF");
            Assert.True(token.IsError(), "Erroneously recognized an identifier DeF with AllLower restriction.");
            token = parser.ParseInput("def");
            Assert.True(token != null && token.ValueString == "def", "Failed to scan identifier def with AllLower restriction.");

            id.CaseRestriction = CaseRestriction.AllUpper;
            parser = TestHelper.CreateParser(id);
            token = parser.ParseInput("EFg ");
            Assert.True(token.IsError(), "Erroneously recognized an identifier EFg with AllUpper restriction.");
            token = parser.ParseInput("EFG");
            Assert.True(token != null && token.ValueString == "EFG", "Failed to scan identifier EFG with AllUpper restriction.");
        }//method


    }//class
}//namespace


