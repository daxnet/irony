using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{

    public class FreeTextLiteralTests
    {
        //A special grammar that does not skip whitespace
        class FreeTextLiteralTestGrammar : Grammar
        {
            public string Terminator = "END";
            public FreeTextLiteralTestGrammar(Terminal terminal)
              : base(caseSensitive: true)
            {
                var rule = new BnfExpression(terminal);
                MarkReservedWords(Terminator);
                rule += Terminator;
                base.Root = new NonTerminal("Root");
                Root.Rule = rule;
            }

            //Overrides base method, effectively suppressing skipping whitespaces
            public override void SkipWhitespace(ISourceStream source)
            {
                return;
            }
        }//class

        private Parser CreateParser(Terminal terminal)
        {
            var grammar = new FreeTextLiteralTestGrammar(terminal);
            return new Parser(grammar);
        }
        private Token GetFirst(ParseTree tree)
        {
            return tree.Tokens[0];
        }

        //The following test method and a fix are contributed by ashmind codeplex user
        [Fact]
        public void TestFreeTextLiteral_Escapes()
        {
            Parser parser; Token token;

            //Escapes test
            var term = new FreeTextLiteral("FreeText", ",", ")");
            term.Escapes.Add(@"\\", @"\");
            term.Escapes.Add(@"\,", @",");
            term.Escapes.Add(@"\)", @")");

            parser = this.CreateParser(term);
            token = GetFirst(parser.Parse(@"abc\\de\,\)fg,"));
            Assert.False(token == null, "Failed to produce a token on valid string.");
            Assert.True(term == token.Terminal, "Failed to scan a string - invalid Terminal in the returned token.");
            Assert.True(token.Value.ToString() == @"abc\de,)fg", "Failed to scan a string");

            term = new FreeTextLiteral("FreeText", FreeTextOptions.AllowEof, ";");
            parser = this.CreateParser(term);
            token = GetFirst(parser.Parse(@"abcdefg"));
            Assert.False(token == null, "Failed to produce a token for free text ending at EOF.");
            Assert.True(term == token.Terminal, "Failed to scan a free text ending at EOF - invalid Terminal in the returned token.");
            Assert.True(token.Value.ToString() == @"abcdefg", "Failed to scan a free text ending at EOF");

            //The following test method and a fix are contributed by ashmind codeplex user
            //VAR
            //MESSAGE:STRING80;
            //(*_ORError Message*)
            //END_VAR
            term = new FreeTextLiteral("varContent", "END_VAR");
            term.Firsts.Add("VAR");
            parser = this.CreateParser(term);
            token = GetFirst(parser.Parse("VAR\r\nMESSAGE:STRING80;\r\n(*_ORError Message*)\r\nEND_VAR"));
            Assert.False(token == null, "Failed to produce a token on valid string.");
            Assert.True(term == token.Terminal, "Failed to scan a string - invalid Terminal in the returned token.");
            Assert.True(token.ValueString == "\r\nMESSAGE:STRING80;\r\n(*_ORError Message*)\r\n", "Failed to scan a string");

            term = new FreeTextLiteral("freeText", FreeTextOptions.AllowEof);
            parser = this.CreateParser(term);
            token = GetFirst(parser.Parse(" "));
            Assert.False(token == null, "Failed to produce a token on valid string.");
            Assert.True(term == token.Terminal, "Failed to scan a string - invalid Terminal in the returned token.");
            Assert.True(token.ValueString == " ", "Failed to scan a string");
        }

    }//class
}//namespace
