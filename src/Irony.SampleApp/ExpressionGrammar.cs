using Irony.Interpreter.Ast;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irony.SampleApp
{
    [Language("Spotfire Expression Grammar", "1.0", "abc")]
    public class ExpressionGrammar : Grammar
    {
        public ExpressionGrammar() : base(false)
        {
            var number = new NumberLiteral("Number");
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int16, TypeCode.Int32, TypeCode.Int64 };
            number.DefaultFloatType = TypeCode.Single;

            var identifier = new IdentifierTerminal("Identifier");
            var comma = ToTerm(",");

            var BinOp = new NonTerminal("BinaryOperator", "operator");
            var ParExpr = new NonTerminal("ParenthesisExpression");
            var BinExpr = new NonTerminal("BinaryExpression", typeof(BinaryOperationNode));
            var ArgList = new NonTerminal("ArgumentList", typeof(ExpressionListNode));
            var Arg = new NonTerminal("Argument");
            var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
            var Expr = new NonTerminal("Expression");
            var Term = new NonTerminal("Term");

            var Program = new NonTerminal("Program", typeof(StatementListNode));

            Expr.Rule = Term | ParExpr | BinExpr;
            Term.Rule = number | FunctionCall | identifier;
            Arg.Rule = Expr;
            ArgList.Rule = MakeStarRule(ArgList, comma, Arg);
            FunctionCall.Rule = Expr + PreferShiftHere() + "(" + ArgList + ")";
            ParExpr.Rule = "(" + Expr + ")";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/";

            RegisterOperators(10, "+", "-");
            RegisterOperators(20, "*", "/");

            MarkPunctuation("(", ")", "${", "}", "[", "]");
            RegisterBracePair("(", ")");
            RegisterBracePair("[", "]");
            MarkTransient(Expr, Term, BinOp, ParExpr, ArgList);

            this.Root = Expr;

        }


    }
}
