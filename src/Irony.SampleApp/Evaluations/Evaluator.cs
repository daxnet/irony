using Irony.Parsing;
using System;
using System.Text;

namespace Irony.SampleApp.Evaluations
{
    internal sealed class Evaluator
    {
        public Evaluation Evaluate(string input)
        {
            var language = new LanguageData(new ExpressionGrammar());
            var parser = new Parser(language);
            var syntaxTree = parser.Parse(input);

            if (syntaxTree.HasErrors())
            {
                throw new InvalidOperationException(BuildParsingErrorMessage(syntaxTree.ParserMessages));
            }

            return PerformEvaluate(syntaxTree.Root);
        }

        private Evaluation PerformEvaluate(ParseTreeNode node)
        {
            switch (node.Term.Name)
            {
                case "BinaryExpression":
                    var leftNode = node.ChildNodes[0];
                    var opNode = node.ChildNodes[1];
                    var rightNode = node.ChildNodes[2];
                    Evaluation left = PerformEvaluate(leftNode);
                    Evaluation right = PerformEvaluate(rightNode);
                    BinaryOperation op = BinaryOperation.Add;
                    switch (opNode.Term.Name)
                    {
                        case "+":
                            op = BinaryOperation.Add;
                            break;
                        case "-":
                            op = BinaryOperation.Sub;
                            break;
                        case "*":
                            op = BinaryOperation.Mul;
                            break;
                        case "/":
                            op = BinaryOperation.Div;
                            break;
                    }
                    return new BinaryEvaluation(left, right, op);
                case "Number":
                    var value = Convert.ToSingle(node.Token.Text);
                    return new ConstantEvaluation(value);
            }

            throw new InvalidOperationException($"Unrecognizable term {node.Term.Name}.");
        }

        private static string BuildParsingErrorMessage(LogMessageList messages)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Parsing failed with the following errors:");
            messages.ForEach(msg => sb.AppendLine($"\t{msg.Message}"));
            return sb.ToString();
        }
    }
}
