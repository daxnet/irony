using Irony.Interpreter.Ast;
using Irony.Parsing;
using System;

namespace Irony.SampleApp
{
    /// <summary>
    /// Represents the grammar of a custom expression.
    /// </summary>
    /// <seealso cref="Irony.Parsing.Grammar" />
    [Language("Expression Grammar", "1.0", "A simple arithmetic expression grammar.")]
    public class ExpressionGrammar : Grammar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionGrammar"/> class.
        /// </summary>
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
            var Expr = new NonTerminal("Expression");
            var Term = new NonTerminal("Term");

            var Program = new NonTerminal("Program", typeof(StatementListNode));

            Expr.Rule = Term | ParExpr | BinExpr;
            Term.Rule = number | identifier;

            ParExpr.Rule = "(" + Expr + ")";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/";

            RegisterOperators(10, "+", "-");
            RegisterOperators(20, "*", "/");

            MarkPunctuation("(", ")");
            RegisterBracePair("(", ")");
            MarkTransient(Expr, Term, BinOp, ParExpr);

            this.Root = Expr;
        }
    }
}
