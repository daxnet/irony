using Irony.SampleApp.Evaluations;
using System;

namespace Irony.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var evaluator = new Evaluator();
            var evaluation = evaluator.Evaluate("2.5+(3-1)*5");
            Console.WriteLine(evaluation.Value);
        }
    }
}
