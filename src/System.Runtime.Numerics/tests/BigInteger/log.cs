// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Tools;
using Xunit;

namespace System.Numerics.Tests
{
    public class logTest
    {
        private static int s_samples = 10;
        private static Random s_random = new Random(100);

        [Fact]
        public static void RunLogTests()
        {
            byte[] tempByteArray1 = new byte[0];
            byte[] tempByteArray2 = new byte[0];
            BigInteger bi;

            // Log Method - Log(1,+Infinity)
            Assert.True((0 == BigInteger.Log(1, Double.PositiveInfinity)), " Verification Failed");

            // Log Method - Log(1,0)
            Assert.True(VerifyLogString("0 1 bLog"), " Verification Failed");

            // Log Method - Log(0, >1)
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomPosByteArray(s_random, 10);
                Assert.True(VerifyLogString(Print(tempByteArray1) + "0 bLog"), " Verification Failed");
            }

            // Log Method - Log(0, 0>x>1)
            for (int i = 0; i < s_samples; i++)
            {
                Assert.True((Double.PositiveInfinity == BigInteger.Log(0, s_random.NextDouble())), " Verification Failed");
            }

            // Log Method - base = 0
            for (int i = 0; i < s_samples; i++)
            {
                bi = 1;
                while (bi == 1)
                {
                    bi = new BigInteger(GetRandomPosByteArray(s_random, 8));
                }
                Assert.True((Double.IsNaN(BigInteger.Log(bi, 0))), " Verification Failed");
            }

            // Log Method - base = 1
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                Assert.True(VerifyLogString("1 " + Print(tempByteArray1) + "bLog"), " Verification Failed");
            }

            // Log Method - base = NaN
            for (int i = 0; i < s_samples; i++)
            {
                Assert.True((Double.IsNaN(BigInteger.Log(new BigInteger(GetRandomByteArray(s_random, 10)), Double.NaN))), " Verification Failed");
            }

            // Log Method - base = +Infinity
            for (int i = 0; i < s_samples; i++)
            {
                Assert.True((Double.IsNaN(BigInteger.Log(new BigInteger(GetRandomByteArray(s_random, 10)), Double.PositiveInfinity))), " Verification Failed");
            }

            // Log Method - Log(0,1)
            Assert.True(VerifyLogString("1 0 bLog"), " Verification Failed");

            // Log Method - base < 0
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 10);
                tempByteArray2 = GetRandomNegByteArray(s_random, 1);
                Assert.True(VerifyLogString(Print(tempByteArray2) + Print(tempByteArray1) + "bLog"), " Verification Failed");
                Assert.True((Double.IsNaN(BigInteger.Log(new BigInteger(GetRandomByteArray(s_random, 10)), -s_random.NextDouble()))), " Verification Failed");
            }

            // Log Method - value < 0
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomNegByteArray(s_random, 10);
                tempByteArray2 = GetRandomPosByteArray(s_random, 1);
                Assert.True(VerifyLogString(Print(tempByteArray2) + Print(tempByteArray1) + "bLog"), " Verification Failed");
            }

            // Log Method - Small BigInteger and 0<base<0.5 
            for (int i = 0; i < s_samples; i++)
            {
                BigInteger temp = new BigInteger(GetRandomPosByteArray(s_random, 10));
                Double newbase = Math.Min(s_random.NextDouble(), 0.5);
                Assert.True((ApproxEqual(BigInteger.Log(temp, newbase), Math.Log((double)temp, newbase))), " Verification Failed");
            }

            // Log Method - Large BigInteger and 0<base<0.5 
            for (int i = 0; i < s_samples; i++)
            {
                BigInteger temp = new BigInteger(GetRandomPosByteArray(s_random, s_random.Next(1, 100)));
                Double newbase = Math.Min(s_random.NextDouble(), 0.5);
                Assert.True((ApproxEqual(BigInteger.Log(temp, newbase), Math.Log((double)temp, newbase))), " Verification Failed");
            }

            // Log Method - two small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomPosByteArray(s_random, 2);
                tempByteArray2 = GetRandomPosByteArray(s_random, 3);
                Assert.True(VerifyLogString(Print(tempByteArray1) + Print(tempByteArray2) + "bLog"), " Verification Failed");
            }

            // Log Method - one small and one large BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomPosByteArray(s_random, 1);
                tempByteArray2 = GetRandomPosByteArray(s_random, s_random.Next(1, 100));
                Assert.True(VerifyLogString(Print(tempByteArray1) + Print(tempByteArray2) + "bLog"), " Verification Failed");
            }

            // Log Method - two large BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomPosByteArray(s_random, s_random.Next(1, 100));
                tempByteArray2 = GetRandomPosByteArray(s_random, s_random.Next(1, 100));
                Assert.True(VerifyLogString(Print(tempByteArray1) + Print(tempByteArray2) + "bLog"), " Verification Failed");
            }
        }

        private static bool VerifyLogString(string opstring)
        {
            bool ret = true;
            StackCalc sc = new StackCalc(opstring);
            while (sc.DoNextOperation())
            {
                ret &= Eval(sc.snCalc.Peek().ToString(), sc.myCalc.Peek().ToString(), String.Format("Out of Sync stacks found.  BigInteger {0} Mine {1}", sc.snCalc.Peek(), sc.myCalc.Peek()));
            }
            return ret;
        }
        private static bool VerifyIdentityString(string opstring1, string opstring2)
        {
            bool ret = true;

            StackCalc sc1 = new StackCalc(opstring1);
            while (sc1.DoNextOperation())
            {	//Run the full calculation
                sc1.DoNextOperation();
            }

            StackCalc sc2 = new StackCalc(opstring2);
            while (sc2.DoNextOperation())
            {	//Run the full calculation
                sc2.DoNextOperation();
            }

            ret &= Eval(sc1.snCalc.Peek().ToString(), sc2.snCalc.Peek().ToString(), String.Format("Out of Sync stacks found.  BigInteger1: {0} BigInteger2: {1}", sc1.snCalc.Peek(), sc2.snCalc.Peek()));

            return ret;
        }

        private static Byte[] GetRandomByteArray(Random random)
        {
            return GetRandomByteArray(random, random.Next(0, 100));
        }
        private static Byte[] GetRandomByteArray(Random random, int size)
        {
            byte[] value = new byte[size];

            for (int i = 0; i < value.Length; ++i)
            {
                value[i] = (byte)random.Next(0, 256);
            }

            return value;
        }
        private static Byte[] GetRandomPosByteArray(Random random, int size)
        {
            byte[] value = new byte[size];

            for (int i = 0; i < value.Length; i++)
            {
                value[i] = (byte)random.Next(0, 256);
            }
            value[value.Length - 1] &= 0x7F;

            return value;
        }
        private static Byte[] GetRandomNegByteArray(Random random, int size)
        {
            byte[] value = new byte[size];

            for (int i = 0; i < value.Length; ++i)
            {
                value[i] = (byte)random.Next(0, 256);
            }
            value[value.Length - 1] |= 0x80;

            return value;
        }

        private static String Print(byte[] bytes)
        {
            String ret = "make ";

            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i] + " ";
            }
            ret += "endmake ";

            return ret;
        }
        private static bool ApproxEqual(double value1, double value2)
        {
            //Special case values;
            if (Double.IsNaN(value1))
                return Double.IsNaN(value2);
            if (Double.IsNegativeInfinity(value1))
                return Double.IsNegativeInfinity(value2);
            if (Double.IsPositiveInfinity(value1))
                return Double.IsPositiveInfinity(value2);
            if (value2 == 0)
                return (value1 == 0);


            double result = Math.Abs((value1 / value2) - 1);
            return (result <= Double.Parse("1e-15"));
        }

        public static bool Eval<T>(T expected, T actual, String errorMsg)
        {
            bool retValue = expected == null ? actual == null : expected.Equals(actual);

            if (!retValue)
                return Eval(retValue, errorMsg +
                " Expected:" + (null == expected ? "<null>" : expected.ToString()) +
                " Actual:" + (null == actual ? "<null>" : actual.ToString()));

            return true;
        }
        public static bool Eval(bool expression, string message)
        {
            if (!expression)
            {
                Console.WriteLine(message);
            }

            return expression;
        }
    }
}
