using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {

            Miki.Miki.RunCalcTest();
            //Passwords.Passwords.PassFromDig(); //Good for long testing - Generate 32-char password from Int-string-number and Int-string-number from password

        }
    }

}

namespace Passwords
{
    class Passwords
    {
        public static void PassFromDig()
        {
            //generate long string-number and convert to 32-char password
            //based on Lehmer algorithm
            //very good to test calculator
            string Xseed = "0";
            string PassCharCount = "62";
            string PassCharPlaces = "32";
            string IncrementPower = "108"; //108
            string MaxValue = Miki.CalcStrings.Add(Miki.CalcStrings.Pow(PassCharCount, PassCharPlaces)[0], "1")[0];
            string Increment = Miki.CalcStrings.Pow("3", IncrementPower)[0];

            long ElapsSec = 0;

            Console.WriteLine(MaxValue);
            Console.WriteLine(Increment);

            int k = 0;
            int i = 1;

            var sw = Stopwatch.StartNew();
            string Xnew = Xseed;
            string Xtemp = Xnew;
            while (true)
            {
                List<char> MyCharList = new(new char[32]);

                k = 0;

                for (int h = 31; h >= 0; h--)

                {
                    List<string> temp = new(Miki.CalcStrings.Div(Xtemp, "62"));
                    int TempInt = Convert.ToInt32(temp[1]);

                    if (TempInt < 10)
                    {
                        MyCharList[h] = ((char)(TempInt + 48));
                        k++;
                    }
                    if (TempInt >= 10 && TempInt < 36)
                    {
                        MyCharList[h] = ((char)(TempInt + 55));
                        k++;
                    }

                    if (TempInt > 35)
                    {
                        MyCharList[h] = ((char)(TempInt + 61));
                        k++;
                    }

                    Xtemp = temp[0];
                }

                string Pass = string.Join("", MyCharList);

                if (Xnew == DigFromPass(Pass) == false) //check if correct
                { Console.ReadKey(); }

                Xnew = Miki.CalcStrings.Div(Miki.CalcStrings.Add(Xnew, Increment)[0], MaxValue)[1];
                Xtemp = Xnew;

                i += 1;

                if ((i) % 1000 == 0)
                {
                    ElapsSec = sw.ElapsedMilliseconds;
                    Console.WriteLine(string.Format("{0}      {1}       Time / 1000: {2}", Pass, i, ElapsSec));
                    sw = Stopwatch.StartNew();
                }
            }

        }
        public static string DigFromPass(string MyPass)
        {
            //convert 32-char password to string-number
            string Dig = "0";
            string TempDig = "0";
            List<char> CharList = new List<char>(MyPass);
            int k = 0;

            for (int i = CharList.Count - 1; i >= 0; i--)
            {
                int CharDec = CharList[i];
                int CharInt = 0;
                if (CharDec > 96 && CharDec < 123)
                {
                    CharInt = CharDec - 61;
                }
                if (CharDec > 64 && CharDec < 91)
                {
                    CharInt = CharDec - 55;
                }

                if (CharDec > 47 && CharDec < 58)
                {
                    CharInt = CharDec - 48;
                }

                TempDig = Miki.CalcStrings.Mul(Convert.ToString(CharInt), Miki.CalcStrings.Pow("62", Convert.ToString(k))[0])[0];

                Dig = Miki.CalcStrings.Add(Dig, TempDig)[0];
                k++;
            }

            return Dig;
        }
    }



}
namespace Miki
{
    class Miki
    {
        //Mega-Integer-string (K)Caluculator Intefrace - Miki...
        //for my son, Miki :-*

        
        public static void RunCalcTest()
        {
            //MikiAdd, MikiSub, MikiMul - returns [0]-Int-string-Result, [1]-Abs(Int-string-Result)
            //MikiDiv - returns [0]-Int-string-Result, [1]-Rest from division
            //MikiPow - returns [0]-Int-string-Result

            List<string> MyOutputList = new();
            string[] TestCalc = new string[2];
            var sw = Stopwatch.StartNew();

            string Dig1 = "31543890397977681684285524423227768626815861760032026538";
            string Dig2 = "17";

            Console.WriteLine(Dig1 + "  Length: " + Dig1.Length);
            Console.WriteLine(Dig2 + "  Length: " + Dig2.Length);
            List<string> MyOutputListM = new();
            List<string> MyOutputListA = new();


            //to speedup CPU
            _ = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {

                _ = CalcStrings.Sub(Dig1, Dig2);
                _ = CalcStrings.Div(Dig1, Dig2);
                _ = CalcStrings.Mul(Dig1, Dig2);
                _ = CalcStrings.Add(Dig1, Dig2);

            }


            Console.WriteLine("++++++++++++++++++++++++++++++");
            Console.WriteLine("-------ADD-------");
            sw = Stopwatch.StartNew();
            MyOutputList = CalcStrings.Add(Dig1, Dig2);
            sw.Stop();
            Console.WriteLine(string.Format("Ticks:  {0}    ms:  {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----ADD CHECK-----");
            MyOutputListA = CalcStrings.Sub(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");


            Console.WriteLine("-------SUB-------");
            sw = Stopwatch.StartNew();
            MyOutputList = CalcStrings.Sub(Dig1, Dig2);
            sw.Stop();
            Console.WriteLine(string.Format("Ticks:  {0}    ms:  {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----SUB CHECK-----");
            MyOutputListA = CalcStrings.Add(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");


            Console.WriteLine("-------MUL-------");
            sw = Stopwatch.StartNew();
            MyOutputList = CalcStrings.Mul(Dig1, Dig2);
            sw.Stop();
            Console.WriteLine(string.Format("Ticks:  {0}    ms:  {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----MUL CHECK-----");
            MyOutputListA = CalcStrings.Div(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");


            Console.WriteLine("-------DIV-------");
            sw = Stopwatch.StartNew();
            MyOutputList = CalcStrings.Div(Dig1, Dig2);
            sw.Stop();
            Console.WriteLine(string.Format("Ticks:  {0}    ms:  {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Console.WriteLine(MyOutputList[0]);
            Console.WriteLine(MyOutputList[1]);

            Console.WriteLine("-----DIV CHECK-----");
            MyOutputListM = CalcStrings.Mul(MyOutputList[0], Dig2);
            MyOutputListA = CalcStrings.Add(MyOutputListM[0], MyOutputList[1]);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");

            Console.WriteLine("-------POW-------");
            sw = Stopwatch.StartNew();
            MyOutputList = CalcStrings.Pow(Dig1, "10");
            sw.Stop();
            Console.WriteLine(string.Format("Ticks:  {0}    ms:  {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Console.WriteLine(MyOutputList[0]);
            Console.WriteLine(MyOutputList[1]);
        }

        


        

        

    }

    
    public static class CalcStringExt
    {
        public static string Left(this string @this, int count)
        {
            if (@this.Length <= count)
            {
                return @this;
            }
            else
            {
                return @this.Substring(0, count);
            }
        }
    }

    public static class CalcLists
    {

        public static List<long> Add(List<long> Dig1List, List<long> Dig2List, long MyDiv, int BiggerList)
        {

            if (Dig1List.Count == 1 && Dig1List[0] == 0)
            { return Dig2List; }
            if (Dig2List.Count == 1 && Dig2List[0] == 0)
            { return Dig1List; }


            //BiggerList: 0 - The same, 1-2 - 1 or 2, 3 - don't know
            List<long> Dig1ListTemp = new(Dig1List);
            List<long> Dig2ListTemp = new(Dig2List);
            List<long> MyOutput = new();
            long tempUp = 0;

            if (BiggerList == 3)//Change Lists make DigList1Temp bigger
            {
                if (CalcCompare.ListBigger(Dig1List, Dig2List) == 2)
                {
                    Dig1ListTemp = new(Dig2List);
                    Dig2ListTemp = new(Dig1List);
                }
            }

            if (BiggerList == 2)//Change Lists make DigList1Temp bigger
            {
                Dig1ListTemp = new(Dig2List);
                Dig2ListTemp = new(Dig1List);
            }

            int loops = Dig1ListTemp.Count - 1;
            for (int i = 0; i <= loops; i++) //adding Lists
            {
                long NextDig2Number = 0;
                if (i <= Dig2ListTemp.Count - 1)
                { NextDig2Number = Dig2ListTemp[i]; }

                long temp = Dig1ListTemp[i] + NextDig2Number + tempUp;
                tempUp = 0;
                if (temp >= MyDiv)
                {
                    tempUp = temp / MyDiv; //(temp - (temp % MyDiv)) / MyDiv;
                    temp %= MyDiv;
                    //temp -= (tempUp * MyDiv);
                    //Dig1ListTemp[i] = temp;
                    MyOutput.Add(temp);
                    if (i == loops)
                    {
                        //Dig1ListTemp.Add(tempUp);
                        MyOutput.Add(tempUp);
                        break; //????????
                    }
                    else
                    {
                        //Dig1ListTemp[i + 1] += tempUp;
                    }
                }
                else
                {
                    MyOutput.Add(temp);
                    //Dig1ListTemp[i] = temp;

                }
                //if (i >= LastDig2Number && tempUp == 0) //break if there is nothig to add
                //{
                //    break;
                //}
                //tempUp = 0;
            }

            //return Dig1ListTemp;
            return MyOutput;
        }

        public static List<long> Sub(List<long> Dig1List, List<long> Dig2List, long MyDiv, int BiggerList)
        {
            if (Dig1List.Count == 1 && Dig1List[0] == 0)
            { return Dig2List; }
            if (Dig2List.Count == 1 && Dig2List[0] == 0)
            { return Dig1List; }

            List<long> Dig1ListTemp = new(Dig1List);
            List<long> Dig2ListTemp = new(Dig2List);
            List<long> MyOutput = new();
            int debt = 0;

            if (BiggerList == 3)//Change Lists make DigList1Temp bigger
            {
                if (CalcCompare.ListBigger(Dig1ListTemp, Dig2ListTemp) == 2)
                {
                    Dig1ListTemp = new(Dig2List);
                    Dig2ListTemp = new(Dig1List);
                }
            }

            if (BiggerList == 2)//Change Lists make DigList1Temp bigger
            {
                Dig1ListTemp = new(Dig2List);
                Dig2ListTemp = new(Dig1List);
            }

            for (int i = 0; i < Dig1ListTemp.Count; i++) //Subtract arrays
            {
                long NextDig2Number = 0;
                if (i < Dig2ListTemp.Count)
                { NextDig2Number = Dig2ListTemp[i]; }

                long temp = (Dig1ListTemp[i] - NextDig2Number) - debt;
                debt = 0;

                if (temp < 0)
                {

                    debt = 1;
                    MyOutput.Add(temp + MyDiv);
                }
                else
                {
                    if (!(i == Dig1ListTemp.Count - 1 && temp == 0))
                    { MyOutput.Add(temp); }

                }
            }

            if (MyOutput.Count > 1)//cleanup zeros
            {
                while (MyOutput[^1] == 0)
                {
                    MyOutput.RemoveAt(MyOutput.Count - 1);
                    if (MyOutput.Count == 1)
                    { break; }
                }
            }
            if (MyOutput.Count <= 0)
            { MyOutput.Add(0); }

            return MyOutput;
        }

        public static List<long> Mul(List<long> Dig1List, List<long> Dig2List, long MyDiv, int BiggerList)
        {
            if (Dig1List.Count == 1 && Dig1List[0] == 1)
            { return Dig2List; }
            if (Dig2List.Count == 1 && Dig2List[0] == 1)
            { return Dig1List; }

            int MyPoss = 0;
            List<long> Dig1ListTemp = new(Dig1List);
            List<long> Dig2ListTemp = new(Dig2List);
            long tempUp = 0;

            if (BiggerList == 3)//Change Lists make DigList1Temp bigger
            {
                if (CalcCompare.ListBigger(Dig1ListTemp, Dig2ListTemp) == 2)
                {
                    Dig1ListTemp = new(Dig2List);
                    Dig2ListTemp = new(Dig1List);
                }
            }

            if (BiggerList == 2)//Change Lists make DigList1Temp bigger
            {
                Dig1ListTemp = new(Dig2List);
                Dig2ListTemp = new(Dig1List);
            }


            int Loop1Count = Dig1ListTemp.Count; //Dig1List.Count == Dig2List.Count
            int Loop2Count = Dig2ListTemp.Count;
            int ResultCount = (Dig1ListTemp.Count + Dig2ListTemp.Count) - 1;
            List<long> ResultList = new(new long[ResultCount]); // Product (ResultList) can be max Dig1List.Count + Dig2List.Count digit long or 1 less

            for (int i = 0; i < Loop2Count; i++)
            {

                for (int k = 0; k < Loop1Count; k++)
                {

                    long temp = (Dig2ListTemp[i] * Dig1ListTemp[k]) + ResultList[MyPoss];
                    tempUp = temp / MyDiv;
                    temp %= MyDiv;
                    ResultList[MyPoss] = temp;

                    if (tempUp > 0)
                    {
                        if (MyPoss + 1 == ResultCount)
                        { ResultList.Add(tempUp); }
                        else
                        { ResultList[MyPoss + 1] += tempUp; }
                    }

                    MyPoss += 1;
                }

                MyPoss = i + 1;
                tempUp = 0;
            }
            return ResultList;
        }

    }

    public class CalcStrings
    {
        public static List<string> Add(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new() { "", "", "" };

            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1Orig.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig[1..];
            }
            if (Dig2Orig.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig[1..];
            }

            if (Dig1 == "0")
            {
                MyOutput[0] = Dig2Orig;
                MyOutput[1] = Dig2;
                return MyOutput;
            }

            if (Dig2 == "0")
            {
                MyOutput[0] = Dig1Orig;
                MyOutput[1] = Dig1;
                return MyOutput;
            }

            if (Dig1.Length < 29 && Dig2.Length < 29)
            {
                decimal IntOutput = Convert.ToDecimal(Dig1Orig) + Convert.ToDecimal(Dig2Orig);
                MyOutput[0] = Convert.ToString(IntOutput);
                MyOutput[1] = Convert.ToString(Math.Abs(IntOutput));
                return MyOutput;
            }

            if (Dig1Sign == "-" && Dig2Sign == "-")
            { ResultSign = "-"; }

            var DigsLists = CalcConvert.StringsToLongLists(Dig1, Dig2, 18);

            List<long> Dig1List = DigsLists[0];
            List<long> Dig2List = DigsLists[1];

            long MyDiv = 1000000000000000000;

            int DigBiggerTemp = CalcCompare.ListBigger(Dig1List, Dig2List);

            if (Dig1Sign == "-" && Dig2Sign != "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");
                if (DigBiggerTemp == 1)
                { ResultSign = "-"; }
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            if (Dig1Sign != "-" && Dig2Sign == "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");
                if (DigBiggerTemp == 2)
                { ResultSign = "-"; }
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");

            if (MyOutput[1] == "")
            {
                MyOutput[1] = "0";
            }

            MyOutput[0] = ResultSign + MyOutput[1];

            return MyOutput;
        }

        public static List<string> Sub(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new() { "", "", "" };
            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig[1..];
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig[1..];
            }

            if (Dig1 == "0")
            {
                if (Dig2Sign == "-")
                {
                    MyOutput[0] = Dig2;
                    MyOutput[1] = Dig2;
                }
                else
                {
                    MyOutput[0] = "-" + Dig2;
                    MyOutput[1] = Dig2;
                }
                return MyOutput;
            }

            if (Dig2 == "0")
            {
                MyOutput[0] = Dig1Orig;
                MyOutput[1] = Dig1;
                return MyOutput;
            }

            if (Dig1.Length < 29 && Dig2.Length < 29)
            {
                decimal IntOutput = Convert.ToDecimal(Dig1Orig) - Convert.ToDecimal(Dig2Orig);
                MyOutput[0] = Convert.ToString(IntOutput);
                MyOutput[1] = Convert.ToString(Math.Abs(IntOutput));
                return MyOutput;
            }

            var DigsLists = CalcConvert.StringsToLongLists(Dig1, Dig2, 18);

            List<long> Dig1List = DigsLists[0];
            List<long> Dig2List = DigsLists[1];
            long MyDiv = 1000000000000000000;

            int DigBiggerTemp = CalcCompare.ListBigger(Dig1List, Dig2List);

            if (Dig1Sign == "-" && Dig2Sign != "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");
                ResultSign = "-";
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            if (Dig1Sign != "-" && Dig2Sign == "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");
                ResultSign = "";
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            if (Dig1Sign != "-" && Dig2Sign != "-")
            {
                if (DigBiggerTemp == 2)
                { ResultSign = "-"; }
            }

            if (Dig1Sign == "-" && Dig2Sign == "-")
            {
                if (DigBiggerTemp == 1)
                { ResultSign = "-"; }
            }

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv, DigBiggerTemp), "000000000000000000");

            if (MyOutput[1] == "")
            { MyOutput[1] = "0"; }

            MyOutput[0] = ResultSign + MyOutput[1];

            return MyOutput;
        }

        public static List<string> Mul(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new() { "", "", "" };
            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig[1..];
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig[1..];
            }

            if (Dig2 == "0" || Dig2 == "0")
            {
                MyOutput[0] = "0";
                MyOutput[1] = "0";
                return MyOutput;
            }

            if (Dig1.Length + Dig2.Length < 29)
            {
                decimal IntOutput = Convert.ToDecimal(Dig1Orig) * Convert.ToDecimal(Dig2Orig);
                MyOutput[0] = Convert.ToString(IntOutput);
                MyOutput[1] = Convert.ToString(Math.Abs(IntOutput));
                return MyOutput;
            }

            if (Dig1Sign != Dig2Sign)
            {
                ResultSign = "-";
            }

            var DigsLists = CalcConvert.StringsToLongLists(Dig1, Dig2, 9);

            List<long> Dig1List = DigsLists[0];
            List<long> Dig2List = DigsLists[1];

            long MyDiv = 1000000000;

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Mul(Dig1List, Dig2List, MyDiv, 3), "000000000");

            if (MyOutput[1] == "")
            {
                MyOutput[1] = "0";
            }

            MyOutput[0] = ResultSign + MyOutput[1];

            return MyOutput;
        }

        public static List<string> Pow(string Dig1Orig, string Dig2Orig)
        {
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;
            string Dig1Sign = "";
            string Dig2Sign = "";

            List<string> MyOutput = new() { Dig1, "", "" };
            List<long> MyPowerList = new() { 1 };
            List<long> MyPowerListAdd = new() { 1 };

            long MyDivM = 1000000000;
            long MyDivA = 1000000000000000000;

            if (Dig1Orig.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig[1..];
            }
            if (Dig2Orig.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig[1..];
            }

            //You can write condition to calculate power with proper result sign
            //or remember, if -Dig1Orig and Dig2Orig % 2 != 0 ResultSign = "-"

            if (Dig2 == "0")
            {
                MyOutput[0] = "1";
                return MyOutput;
            }

            if (Dig2 == "1")
            {
                MyOutput[0] = Dig1Orig;
                return MyOutput;
            }

            List<long> Dig1List = CalcConvert.StringToLongList(Dig1, 9)[0];
            List<long> Dig2List = CalcConvert.StringToLongList(Dig2, 18)[0];
            List<long> MyOutputList = Dig1List;

            while (CalcCompare.ListBigger(MyPowerList, Dig2List) == 2)
            {
                MyOutputList = CalcLists.Mul(MyOutputList, Dig1List, MyDivM, 1); //first pass list are the same, next MyOutputList is bigger, so 1 is good option
                MyPowerList = CalcLists.Add(MyPowerList, MyPowerListAdd, MyDivA, 1); //first pass list are the same, next MyOutputList is bigger, so 1 is good option
            }
            MyOutput[0] = CalcConvert.LongListToString(MyOutputList, "000000000");
            return MyOutput;
        }

        public static List<string> Div(string Dig1Orig, string Dig2Orig)
        {
            //most difficult part of job
            List<string> MyOutput = new() { "", "", "" };
            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;
            int Dig1Length = Dig1.Length;
            int Dig2Length = Dig2.Length;

            if (Dig1.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig[1..];
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig[1..];
            }

            if (Dig2 == "0")
            {
                MyOutput[0] = "ERROR Div by 0";
                MyOutput[1] = "ERROR Div by 0";
                return MyOutput;
            }

            if (Dig1Sign != Dig2Sign)
            {
                ResultSign = "-";
            }

            if (Dig1Length < Dig2Length)
            {
                MyOutput[0] = "0";
                MyOutput[1] = Dig2Sign + Dig1;
                return MyOutput;
            }

            if (Dig2 == "1")
            {
                MyOutput[0] = ResultSign + Dig1;
                MyOutput[1] = "0";
                return MyOutput;
            }

            if (Dig1 == "0")
            {
                MyOutput[0] = "0";
                MyOutput[1] = "0";
                return MyOutput;
            }

            if (Dig1Length < 29 && Dig2Length < 29)
            {
                decimal Dig1Dec = Convert.ToDecimal(Dig1Orig);
                decimal Dig2Dec = Convert.ToDecimal(Dig2Orig);
                decimal TempRestDec = Dig1Dec % Dig2Dec;
                //decimal DecModOutput = Convert.ToDecimal(Dig1Orig) % Convert.ToDecimal(Dig2Orig);
                decimal DecDecOutput = ((Dig1Dec - TempRestDec) / Dig2Dec);
                MyOutput[0] = Convert.ToString(DecDecOutput);
                MyOutput[1] = Convert.ToString(TempRestDec);
                return MyOutput;
            }

            string Dig1TempS = Dig1;
            string Dig2TempS = Dig2;

            var DigsList = CalcConvert.StringsToLongLists(Dig1TempS, Dig2TempS, 18);
            List<long> Dig1List = DigsList[0];
            List<long> Dig2OrigList = DigsList[1]; //It is needed for looping, must be still the same
            List<long> Dig2List = new(Dig2OrigList);

            int BiggerList = CalcCompare.ListBigger(Dig1List, Dig2OrigList);

            if (Dig1Length == Dig2Length)
            {

                if (BiggerList == 2)
                {
                    MyOutput[0] = "0";
                    MyOutput[1] = Dig1Sign + Dig1;
                    return MyOutput;
                }

                if (BiggerList == 0 && Dig1 != "0")
                {
                    MyOutput[0] = ResultSign + "1";
                    MyOutput[1] = "0";
                    return MyOutput;
                }

            }

            List<long> MultiplyList = new() { 0 };

            long MyDiv = 1000000000000000000;
            string Multiply = "";
            string TempMultiply = "";
            if (Dig1Length >= Dig2Length)
            {
                MyOutput[0] = Dig1;
                string FirstDigsDig1String = Dig1.Left(18);
                long First18DigDig1 = Convert.ToInt64(FirstDigsDig1String);
                long First18DigDig2 = Convert.ToInt64(Dig2.Left(18));
                long First17DigDig2 = Convert.ToInt64(Dig2.Left(17));

                while (BiggerList < 2)
                {
                    string TempMultiplierS = "";

                    if (MyOutput[0].Length < 29 && Dig2TempS.Length < 29)
                    {
                        decimal Dig1Dec = Convert.ToDecimal(MyOutput[0]);
                        decimal Dig2Dec = Convert.ToDecimal(Dig2);
                        decimal TempRestDec = Dig1Dec % Dig2Dec;
                        decimal TempMultiplierDec = (Dig1Dec - TempRestDec) / Dig2Dec;
                        Dig2TempS = Convert.ToString(Dig2Dec * TempMultiplierDec);
                        TempMultiply = Convert.ToString(TempMultiplierDec);

                    }
                    else
                    {

                        //solution "safe multiplier"
                        //Two important cases, 
                        //                     1 - first digits of MyOutput[0] are bigger or the same
                        //                     2 - first digits of MyOutput[0] are smaller then first digits of Dig1,

                        long FirstDigsDig1Long = Convert.ToInt64(FirstDigsDig1String.Left(18));


                        string AddZeros = "";
                        //Here we can't calculate Dig2TempS as list, because of MyDiv differences betwen Add and Mul
                        if (FirstDigsDig1Long >= First18DigDig2) //FirstDig1 >= FirstDig2
                        {
                            long TempMultiplier = (FirstDigsDig1Long / First18DigDig2);
                            if (TempMultiplier > 2) //change multiple subtraction to one multiplication
                            {
                                TempMultiplier -= 1; //for safe
                                TempMultiplierS = Convert.ToString(TempMultiplier);
                                Dig2TempS = CalcStrings.Mul(Dig2TempS, TempMultiplierS)[0];
                            }
                            else
                            {
                                TempMultiplier = 1;
                                TempMultiplierS = Convert.ToString(TempMultiplier);
                            }

                            //adding Zeros to make the same length, for safe one zero less
                            if (MyOutput[0].Length > Dig2TempS.Length)
                            {
                                AddZeros = new('0', (MyOutput[0].Length - Dig2TempS.Length) - 1);
                                Dig2TempS += AddZeros;
                            }

                            //and one more zero if we can
                            if (CalcCompare.StringBigger(MyOutput[0], Dig2TempS + "0") != 2)
                            {
                                Dig2TempS += "0";
                                TempMultiplierS += "0";
                            }

                            TempMultiply = TempMultiplierS + AddZeros; //and multiply TempMultiply by power of 10
                        }
                        else //FirstDig1 < FirstDig2
                        {
                            //we must take one digit less then First18DigMyOutput
                            long TempMultiplier = (FirstDigsDig1Long / First17DigDig2);
                            if (TempMultiplier > 2) //change multiple subtraction to one multiplication
                            {
                                TempMultiplier -= 1; //for safe
                                TempMultiplierS = Convert.ToString(TempMultiplier);
                                Dig2TempS = CalcStrings.Mul(Dig2TempS, TempMultiplierS)[0];
                            }
                            else
                            {
                                TempMultiplier = 1;
                                TempMultiplierS = Convert.ToString(TempMultiplier);
                            }

                            //adding Zeros to make the same length, for safe one zero less
                            if (MyOutput[0].Length > Dig2TempS.Length)
                            {
                                AddZeros = new('0', (MyOutput[0].Length - Dig2TempS.Length) - 1);
                                Dig2TempS += AddZeros;
                            }

                            //and one more zero if we can
                            if (CalcCompare.StringBigger(MyOutput[0], Dig2TempS + "0") != 2)
                            {
                                Dig2TempS += "0";
                                TempMultiplierS += "0";
                            }


                            TempMultiply = TempMultiplierS + AddZeros; //and multiply TempMultiply by power of 10
                        }

                    }

                    //now we can calculate on list
                    //make lists for subtraction. It's better to work on list, because sometimes "while" subtraction needs more than one loop
                    //this is how we can avoid string-list-string conversions in MikiSub and MikiAdd
                    DigsList = CalcConvert.StringsToLongLists(TempMultiply, Dig2TempS, 18);
                    Dig2List = DigsList[1];
                    List<long> TempMultiplyList = new(DigsList[0]);

                    while (CalcCompare.ListBigger(Dig1List, Dig2List) != 2) //subtracting lists and adding TempMultiply as list to TempMultiplyList
                    {
                        Dig1List = CalcLists.Sub(Dig1List, Dig2List, MyDiv, 1); //Here Dig1List can be bigger or equal Dig2List, so 1 is the good option
                        MultiplyList = new(CalcLists.Add(MultiplyList, TempMultiplyList, MyDiv, 3));//MultiplyList will be begger after first pass
                    }



                    //Now, for next loop, we need Dig1List as string
                    //MultiplyList can stay as list in this loop
                    //This way we can stay (partially) in lists calculation concept in this loop
                    MyOutput[0] = CalcConvert.LongListToString(Dig1List, "000000000000000000"); //create strings from list after repeated subtractions (Temporary rest)

                    //and the first 18 digits from Temporary rest are needed
                    FirstDigsDig1String = Convert.ToString(MyOutput[0].Left(18));
                    //restore original Dig2TempS
                    Dig2TempS = Dig2;

                    //And we must check if we can break calculation. Checking is "time waster", it's the best place for it
                    BiggerList = CalcCompare.ListBigger(Dig1List, Dig2OrigList);
                    if (BiggerList == 2) { break; }

                }

                //Now it' done, we can convert MultiplyList to (string)Multiply
                Multiply = CalcConvert.LongListToString(MultiplyList, "000000000000000000"); //Temporary Result(Quotient) //create strings from list after repeated subtractions

                //and change places of results
                MyOutput[1] = MyOutput[0]; //Rest
                MyOutput[0] = Multiply; //Quotient

                //last conditions to avoid empty lists
                if (MyOutput[0] == "") { MyOutput[0] = "0"; }
                if (MyOutput[1] == "") { MyOutput[1] = "0"; }

                //propably not nedded, but to avoid incomplete subtraction
                if (MyOutput[1] == Dig2)
                {
                    MyOutput = CalcStrings.Add(MyOutput[0], "1");
                    MyOutput[1] = "0";
                }
                //add proper sign for quotient and rest (REST NOT MODULO)
                MyOutput[0] = ResultSign + MyOutput[0];
                MyOutput[1] = Dig1Sign + MyOutput[1];

                return MyOutput;
            }

            return MyOutput;
        }

    }

    class CalcCompare
    {
        public static int StringBigger(string Dig1Orig, string Dig2Orig)
        {

            long DigsDiff = Dig1Orig.Length - Dig2Orig.Length;

            switch (DigsDiff)
            {

                case > 0:
                    return 1;
                case < 0:
                    return 2;
                case 0:
                    if (Dig1Orig == Dig2Orig)
                    { return 0; }

                    var ListTest = CalcConvert.StringsToLongLists(Dig1Orig, Dig2Orig, 18);

                    List<long> Dig1List = ListTest[0];
                    List<long> Dig2List = ListTest[1];

                    for (int i = Dig1List.Count - 1; i >= 0; i--)
                    {
                        if (Dig1List[i] > Dig2List[i])
                        {
                            return 1;
                        }
                        if (Dig1List[i] < Dig2List[i])
                        {
                            return 2;
                        }
                    }

                    return 0;
            }

        }

        public static int ListBigger(List<long> Dig1List, List<long> Dig2List)
        {
            long DigsDiff = Dig1List.Count - Dig2List.Count;

            switch (DigsDiff)
            {

                case > 0:
                    return 1;
                case < 0:
                    return 2;
                case 0:
                    if (Dig1List.SequenceEqual(Dig2List) == true)
                    { return 0; }

                    for (int i = Dig1List.Count - 1; i >= 0; i--)
                    {
                        if (Dig1List[i] > Dig2List[i])
                        {
                            return 1;
                        }
                        if (Dig1List[i] < Dig2List[i])
                        {
                            return 2;
                        }
                    }
                    return 0;
            }

        }
    }

    class CalcConvert
    {
        public static List<List<long>> StringsToLongLists(string Dig1Orig, string Dig2Orig, int FragLength)
        {
            List<List<long>> MyOutput = new();
            List<long> Dig1List = new();
            List<long> Dig2List = new();
            string Dig1;
            string Dig2;

            int DigLengthDiff = Dig1Orig.Length - Dig2Orig.Length;

            if (DigLengthDiff < 0)
            {
                Dig1 = Dig2Orig;
                Dig2 = Dig1Orig;
            }
            else
            {
                Dig1 = Dig1Orig;
                Dig2 = Dig2Orig;
            }

            int CommonLength = Dig1.Length;
            int Dig2Length = Dig2.Length;


            if (CommonLength > FragLength) //cut number string into n-digit int array (from right to left)
            {
                int ModLength1 = CommonLength % FragLength;
                int PartCount1 = CommonLength / FragLength;

                int ModLength2 = Dig2Length % FragLength;
                int PartCount2 = Dig2Length / FragLength;

                for (int i = PartCount1 - 1; i >= 0; i--)
                {
                    int start1 = ModLength1 + (i * FragLength);
                    int start2 = ModLength2 + (i * FragLength);
                    Dig1List.Add(Convert.ToInt64(Dig1.Substring(start1, FragLength))); //get n-digit string to list
                    if (PartCount2 - 1 >= i && PartCount2 > 0)
                    {
                        Dig2List.Add(Convert.ToInt64(Dig2.Substring(start2, FragLength)));
                    }
                }

                if (ModLength1 > 0)
                {
                    Dig1List.Add(Convert.ToInt64(Dig1.Substring(0, ModLength1))); //get n-digit string to list
                }
                if (ModLength2 > 0)
                {
                    Dig2List.Add(Convert.ToInt64(Dig2.Substring(0, ModLength2)));
                }
            }
            else
            {
                Dig1List.Add(Convert.ToInt64(Dig1)); //if number is smaller then n digit
                Dig2List.Add(Convert.ToInt64(Dig2));
            }

            if (DigLengthDiff < 0)
            {
                MyOutput.Add(Dig2List);
                MyOutput.Add(Dig1List);
            }
            else
            {
                MyOutput.Add(Dig1List);
                MyOutput.Add(Dig2List);
            }

            return MyOutput;
        }


        public static List<List<long>> StringToLongList(string Dig1Orig, int FragLength)
        {
            List<List<long>> MyOutput = new();
            List<long> Dig1List = new();
            List<long> LastDigNumber = new(new long[1]);

            int Dig1Length = Dig1Orig.Length;


            int CommonLength = Dig1Orig.Length;

            if (CommonLength > FragLength) //cut number string into n-digit int array (from right to left)
            {
                int ModLength = CommonLength % FragLength;
                int PartCount = CommonLength / FragLength;

                for (int i = PartCount - 1; i >= 0; i--)
                {
                    int start = ModLength + (i * FragLength);
                    Dig1List.Add(Convert.ToInt64(Dig1Orig.Substring(start, FragLength))); //get n-digit string to list
                    if (Dig1List[^1] > 0) { LastDigNumber[0] = (Dig1List.Count - 1); }
                }

                if (ModLength > 0)
                {
                    Dig1List.Add(Convert.ToInt64(Dig1Orig.Substring(0, ModLength))); //get n-digit string to list
                    if (Dig1List[^1] > 0) { LastDigNumber[0] = (Dig1List.Count - 1); }
                }
            }
            else
            {
                Dig1List.Add(Convert.ToInt64(Dig1Orig)); //if number is smaller then n digit
                LastDigNumber[0] = 0;
            }

            MyOutput.Add(Dig1List);
            MyOutput.Add(LastDigNumber);
            return MyOutput;
        }

        public static string LongListToString(List<long> DigList, string StringPlaces)
        {
            bool NextWithZero = false;
            string MyOutput = "";
            for (int i = DigList.Count - 1; i >= 0; i--)
            {

                if (NextWithZero == false)
                {
                    if (DigList[i] != 0)
                    {
                        MyOutput += Convert.ToString(DigList[i]);
                        NextWithZero = true;
                    }
                }
                else
                {
                    MyOutput += DigList[i].ToString(StringPlaces);
                }
            }
            return MyOutput;
        }

    }

}

