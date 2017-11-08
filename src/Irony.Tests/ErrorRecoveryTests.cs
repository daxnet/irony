using Irony.Parsing;
using Xunit;

namespace Irony.Tests
{
    public class ErrorRecoveryTests {

    #region Grammars
    //A simple grammar for language consisting of simple assignment statements: x=y + z; z= t + m;
    public class ErrorRecoveryGrammar : Grammar {
      public ErrorRecoveryGrammar() {
        var id = new IdentifierTerminal("id");
        var expr = new NonTerminal("expr");
        var stmt = new NonTerminal("stmt");
        var stmtList = new NonTerminal("stmt");

        base.Root = stmtList;
        stmtList.Rule = MakeStarRule(stmtList, stmt);
        stmt.Rule = id + "=" + expr + ";";
        stmt.ErrorRule = SyntaxError + ";";
        expr.Rule = id | id + "+" + id; 
      }
    }// class

    #endregion

    [Fact]
    public void TestErrorRecovery() {

      var grammar = new ErrorRecoveryGrammar();
      var parser = new Parser(grammar);
      TestHelper.CheckGrammarErrors(parser);

      //correct sample
      var parseTree = parser.Parse("x = y; y = z + m; m = n;");
      Assert.False(parseTree.HasErrors(), "Unexpected parse errors in correct source sample.");

      parseTree = parser.Parse("x = y; m = = d ; y = z + m; x = z z; m = n;");
      Assert.True(2 == parseTree.ParserMessages.Count, "Invalid # of errors.");

    }


  }//class
}//namespace
