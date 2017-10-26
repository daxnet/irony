using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Interpreter;
using Irony.Interpreter.Evaluator;
using Xunit;

namespace Irony.Tests
{

    public class EvaluatorTests
    {

        [Fact]
        public void TestEvaluator_Ops()
        {
            var eval = new ExpressionEvaluator();
            string script;
            object result;

            //Simple computation
            script = "2*3";
            result = eval.Evaluate(script);
            Assert.Equal(6, result);

            //Using variables
            script = @"
x=2
y=4
x * y
";
            result = eval.Evaluate(script);
            Assert.Equal(8, result);

            //Operator precedence
            script = @"
x=2
y=3
x + y * 5
";
            result = eval.Evaluate(script);
            Assert.Equal(17, result);

            //parenthesis
            script = @"
x=3
y=2
1 + (x - y) * 5
";
            result = eval.Evaluate(script);
            Assert.Equal(6, result);

            //strings
            script = @"
x='2'
y='3'
x + y + 4
";
            result = eval.Evaluate(script);
            Assert.Equal("234", result);

            //string with embedded expressions
            script = @"
x = 4
y = 7 
'#{x} * #{y} = #{x * y}'
";
            result = eval.Evaluate(script);
            Assert.Equal("4 * 7 = 28", result);

            //various operators
            script = @"
x = 1 + 2 * 3      # =7
y = --x            # = 6
z =  x * 1.5       # = 9
z -= y             # = 3   
";
            result = eval.Evaluate(script);
            Assert.InRange(3.0 - (double)result, -0.0001, 0.0001);

            //&&, || operators
            script = @"x = (1 > 0) || (1/0)";
            result = eval.Evaluate(script);
            Assert.Equal(true, result);

            //Operator precedence test
            script = @"2+3*3*3";
            result = eval.Evaluate(script);
            Assert.Equal(29, result);

            script = @"x = (1 < 0) && (1/0)";
            result = eval.Evaluate(script);
            Assert.Equal(false, result);
        }

        [Fact]
        public void TestEvaluator_BuiltIns()
        {
            var eval = new ExpressionEvaluator();
            string script;
            object result;

            //Using methods imported from System.Math class

            //TODO this generates System.Reflection.AmbiguousMatchException
            script = @"abs(-1.0) + Log10(100.0) + sqrt(9) + floor(4.5) + sin(PI/2)";
            result = eval.Evaluate(script);
            Assert.True(result is double, "Result is not double.");
            Assert.InRange(11.0 - (double)result, -0.001, 0.001);

            //Using methods imported from System.Environment
            script = @"report = '#{MachineName}-#{ProcessorCount}'";
            result = eval.Evaluate(script);
            var expected = string.Format("{0}-{1}", Environment.MachineName, Environment.ProcessorCount);
            Assert.Equal(expected, result);

            //Using special built-in methods print and format
            eval.ClearOutput();
            script = @"print(format('{0} * {1} = {2}', 3, 4, 3 * 4))";
            eval.Evaluate(script);
            result = eval.GetOutput();
            Assert.Equal("3 * 4 = 12\r\n", result);

            //Add custom built-in method SayHello and test it
            eval.Runtime.BuiltIns.AddMethod(SayHello, "SayHello", 1, 1, "name");
            script = @"SayHello('John')";
            result = eval.Evaluate(script);
            Assert.Equal("Hello, John!", result);
        }

        //custom built-in method added to evaluator in Built-in tests
        public static string SayHello(ScriptThread thread, object[] args)
        {
            return "Hello, " + args[0] + "!";
        }

        [Fact]
        public void TestEvaluator_Iif()
        {
            var eval = new ExpressionEvaluator();
            string script;
            object result;

            //Test '? :' operator
            script = @"1 < 0 ? 1/0 : 'false' "; // Notice that (1/0) is not evaluated
            result = eval.Evaluate(script);
            Assert.Equal("false", result);

            //Test iif special form
            script = @"iif(1 > 0, 'true', 1/0) "; //Notice that (1/0) is not evaluated
            result = eval.Evaluate(script);
            Assert.Equal("true", result);
        }

        [Fact]
        public void TestEvaluator_MemberAccess()
        {
            var eval = new ExpressionEvaluator();
            eval.Globals["foo"] = new Foo();
            string script;
            object result;

            //Test access to field, prop, calling a method
            script = @"foo.Field + ',' + foo.Prop + ',' + foo.GetStuff()";
            result = eval.Evaluate(script);
            Assert.Equal("F,P,S", result);

            script = @"
foo.Field = 'FF'
foo.Prop = 'PP'
R = foo.Field + foo.Prop ";
            result = eval.Evaluate(script);
            Assert.Equal("FFPP", result);

            //Test access to indexed properties

            //TODO this generates System.Reflection.AmbiguousMatchException
            script = @"foo[3]";
            result = eval.Evaluate(script);
            Assert.Equal("#3", result);

            //TODO this generates System.Reflection.AmbiguousMatchException
            script = @"foo['a']";
            result = eval.Evaluate(script);
            Assert.Equal("V-a", result);

            // Test with string literal
            script = @" '0123'.Substring(1) + 'abcd'.Length    ";
            result = eval.Evaluate(script);
            Assert.Equal("1234", result);
        }

        //A class used for member access testing
        public class Foo
        {
            public string Field = "F";
            public string Prop { get; set; }

            public Foo()
            {
                Prop = "P";
            }
            public string GetStuff()
            {
                return "S";
            }
            public string this[int i]
            {
                get { return "#" + i; }
                set { }
            }
            public string this[string key]
            {
                get { return "V-" + key; }
                set { }
            }
        }

        [Fact]
        public void TestEvaluator_ArrayDictDataRow()
        {
            var eval = new ExpressionEvaluator();
            //Create an array, a dictionary and a data row and add them to Globals
            eval.Globals["primes"] = new int[] { 3, 5, 7, 11, 13 };
            var nums = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            nums["one"] = "1";
            nums["two"] = "2";
            nums["three"] = "3";
            eval.Globals["nums"] = nums;
            //var t = new System.Data.DataTable();
            //t.Columns.Add("Name", typeof(string));
            //t.Columns.Add("Age", typeof(int));
            //var row = t.NewRow();
            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            row["Name"] = "John";
            row["Age"] = 30;
            eval.Globals["row"] = row;

            string script;
            object result;

            //Test array
            script = @"primes[3]";
            result = eval.Evaluate(script);
            Assert.Equal(11, result);
            script = @"
primes[3] = 12345
primes[3]";
            result = eval.Evaluate(script);
            Assert.Equal(12345, result);

            //Test dict
            script = @"nums['three'] + nums['two'] + nums['one']";
            result = eval.Evaluate(script);
            Assert.Equal("321", result);
            script = @"
nums['two'] = '22'
nums['three'] + nums['two'] + nums['one']
";
            result = eval.Evaluate(script);
            Assert.Equal("3221", result);

            //Test data row
            script = @"row['Name'] + ', ' + row['age']";
            result = eval.Evaluate(script);
            Assert.Equal("John, 30", result);
            script = @"
row['Name'] = 'Jon'
row['Name'] + ', ' + row['age']";
            result = eval.Evaluate(script);
            Assert.Equal("Jon, 30", result);
        }


    }//class
}//namespace
