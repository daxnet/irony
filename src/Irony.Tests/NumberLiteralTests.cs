//Authors: Roman Ivantsov, Philipp Serr

using Irony.Parsing;
using System;
using Xunit;

namespace Irony.Tests
{

    public class NumberLiteralTests
    {

        [Fact]
        public void TestNumber_General()
        {
            Parser parser; Token token;

            NumberLiteral number = new NumberLiteral("Number");
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            parser = TestHelper.CreateParser(number);
            token = parser.ParseInput("123");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 123, "Failed to read int value");
            token = parser.ParseInput("123.4");
            Assert.True(Math.Abs(Convert.ToDouble(token.Value) - 123.4) < 0.000001, "Failed to read float value");
            //100 digits
            string sbig = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            token = parser.ParseInput(sbig);
            Assert.True(token.Value.ToString() == sbig, "Failed to read big integer value");
        }//method

        //The following "sign" test methods and a fix are contributed by ashmind codeplex user
        [Fact]
        public void TestNumber_SignedDoesNotMatchSingleMinus()
        {
            Parser parser; Token token;

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            parser = TestHelper.CreateParser(number);
            token = parser.ParseInput("-");
            Assert.True(token.IsError(), "Parsed single '-' as a number value.");
        }

        [Fact]
        public void TestNumber_SignedDoesNotMatchSinglePlus()
        {
            Parser parser; Token token;

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            parser = TestHelper.CreateParser(number);
            token = parser.ParseInput("+");
            Assert.True(token.IsError(), "Parsed single '+' as a number value.");
        }

        [Fact]
        public void TestNumber_SignedMatchesNegativeCorrectly()
        {
            Parser parser; Token token;

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            parser = TestHelper.CreateParser(number);
            token = parser.ParseInput("-500");
            Assert.Equal(-500, token.Value);
        }

        [Fact]
        public void TestNumber_CSharp()
        {
            Parser parser; Token token;

            double eps = 0.0001;
            parser = TestHelper.CreateParser(TerminalFactory.CreateCSharpNumber("Number"));

            //Simple integers and suffixes
            token = parser.ParseInput("123 ");
            CheckType(token, typeof(int));
            Assert.True(token.Details != null, "ScanDetails object not found in token.");
            Assert.True((int)token.Value == 123, "Failed to read int value");

            token = parser.ParseInput(Int32.MaxValue.ToString());
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == Int32.MaxValue, "Failed to read Int32.MaxValue.");

            token = parser.ParseInput(UInt64.MaxValue.ToString());
            CheckType(token, typeof(ulong));
            Assert.True((ulong)token.Value == UInt64.MaxValue, "Failed to read uint64.MaxValue value");

            token = parser.ParseInput("123U ");
            CheckType(token, typeof(UInt32));
            Assert.True((UInt32)token.Value == 123, "Failed to read uint value");

            token = parser.ParseInput("123L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 123, "Failed to read long value");

            token = parser.ParseInput("123uL ");
            CheckType(token, typeof(ulong));
            Assert.True((ulong)token.Value == 123, "Failed to read ulong value");

            //Hex representation
            token = parser.ParseInput("0x012 ");
            CheckType(token, typeof(Int32));
            Assert.True((Int32)token.Value == 0x012, "Failed to read hex int value");

            token = parser.ParseInput("0x12U ");
            CheckType(token, typeof(UInt32));
            Assert.True((UInt32)token.Value == 0x012, "Failed to read hex uint value");

            token = parser.ParseInput("0x012L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 0x012, "Failed to read hex long value");

            token = parser.ParseInput("0x012uL ");
            CheckType(token, typeof(ulong));
            Assert.True((ulong)token.Value == 0x012, "Failed to read hex ulong value");

            //Floating point types
            token = parser.ParseInput("123.4 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #1");

            token = parser.ParseInput("1234e-1 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 1234e-1) < eps, "Failed to read double value #2");

            token = parser.ParseInput("12.34e+01 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #3");

            token = parser.ParseInput("0.1234E3 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #4");

            token = parser.ParseInput("123.4f ");
            CheckType(token, typeof(float));
            Assert.True(Math.Abs((Single)token.Value - 123.4) < eps, "Failed to read float(single) value");

            token = parser.ParseInput("123.4m ");
            CheckType(token, typeof(decimal));
            Assert.True(Math.Abs((decimal)token.Value - 123.4m) < Convert.ToDecimal(eps), "Failed to read decimal value");

            token = parser.ParseInput("123. ", useTerminator: false); //should ignore dot and read number as int. compare it to python numbers - see below
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 123, "Failed to read int value with trailing dot");

            //Quick parse
            token = parser.ParseInput("1 ");
            CheckType(token, typeof(int));
            //When going through quick parse path (for one-digit numbers), the NumberScanInfo record is not created and hence is absent in Attributes
            Assert.True(token.Details == null, "Quick parse test failed: ScanDetails object is found in token - quick parse path should not produce this object.");
            Assert.True((int)token.Value == 1, "Failed to read quick-parse value");
        }

        [Fact]
        public void TestNumber_VB()
        {
            Parser parser; Token token;

            double eps = 0.0001;
            parser = TestHelper.CreateParser(TerminalFactory.CreateVbNumber("Number"));

            //Simple integer
            token = parser.ParseInput("123 ");
            CheckType(token, typeof(int));
            Assert.True(token.Details != null, "ScanDetails object not found in token.");
            Assert.True((int)token.Value == 123, "Failed to read int value");

            //Test all suffixes
            token = parser.ParseInput("123S ");
            CheckType(token, typeof(Int16));
            Assert.True((Int16)token.Value == 123, "Failed to read short value");

            token = parser.ParseInput("123I ");
            CheckType(token, typeof(Int32));
            Assert.True((Int32)token.Value == 123, "Failed to read int value");

            token = parser.ParseInput("123% ");
            CheckType(token, typeof(Int32));
            Assert.True((Int32)token.Value == 123, "Failed to read int value");

            token = parser.ParseInput("123L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 123, "Failed to read long value");

            token = parser.ParseInput("123& ");
            CheckType(token, typeof(Int64));
            Assert.True((Int64)token.Value == 123, "Failed to read long value");

            token = parser.ParseInput("123us ");
            CheckType(token, typeof(UInt16));
            Assert.True((UInt16)token.Value == 123, "Failed to read ushort value");

            token = parser.ParseInput("123ui ");
            CheckType(token, typeof(UInt32));
            Assert.True((UInt32)token.Value == 123, "Failed to read uint value");

            token = parser.ParseInput("123ul ");
            CheckType(token, typeof(ulong));
            Assert.True((ulong)token.Value == 123, "Failed to read ulong value");

            //Hex and octal 
            token = parser.ParseInput("&H012 ");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 0x012, "Failed to read hex int value");

            token = parser.ParseInput("&H012L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 0x012, "Failed to read hex long value");

            token = parser.ParseInput("&O012 ");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 10, "Failed to read octal int value"); //12(oct) = 10(dec)

            token = parser.ParseInput("&o012L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 10, "Failed to read octal long value");

            //Floating point types
            token = parser.ParseInput("123.4 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #1");

            token = parser.ParseInput("1234e-1 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 1234e-1) < eps, "Failed to read double value #2");

            token = parser.ParseInput("12.34e+01 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #3");

            token = parser.ParseInput("0.1234E3 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #4");

            token = parser.ParseInput("123.4R ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #5");

            token = parser.ParseInput("123.4# ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #6");

            token = parser.ParseInput("123.4f ");
            CheckType(token, typeof(float));
            Assert.True(Math.Abs((Single)token.Value - 123.4) < eps, "Failed to read float(single) value");

            token = parser.ParseInput("123.4! ");
            CheckType(token, typeof(float));
            Assert.True(Math.Abs((Single)token.Value - 123.4) < eps, "Failed to read float(single) value");

            token = parser.ParseInput("123.4D ");
            CheckType(token, typeof(decimal));
            Assert.True(Math.Abs((decimal)token.Value - 123.4m) < Convert.ToDecimal(eps), "Failed to read decimal value");

            token = parser.ParseInput("123.4@ ");
            CheckType(token, typeof(decimal));
            Assert.True(Math.Abs((decimal)token.Value - 123.4m) < Convert.ToDecimal(eps), "Failed to read decimal value");

            //Quick parse
            token = parser.ParseInput("1 ");
            CheckType(token, typeof(int));
            //When going through quick parse path (for one-digit numbers), the NumberScanInfo record is not created and hence is absent in Attributes
            Assert.True(token.Details == null, "Quick parse test failed: ScanDetails object is found in token - quick parse path should not produce this object.");
            Assert.True((int)token.Value == 1, "Failed to read quick-parse value");
        }


        [Fact]
        public void TestNumber_Python()
        {
            Parser parser; Token token;

            double eps = 0.0001;
            parser = TestHelper.CreateParser(TerminalFactory.CreatePythonNumber("Number"));

            //Simple integers and suffixes
            token = parser.ParseInput("123 ");
            CheckType(token, typeof(int));
            Assert.True(token.Details != null, "ScanDetails object not found in token.");
            Assert.True((int)token.Value == 123, "Failed to read int value");

            token = parser.ParseInput("123L ");
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 123, "Failed to read long value");

            //Hex representation
            token = parser.ParseInput("0x012 ");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 0x012, "Failed to read hex int value");

            token = parser.ParseInput("0x012l "); //with small "L"
            CheckType(token, typeof(long));
            Assert.True((long)token.Value == 0x012, "Failed to read hex long value");

            //Floating point types
            token = parser.ParseInput("123.4 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #1");

            token = parser.ParseInput("1234e-1 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 1234e-1) < eps, "Failed to read double value #2");

            token = parser.ParseInput("12.34e+01 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #3");

            token = parser.ParseInput("0.1234E3 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #4");

            token = parser.ParseInput(".1234 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 0.1234) < eps, "Failed to read double value with leading dot");

            token = parser.ParseInput("123. ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.0) < eps, "Failed to read double value with trailing dot");

            //Big integer
            string sbig = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"; //100 digits
            token = parser.ParseInput(sbig);
            Assert.True(token.Value.ToString() == sbig, "Failed to read big integer value");

            //Quick parse
            token = parser.ParseInput("1 ");
            CheckType(token, typeof(int));
            Assert.True(token.Details == null, "Quick parse test failed: ScanDetails object is found in token - quick parse path should produce this object.");
            Assert.True((int)token.Value == 1, "Failed to read quick-parse value");

        }

        [Fact]
        public void TestNumber_Scheme()
        {
            Parser parser; Token token;

            double eps = 0.0001;
            parser = TestHelper.CreateParser(TerminalFactory.CreateSchemeNumber("Number"));


            //Just test default float value (double), and exp symbols (e->double, s->single, d -> double)
            token = parser.ParseInput("123.4 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value #1");

            token = parser.ParseInput("1234e-1 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 1234e-1) < eps, "Failed to read single value #2");

            token = parser.ParseInput("1234s-1 ");
            CheckType(token, typeof(Single));
            Assert.True(Math.Abs((Single)token.Value - 1234e-1) < eps, "Failed to read single value #3");

            token = parser.ParseInput("12.34d+01 ");
            CheckType(token, typeof(double));
            Assert.True(Math.Abs((double)token.Value - 123.4) < eps, "Failed to read double value  #4");
        }//method

        [Fact]
        public void TestNumber_WithUnderscore()
        {
            Parser parser; Token token;

            var number = new NumberLiteral("number", NumberOptions.AllowUnderscore);
            parser = TestHelper.CreateParser(number);

            //Simple integers and suffixes
            token = parser.ParseInput("1_234_567");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == 1234567, "Failed to read int value with underscores.");
        }//method


        //There was a bug discovered in NumberLiteral - it cannot parse appropriately the int.MinValue value.
        // This test ensures that the issue is fixed.
        [Fact]
        public void TestNumber_MinMaxValues()
        {
            Parser parser; Token token;

            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32 };
            parser = TestHelper.CreateParser(number);
            var s = int.MinValue.ToString();
            token = parser.ParseInput(s);
            Assert.False(token.IsError(), "Failed to scan int.MinValue, scanner returned an error.");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == int.MinValue, "Failed to scan int.MinValue, scanned value does not match.");
            s = int.MaxValue.ToString();
            token = parser.ParseInput(s);
            Assert.False(token.IsError(), "Failed to scan int.MaxValue, scanner returned an error.");
            CheckType(token, typeof(int));
            Assert.True((int)token.Value == int.MaxValue, "Failed to read int.MaxValue");
        }//method

        private void CheckType(Token token, Type type)
        {
            Assert.False(token == null, "TryMatch returned null, while token was expected.");
            Type vtype = token.Value.GetType();
            Assert.True(vtype == type, "Invalid target type, expected " + type.ToString() + ", found:  " + vtype);
        }


    }//class
}//namespace
