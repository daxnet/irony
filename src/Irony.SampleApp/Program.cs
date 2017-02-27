using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irony.SampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lang = new LanguageData(new ExpressionGrammar());
            var parser = new Parser(lang);
            var tree = parser.Parse("2+3");
            Console.WriteLine(tree.Status);
        }
    }
}
