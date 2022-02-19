using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
using System.Diagnostics;

namespace Program
{
    class Program
    {



        static void Main()
        {
            int i = 0; //change to 0 if you want use calculator only, if you want to test calculator change to 1
            //look also at RunGenerator() and variable PassControl, program can check if password matches the number

            if (i == 0)
            { Miki.Miki.RunCalcTest(); }

            else
            {
                //string InputString = "00000000000000000000000000000000"; //seed as string
                string InputString = "1"; //seed as string //"2272657884496751345355241563627544170162852933518655225855"

                string WhatIs = "I"; //seed can be: P-password, N-Number, I-Password Number (1 -> Pass Pool)

                if (WhatIs == "I")
                {
                    if (InputString == "0")
                    { Console.WriteLine("Bad password number"); return; }

                    InputString = Miki.CalcStrings.Sub(InputString, "1")[0];
                }

                bool GetFirst = true; //if true, first seed (InputString) is returned
                int PassToGen = 0; //0 - No limit
                Passwords.Passwords.PrepareGenerator(InputString, WhatIs, GetFirst, PassToGen);
                Passwords.Passwords.RunGenerator(); //Good for long testing - Generate 32-char password from Int-string-number and Int-string-number from password
            }
        }
    }

}

namespace Passwords
{
    class Passwords
    {
        public static List<long> IncrementList; //static
        public static List<long> PoolValueList; //static
        public static List<long> DivList; //static - 62
        public static List<long> DigPassListCurr; //dynamic
        public static List<long> DigPassListNext; //dynamic
        public static int PassGenerated = 0;
        public static int PassToBeGen = 0;
        public static List<string> PassTextList; //dynamic

        public static void PrepareGenerator(string InputString, string WhatIs, bool GetFirst, int PassToGen)
        {
            //string Xseed = "0";
            string PassCharCount = "62";
            string PassCharPlaces = "32";
            string IncrementPower = "108"; //108
            string PassPool = Miki.CalcStrings.Pow(PassCharCount, PassCharPlaces)[0]; //include 000000000....00 - zzzzzzzzz....zz, eg. for 00-99 we have 100 possibilities (10^2), so the MaxValue is 99.
            string MaxValue = Miki.CalcStrings.Add(PassPool, "-1")[0]; //the MaxValue is 1 less

            string Increment = Miki.CalcStrings.Pow("3", IncrementPower)[0];


            string PassNumber = "";
            switch (WhatIs)
            {
                case "P":
                    PassNumber = LehmerGen.DigFromPass(InputString);
                    break;
                case "I":
                    PassNumber = Miki.CalcStrings.Div(Miki.CalcStrings.Mul(InputString, Increment)[0], PassPool)[1];
                    break;
                case "N":
                    PassNumber = InputString;
                    break;
            }

            string Xtemp = PassNumber;

            var XtempIncr = Miki.CalcConvert.StringsToLongLists(Xtemp, Increment, 18);
            var DivMaxValue = Miki.CalcConvert.StringsToLongLists(PassCharCount, PassPool, 18);

            Passwords.IncrementList = XtempIncr[1];
            Passwords.PoolValueList = DivMaxValue[1];
            Passwords.DivList = DivMaxValue[0];
            Passwords.PassToBeGen = PassToGen;

            Console.WriteLine("Pass Pool Count:   " + PassPool);
            Console.WriteLine("Max Value:         " + MaxValue);
            Console.WriteLine("Increment:         " + Increment);

            if (Miki.CalcStrings.Div(PassPool, Increment)[1] != "0") //Check if Increment value is good for LCG
            { Console.WriteLine("TRUE - Good Increment: PassPool % Increment != 0"); }
            else
            { Console.WriteLine("FALSE - Bad Increment: PassPool % Increment == 0"); return; }

            DigPassListCurr = XtempIncr[0];

            if (GetFirst == false)
            {
                _ = PassFromDig();
                DigPassListCurr = (DigPassListNext);
                Console.WriteLine("XXXX");
            }

        }

        public static void RunGenerator()
        {
            bool PassControl = false;

            long ElapsSecMin = 100000000000;
            long ElapsSecMax = 0;

            int PerCount = 10000;

            var sw = Stopwatch.StartNew();
            var swFull = Stopwatch.StartNew();
            List<string> PassTextListLocal = new();

            while (true)
            {
                string Pass = PassFromDig();

                PassTextListLocal.Add(Pass);

                PassControl = true;
                if (PassControl == true) //if true program calculate number from Pass and check if all calculation are good
                {
                    string OrygDig = Miki.CalcConvert.LongListToString(DigPassListCurr, "000000000000000000");
                    string FromPassDig = DigFromPass(Pass);
                    if (OrygDig != FromPassDig) //check if correct
                    {
                        Console.ReadKey();
                    }
                }

                DigPassListCurr = (DigPassListNext); //new

                PassGenerated += 1; //Global pass generated in this while

                if (PassGenerated % PerCount == 0)
                {
                    sw.Stop();
                    if (ElapsSecMin > sw.ElapsedMilliseconds)
                    { ElapsSecMin = sw.ElapsedMilliseconds; }
                    if (ElapsSecMax < sw.ElapsedMilliseconds)
                    { ElapsSecMax = sw.ElapsedMilliseconds; }
                    long FullSpeed = (swFull.ElapsedMilliseconds * PerCount / PassGenerated);
                    Console.WriteLine(string.Format("{0}     {1}     Time Min:  {2}      Time Max:  {3}    Time Mean:  {4}   /   {5}", Pass, PassGenerated, ElapsSecMin, ElapsSecMax, FullSpeed, PerCount));
                    sw = Stopwatch.StartNew();
                }

                if (PassGenerated == PassToBeGen)
                { break; }
            }

            PassTextList = PassTextListLocal;
            PassTextList.Clear();
        }


        public static string PassFromDig()
        {
            //generate long string-number and convert it to 32-char password
            //based on Lehmer algorithm
            //very good to test calculator
            //to avoid lots of conversion long lists <-> strings, all calculation are made on lists
            //becouse of calculation on lists, lists must be global
            List<long> Reminder;
            int k;

            long MyDiv = 1000000000000000000;

            List<char> MyCharList = new(new char[32]);
            Reminder = DigPassListCurr;

            k = 0;

            for (int h = 31; h >= 0; h--)
            {
                var templist = Miki.CalcLists.Div(Reminder, DivList, MyDiv, true);
                long TempInt = templist[1][0]; //reminder


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


                Reminder = templist[0]; //return with quotient

            }

            string Pass = new(MyCharList.ToArray());

            var PartDigPassListNext = Miki.CalcLists.Add(DigPassListCurr, IncrementList, MyDiv);
            DigPassListNext = Miki.CalcLists.Div(PartDigPassListNext, PoolValueList, MyDiv, true)[1];

            return Pass;
        }
        public static string DigFromPass(string MyPass)
        {
            //convert 32-char password to string-number

            List<char> CharList = new(MyPass);
            int k = 0;
            List<long> kList = new() { k };

            List<long> DigList = new() { 0 };

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

                List<long> CharIntList = new() { CharInt };

                var MyPower = Miki.CalcLists.Pow(DivList, kList);
                var TempDigList = Miki.CalcLists.Mul(CharIntList, MyPower, 1000000000);
                TempDigList = Miki.CalcConvert.ConvertLists(TempDigList, 1000000000, 1000000000000000000);
                DigList = Miki.CalcLists.Add(DigList, TempDigList, 1000000000000000000);
                k++;
                kList[0] = k;
            }

            string Dig = Miki.CalcConvert.LongListToString(DigList, "000000000000000000");
            return Dig;
        }
    }


    class LehmerGen
    {
        public static List<string> PassFromDig(string InputString, string WhatIs, bool CheckFirst)
        {

            //WhatIs: P - Pass, N - Number, I - Iteration
            List<string> Result = new();

            string PassCharCount = "62";
            string PassCharPlaces = "32";
            string IncrementPower = "108"; //108
            string MaxValue = Miki.CalcStrings.Add(Miki.CalcStrings.Pow(PassCharCount, PassCharPlaces)[0], "1")[0];
            string Increment = Miki.CalcStrings.Pow("3", IncrementPower)[0];


            string PassNumber = "";
            switch (WhatIs)
            {
                case "P":
                    PassNumber = LehmerGen.DigFromPass(InputString);
                    break;
                case "I":
                    PassNumber = Miki.CalcStrings.Div(Miki.CalcStrings.Mul(InputString, Increment)[0], MaxValue)[1];
                    break;
                case "N":
                    PassNumber = InputString;
                    break;
            }

            string Xseed = PassNumber;


            int k = 0;


            string Xnew = Xseed;


            if (CheckFirst == false)
            {
                Xnew = Miki.CalcStrings.Div(Miki.CalcStrings.Add(Xseed, Increment)[0], MaxValue)[1];
            }

            string Xtemp = Xnew;

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

            Result.Add(Pass);
            Result.Add(Xnew);

            return Result;

        }
        public static string DigFromPass(string MyPass)
        {
            //convert 32-char password to string-number

            List<long> TempDigList = new() { 0 };
            List<long> MyPower = new() { 0 };

            List<char> CharList = new(MyPass);
            int k = 0;
            List<long> kList = new() { k };

            List<long> DigList = new() { 0 };
            List<long> BasePower = new() { 62 };

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

                List<long> CharIntList = new() { CharInt };

                MyPower = Miki.CalcLists.Pow(BasePower, kList);
                TempDigList = Miki.CalcLists.Mul(CharIntList, MyPower, 1000000000);
                TempDigList = Miki.CalcConvert.ConvertLists(TempDigList, 1000000000, 1000000000000000000);
                DigList = Miki.CalcLists.Add(DigList, TempDigList, 1000000000000000000);
                k++;
                kList[0] = k;
            }
            string Dig = Miki.CalcConvert.LongListToString(DigList, "000000000000000000");
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

            List<string> MyOutputList = new List<string>();
            string[] TestCalc = new string[2];
            var sw = Stopwatch.StartNew();

            string Dig1 = "102543890397977681684285524423227768626815861760032026538";
            //string Dig1 = "99999999999999999999999999999999999999999999999999999999";
            //string Dig2 = "101111111397977681684285524423227768626815861760032026538";
            string Dig2 = "815861760032026538";

            Console.WriteLine(Dig1 + "  Length: " + Dig1.Length);
            Console.WriteLine(Dig2 + "  Length: " + Dig2.Length);
            List<string> MyOutputListM = new List<string>();
            List<string> MyOutputListA = new List<string>();

            int RepCount = 100;

            long swMin = 1000000000000;
            long swMax = 0;
            long swMsMin = 1000000000;
            Console.WriteLine("++++++++++++++++++++++++++++++");
            Console.WriteLine("-------ADD-------");
            for (int i = 0; i <= RepCount; i++)
            {
                sw = Stopwatch.StartNew();
                MyOutputList = CalcStrings.Add(Dig1, Dig2);
                sw.Stop();
                if (sw.ElapsedTicks < swMin) { swMin = sw.ElapsedTicks; swMsMin = sw.ElapsedMilliseconds; }
                if (sw.ElapsedTicks > swMax) { swMax = sw.ElapsedTicks; }
            }
            Console.WriteLine(string.Format("Ticks Min:  {0}    Ticks Max:  {1}    ms Min:  {2}", swMin, swMax, swMsMin));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----ADD CHECK-----");
            MyOutputListA = CalcStrings.Sub(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");

            swMin = 1000000000000;
            swMax = 0;
            swMsMin = 1000000000;
            Console.WriteLine("-------SUB-------");
            for (int i = 0; i <= RepCount; i++)
            {
                sw = Stopwatch.StartNew();
                MyOutputList = CalcStrings.Sub(Dig1, Dig2);
                sw.Stop();
                if (sw.ElapsedTicks < swMin) { swMin = sw.ElapsedTicks; swMsMin = sw.ElapsedMilliseconds; }
                if (sw.ElapsedTicks > swMax) { swMax = sw.ElapsedTicks; }
            }
            Console.WriteLine(string.Format("Ticks Min:  {0}    Ticks Max:  {1}    ms Min:  {2}", swMin, swMax, swMsMin));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----SUB CHECK-----");
            MyOutputListA = CalcStrings.Add(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");

            swMin = 1000000000000;
            swMax = 0;
            swMsMin = 1000000000;
            Console.WriteLine("-------MUL-------");
            for (int i = 0; i <= RepCount; i++)
            {
                sw = Stopwatch.StartNew();
                MyOutputList = CalcStrings.Mul(Dig1, Dig2);
                sw.Stop();
                if (sw.ElapsedTicks < swMin) { swMin = sw.ElapsedTicks; swMsMin = sw.ElapsedMilliseconds; }
                if (sw.ElapsedTicks > swMax) { swMax = sw.ElapsedTicks; }
            }
            Console.WriteLine(string.Format("Ticks Min:  {0}    Ticks Max:  {1}    ms Min:  {2}", swMin, swMax, swMsMin));
            Console.WriteLine(MyOutputList[0]);

            Console.WriteLine("-----MUL CHECK-----");
            MyOutputListA = CalcStrings.Div(MyOutputList[0], Dig2);
            Console.WriteLine(MyOutputListA[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MyOutputListA[0] == Dig1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("++++++++++++++++++++++++++++++");

            swMin = 1000000000000;
            swMax = 0;
            swMsMin = 1000000000;
            Console.WriteLine("-------DIV-------");
            for (int i = 0; i <= RepCount; i++)
            {
                sw = Stopwatch.StartNew();
                MyOutputList = CalcStrings.Div(Dig1, Dig2);
                sw.Stop();
                if (sw.ElapsedTicks < swMin) { swMin = sw.ElapsedTicks; swMsMin = sw.ElapsedMilliseconds; }
                if (sw.ElapsedTicks > swMax) { swMax = sw.ElapsedTicks; }
            }
            Console.WriteLine(string.Format("Ticks Min:  {0}    Ticks Max:  {1}    ms Min:  {2}", swMin, swMax, swMsMin));
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

            swMin = 1000000000000;
            swMax = 0;
            swMsMin = 1000000000;
            Console.WriteLine("-------POW-------");
            for (int i = 0; i <= RepCount; i++)
            {
                sw = Stopwatch.StartNew();
                MyOutputList = CalcStrings.Pow(Dig1, "10");
                sw.Stop();
                if (sw.ElapsedTicks < swMin) { swMin = sw.ElapsedTicks; swMsMin = sw.ElapsedMilliseconds; }
                if (sw.ElapsedTicks > swMax) { swMax = sw.ElapsedTicks; }
            }
            Console.WriteLine(string.Format("Ticks Min:  {0}    Ticks Max:  {1}    ms Min:  {2}", swMin, swMax, swMsMin));
            Console.WriteLine(MyOutputList[0]);
            Console.WriteLine(MyOutputList[1]);
        }


    }

    public static class CalcIntExt
    {
        public static int IntLength(long i)
        {
            //if (i < 0)
            //throw new ArgumentOutOfRangeException();

            if (i == 0)
                return 1;

            return (int)Math.Floor(Math.Log10(i)) + 1;
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

        public static string Right(this string input, int count)
        {
            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }

        public static string Mid(this string input, int start, int count)
        {
            return input.Substring(Math.Min(start, input.Length), Math.Min(count, Math.Max(input.Length - start, 0)));
        }

    }

    public static class CalcLists
    {

        public static List<long> Add(List<long> Dig1List, List<long> Dig2List, long MyDiv)
        {
            //Old, very good

            if (Dig1List.Count == 1 && Dig1List[0] == 0)
            { return Dig2List; }
            if (Dig2List.Count == 1 && Dig2List[0] == 0)
            { return Dig1List; }
            List<long> MyOutput = new List<long>();

            List<long> Dig1ListTemp;
            List<long> Dig2ListTemp;

            long tempUp = 0;
            long temp;

            if (Dig1List.Count < Dig2List.Count)
            {
                Dig1ListTemp = new List<long>(Dig2List);
                Dig2ListTemp = new List<long>(Dig1List);
            }
            else
            {
                Dig1ListTemp = new List<long>(Dig1List);
                Dig2ListTemp = new List<long>(Dig2List);
            }

            int loops = Dig1ListTemp.Count - 1;
            for (int i = 0; i <= loops; i++) //adding Lists
            {
                long NextDig2Number = 0;
                if (i < Dig2ListTemp.Count)
                { NextDig2Number = Dig2ListTemp[i]; }


                temp = Dig1ListTemp[i] + NextDig2Number + tempUp;

                if (temp >= MyDiv)
                {
                    tempUp = 1;
                    temp -= MyDiv;
                    MyOutput.Add(temp);
                    if (i == loops)
                    {
                        MyOutput.Add(tempUp);
                        break; //????????
                    }

                }
                else
                {
                    tempUp = 0;
                    MyOutput.Add(temp);
                }

            }

            return MyOutput;
        }

        public static List<long> Sub(List<long> Dig1List, List<long> Dig2List, long MyDiv)
        {
            if (Dig1List.Count == 1 && Dig1List[0] == 0)
            { return Dig2List; }
            if (Dig2List.Count == 1 && Dig2List[0] == 0)
            { return Dig1List; }

            List<long> Dig1ListTemp;
            List<long> Dig2ListTemp;
            List<long> MyOutput = new List<long>();
            int debt = 0;
            long temp;


            if (CalcCompare.ListBigger(Dig1List, Dig2List) == 2)
            {
                Dig1ListTemp = new List<long>(Dig2List);
                Dig2ListTemp = new List<long>(Dig1List);
            }
            else
            {
                Dig1ListTemp = new List<long>(Dig1List);
                Dig2ListTemp = new List<long>(Dig2List);
            }

            int ListDiff = Dig1ListTemp.Count - Dig2ListTemp.Count;

            for (int i = 0; i < Dig1ListTemp.Count; i++) //Subtract arrays
            {
                long NextDig2Number = 0;
                if (i < Dig2ListTemp.Count)
                { NextDig2Number = Dig2ListTemp[i]; }


                temp = (Dig1ListTemp[i] - NextDig2Number) - debt;


                if (temp < 0)
                {

                    debt = 1;
                    MyOutput.Add(temp + MyDiv);
                }
                else
                {
                    debt = 0;
                    MyOutput.Add(temp);
                }

            }

            for (int i = MyOutput.Count - 1; i >= 1; i--)
            {
                if (MyOutput[i] == 0)
                { MyOutput.RemoveAt(i); }
                if (MyOutput[MyOutput.Count - 1] != 0) //^
                { break; }
            }

            return MyOutput;
        }

        public static List<long> Mul(List<long> Dig1List, List<long> Dig2List, long MyDiv)
        {

            if (Dig1List.Count == 1)
            {
                if (Dig1List[0] == 1)
                { return Dig2List; }
                if (Dig1List[0] == 0)
                { return Dig1List; }

            }

            if (Dig2List.Count == 1)
            {
                if (Dig2List[0] == 1)
                { return Dig1List; }
                if (Dig2List[0] == 0)
                { return Dig2List; }
            }


            int MyPoss = 0;
            List<long> Dig1ListTemp;
            List<long> Dig2ListTemp;
            long tempUp; ;

            if (Dig1List.Count < Dig2List.Count)
            {
                Dig1ListTemp = new List<long>(Dig2List);
                Dig2ListTemp = new List<long>(Dig1List);
            }
            else
            {
                Dig1ListTemp = new List<long>(Dig1List);
                Dig2ListTemp = new List<long>(Dig2List);
            }


            int Loop1Count = Dig1ListTemp.Count;
            int Loop2Count = Dig2ListTemp.Count;
            int ResultCount = (Dig1ListTemp.Count + Dig2ListTemp.Count) - 1;
            List<long> ResultList = new List<long>(new long[ResultCount]); // Product (ResultList) can be max Dig1List.Count + Dig2List.Count digit long or 1 less

            for (int i = 0; i < Loop2Count; i++)
            {

                for (int k = 0; k < Loop1Count; k++)
                {

                    long temp = (Dig2ListTemp[i] * Dig1ListTemp[k]) + ResultList[MyPoss];
                    if (temp >= MyDiv)
                    {
                        tempUp = temp / MyDiv;
                        temp %= MyDiv;
                        if (MyPoss + 1 == ResultCount)
                        { ResultList.Add(tempUp); }
                        else
                        { ResultList[MyPoss + 1] += tempUp; }
                    }

                    ResultList[MyPoss] = temp;

                    MyPoss += 1;
                }

                MyPoss = i + 1;
            }


            return ResultList;
        }

        public static List<List<long>> Div(List<long> Dig1List, List<long> Dig2List, long MyDiv18, bool CheckDecimal)
        {

            List<List<long>> MyOutput = new List<List<long>>();

            if (Dig1List.Count < Dig2List.Count) // fast pass
            {
                MyOutput.Add(new List<long>() { 0 });
                MyOutput.Add(Dig1List);
                return MyOutput;
            }

            if (Dig1List.Count == 1 && Dig2List.Count == 1) // sometimes we can simply divide two longs - fast pass
            {
                if (Dig2List[0] != 0)
                {
                    MyOutput.Add(new List<long>() { Dig1List[0] / Dig2List[0] });
                    MyOutput.Add(new List<long>() { Dig1List[0] % Dig2List[0] });
                }
                else
                {
                    MyOutput.Add(new List<long>() { 0 }); //error Div by 0, but I don't care
                    MyOutput.Add(new List<long>() { 0 });
                    Console.WriteLine("ERROR - Div by 0 - Return: 0");
                }
                return MyOutput;
            }


            int Dig2ListFirstLength;
            int Dig2ListLength;

            int Dig1ListFirstLength;
            int Dig1ListLength;

            int BiggerList = 1;

            Dig2ListFirstLength = CalcIntExt.IntLength(Dig2List[Dig2List.Count - 1]); //static
            Dig2ListLength = (Dig2List.Count - 1) * 18 + Dig2ListFirstLength; //static

            Dig1ListFirstLength = CalcIntExt.IntLength(Dig1List[Dig1List.Count - 1]); //must be counted in each loop and it is necessary here
            Dig1ListLength = (Dig1List.Count - 1) * 18 + Dig1ListFirstLength; //must be counted in each loop and it is necessary here

            if (Dig1ListLength < Dig2ListLength)
            {
                MyOutput.Add(new List<long>() { 0 });
                MyOutput.Add(Dig1List);
                return MyOutput;
            }

            if (Dig1ListLength == Dig2ListLength)
            {
                BiggerList = CalcCompare.ListBigger(Dig1List, Dig2List);

                switch (BiggerList)
                {
                    case 0:
                        MyOutput.Add(new List<long>() { 1 });
                        MyOutput.Add(new List<long>() { 0 });
                        return MyOutput;
                    case 2:
                        MyOutput.Add(new List<long>() { 0 });
                        MyOutput.Add(Dig1List);
                        return MyOutput;
                }

            }

            if (CheckDecimal == true) //sometimes we can simply calculate decimals
            {

                if (Dig1ListLength < 29 && Dig2ListLength < 29)
                {
                    decimal DecimalDig1 = CalcCompare.GetDecimalFromList(Dig1List);
                    decimal DecimalDig2 = CalcCompare.GetDecimalFromList(Dig2List);

                    decimal TempRestDec = DecimalDig1 % DecimalDig2;
                    decimal DecDecOutput = ((DecimalDig1 - TempRestDec) / DecimalDig2);

                    List<long> LongResult = new List<long>();
                    if (DecDecOutput >= MyDiv18)
                    { LongResult = new List<long>() { (long)(DecDecOutput % MyDiv18), (long)(DecDecOutput / MyDiv18) }; }
                    else
                    { LongResult = new List<long>() { (long)(DecDecOutput) }; }

                    List<long> LongRest = new List<long>();
                    if (TempRestDec >= MyDiv18)
                    { LongRest = new List<long>() { (long)(TempRestDec % MyDiv18), (long)(TempRestDec / MyDiv18) }; }
                    else
                    { LongRest = new List<long>() { (long)(TempRestDec) }; }

                    MyOutput.Add(LongResult);
                    MyOutput.Add(LongRest);
                    return MyOutput;
                }

            }


            List<long> TempDig2List18 = new List<long>();
            List<long> TempDig2List9 = new List<long>();
            List<long> TempMultiplyList = new List<long>();
            long First18DigDig1;
            long First17DigDig2;
            long MyDiv9 = 1000000000;

            List<long> Dig2OrigList9 = CalcConvert.ConvertLists(Dig2List, MyDiv18, MyDiv9); //static, necessary for multiplication

            //prepare number from Dig2 to estimate multiplier, take 17 digits
            if (Dig2ListLength > 17)
            {
                First17DigDig2 = (CalcCompare.GetLongFromList(Dig2List, 16, 18) * 10) + 9; //last digit is uknown, so it can be 9
            }
            else
            {
                First17DigDig2 = CalcCompare.GetLongFromList(Dig2List, 17, 18); //static, necessary to find "safe multiplier"
            }


            List<long> MultiplyList = new List<long>() { 0 };

            while (BiggerList < 2) //division by repeated subtraction
            {

                List<long> SafeMultiplierList18 = new List<long>();


                if (Dig1ListLength < 29 && Dig2ListLength < 29) //sometimes we can simply calculate on decimals at the end
                {

                    if (Dig1ListLength < 19 && Dig2ListLength < 19) // or on longs
                    {
                        long LongDig1 = Dig1List[0];
                        long LongDig2 = Dig2List[0];

                        long TempRestLong = LongDig1 % LongDig2;
                        long LongOutput = ((LongDig1 - TempRestLong) / LongDig2); //SafeMultiplierList18

                        long LongTempDig2 = LongOutput * LongDig2; // TempDig2List18

                        SafeMultiplierList18 = new List<long>() { LongOutput };
                        TempDig2List18 = new List<long>() { LongTempDig2 };
                    }
                    else
                    {
                        decimal DecimalDig1 = CalcCompare.GetDecimalFromList(Dig1List);
                        decimal DecimalDig2 = CalcCompare.GetDecimalFromList(Dig2List);

                        decimal TempRestDec = DecimalDig1 % DecimalDig2;
                        decimal DecDecOutput = ((DecimalDig1 - TempRestDec) / DecimalDig2); //SafeMultiplierList18

                        if (DecDecOutput >= MyDiv18) //SafeMultiplierList18
                        { SafeMultiplierList18 = new List<long>() { (long)(DecDecOutput % MyDiv18), (long)(DecDecOutput / MyDiv18) }; }
                        else
                        { SafeMultiplierList18 = new List<long>() { (long)(DecDecOutput) }; }

                        decimal DecTempDig2 = DecDecOutput * DecimalDig2; // TempDig2List18
                        if (DecTempDig2 >= MyDiv18)
                        { TempDig2List18 = new List<long>() { (long)(DecTempDig2 % MyDiv18), (long)(DecTempDig2 / MyDiv18) }; }
                        else
                        { TempDig2List18 = new List<long>() { (long)(DecTempDig2) }; }

                    }
                }
                else //but if numbers are bigger then decimal
                {
                    //here will be numbers longer then 18 digits, only
                    First18DigDig1 = CalcCompare.GetLongFromList(Dig1List, 17, 18); //so we take 17 digits... must be counted in each loop

                    { First18DigDig1 *= 10; } //and multiply by 10 because next digit is uknown, so it can be 0

                    long SafeMultiplier = First18DigDig1 / First17DigDig2; //it is max 18 digits

                    if (Dig2ListLength == Dig1ListLength) // This is necessary.
                    {
                        SafeMultiplier /= 10;
                        if (SafeMultiplier == 0) { SafeMultiplier = 1; }
                    }

                    SafeMultiplierList18 = new List<long>() { SafeMultiplier };

                    List<long> SafeMultiplierList9 = new List<long>();
                    if (SafeMultiplier >= MyDiv9) //faster
                    { SafeMultiplierList9 = new List<long>() { SafeMultiplier % MyDiv9, SafeMultiplier / MyDiv9 }; }
                    else
                    { SafeMultiplierList9 = new List<long>() { SafeMultiplier }; }

                    TempDig2List9 = (CalcLists.Mul(Dig2OrigList9, SafeMultiplierList9, MyDiv9));

                    TempDig2List18 = (CalcConvert.ConvertLists(TempDig2List9, MyDiv9, MyDiv18));

                    int TempDig2ListLength = (TempDig2List9.Count - 1) * 9 + CalcIntExt.IntLength(TempDig2List9[TempDig2List9.Count - 1]); //must be counted in each loop

                    int ZerosToAdd = Dig1ListLength - TempDig2ListLength;

                    if (ZerosToAdd > 0) //if 0 then we multiply by 1
                    {
                        long FirstTempDig2List = CalcCompare.GetLongFromList(TempDig2List18, 1, 18);
                        long FirstDig1List = CalcCompare.GetLongFromList(Dig1List, 1, 18);

                        if (FirstDig1List - FirstTempDig2List < 0)
                        { ZerosToAdd -= 1; }

                        int ZerosD = (ZerosToAdd) / 9;
                        int ZerosU = (ZerosToAdd) % 9;

                        List<long> MultiplyBy10List = new List<long>();

                        for (int t = 0; t < ZerosD; t++)
                        { MultiplyBy10List.Add(0); }
                        MultiplyBy10List.Add((long)Math.Pow(10, ZerosU));

                        TempDig2List9 = (CalcLists.Mul(TempDig2List9, MultiplyBy10List, MyDiv9));
                        SafeMultiplierList9 = (CalcLists.Mul(SafeMultiplierList9, MultiplyBy10List, MyDiv9));
                    }

                    TempDig2List18 = (CalcConvert.ConvertLists(TempDig2List9, MyDiv9, MyDiv18));
                    SafeMultiplierList18 = (CalcConvert.ConvertLists(SafeMultiplierList9, MyDiv9, MyDiv18));

                }



                Dig1List = CalcLists.Sub(Dig1List, TempDig2List18, MyDiv18);
                MultiplyList = (CalcLists.Add(MultiplyList, SafeMultiplierList18, MyDiv18));

                //And we must check if we can break calculation. Checking is "time waster", it's the best place for it
                BiggerList = CalcCompare.ListBigger(Dig1List, Dig2List);

                if (BiggerList == 2)
                { break; }

                Dig1ListFirstLength = CalcIntExt.IntLength(Dig1List[Dig1List.Count - 1]); //must be counted in each loop
                Dig1ListLength = (Dig1List.Count - 1) * 18 + Dig1ListFirstLength; //must be counted in each loop

            }


            MyOutput.Add(MultiplyList);
            MyOutput.Add(Dig1List);

            return MyOutput;
        }



        public static List<long> Pow(List<long> Dig1List, List<long> Dig2List)
        {

            List<long> MyPowerList = new List<long>() { 1 };
            List<long> MyPowerListAdd = new List<long>() { 1 };

            long MyDivM = 1000000000;
            long MyDivA = 1000000000000000000;

            if (Dig2List.Count == 1 && Dig2List[0] == 0)
            {
                List<long> MyOutput1 = new List<long>() { 1 };
                return MyOutput1;
            }

            if (Dig2List.Count == 1 && Dig2List[0] == 1)
            {
                return Dig1List;
            }


            List<long> MyOutputList = Dig1List;

            while (CalcCompare.ListBigger(MyPowerList, Dig2List) == 2)
            {
                MyOutputList = CalcLists.Mul(MyOutputList, Dig1List, MyDivM);
                MyPowerList = CalcLists.Add(MyPowerList, MyPowerListAdd, MyDivA);
            }

            return MyOutputList;

        }

    }
    public class CalcStrings
    {
        public static List<string> Add(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new List<string>() { "", "" };

            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1Orig.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig.Substring(1); //[1..]
            }
            if (Dig2Orig.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig.Substring(1);
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
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv), "000000000000000000");
                if (DigBiggerTemp == 1)
                { ResultSign = "-"; }
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            if (Dig1Sign != "-" && Dig2Sign == "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv), "000000000000000000");
                if (DigBiggerTemp == 2)
                { ResultSign = "-"; }
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv), "000000000000000000");

            if (MyOutput[1] == "")
            {
                MyOutput[1] = "0";
            }

            MyOutput[0] = ResultSign + MyOutput[1];

            return MyOutput;
        }

        public static List<string> Sub(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new List<string>() { "", "", "" };
            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig.Substring(1);
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig.Substring(1);
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
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv), "000000000000000000");
                ResultSign = "-";
                MyOutput[0] = ResultSign + MyOutput[1];
                return MyOutput;
            }

            if (Dig1Sign != "-" && Dig2Sign == "-")
            {
                MyOutput[1] = CalcConvert.LongListToString(CalcLists.Add(Dig1List, Dig2List, MyDiv), "000000000000000000");
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

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Sub(Dig1List, Dig2List, MyDiv), "000000000000000000");

            if (MyOutput[1] == "")
            { MyOutput[1] = "0"; }

            MyOutput[0] = ResultSign + MyOutput[1];

            return MyOutput;
        }

        public static List<string> Mul(string Dig1Orig, string Dig2Orig)
        {

            List<string> MyOutput = new List<string>() { "", "", "" };
            string ResultSign = "";
            string Dig1Sign = "";
            string Dig2Sign = "";
            string Dig1 = Dig1Orig;
            string Dig2 = Dig2Orig;

            if (Dig1.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig.Substring(1);
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig.Substring(1);
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

            MyOutput[1] = CalcConvert.LongListToString(CalcLists.Mul(Dig1List, Dig2List, MyDiv), "000000000");

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

            List<string> MyOutput = new List<string>() { Dig1, "", "" };
            List<long> MyPowerList = new List<long>() { 1 };
            List<long> MyPowerListAdd = new List<long>() { 1 };

            long MyDivM = 1000000000;
            long MyDivA = 1000000000000000000;

            if (Dig1Orig.Substring(0, 1) == "-")
            {
                Dig1Sign = "-";
                Dig1 = Dig1Orig.Substring(1);
            }
            if (Dig2Orig.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig.Substring(1);
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

            List<long> Dig1List = CalcConvert.StringToLongList(Dig1, 9);
            List<long> Dig2List = CalcConvert.StringToLongList(Dig2, 18);
            List<long> MyOutputList = CalcLists.Pow(Dig1List, Dig2List); // Dig1List;

            MyOutput[0] = CalcConvert.LongListToString(MyOutputList, "000000000");
            return MyOutput;
        }

        public static List<string> Div(string Dig1Orig, string Dig2Orig)
        {
            //most difficult part of job
            List<string> MyOutput = new List<string>() { "", "", "" };
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
                Dig1 = Dig1Orig.Substring(1);
            }
            if (Dig2.Substring(0, 1) == "-")
            {
                Dig2Sign = "-";
                Dig2 = Dig2Orig.Substring(1);
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
                decimal DecDecOutput = ((Dig1Dec - TempRestDec) / Dig2Dec);
                MyOutput[0] = Convert.ToString(DecDecOutput);
                MyOutput[1] = Convert.ToString(TempRestDec);
                return MyOutput;
            }


            var DigsList = CalcConvert.StringsToLongLists(Dig1, Dig2, 18);

            List<long> Dig1List = DigsList[0];
            List<long> Dig2List = DigsList[1];

            int BiggerList = CalcCompare.ListBigger(Dig1List, Dig2List);

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


            if (Dig1Length >= Dig2Length)
            {
                long MyDiv = 1000000000000000000;

                var MyResult = CalcLists.Div(Dig1List, Dig2List, MyDiv, false);

                string Multiply = CalcConvert.LongListToString(MyResult[0], "000000000000000000"); //Temporary Result(Quotient) //create strings from list after repeated subtractions
                MyOutput[0] = CalcConvert.LongListToString(MyResult[1], "000000000000000000"); //create strings from list after repeated subtractions (Temporary rest)

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

            if (DigsDiff > 0)
            { return 1; }
            if (DigsDiff < 0)
            { return 2; }
            if (DigsDiff == 0)
            {
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


            return 0;
        }

        public static int ListBigger(List<long> Dig1List, List<long> Dig2List)
        {
            long DigsDiff = Dig1List.Count - Dig2List.Count;

            if (DigsDiff > 0)
            { return 1; }
            if (DigsDiff < 0)
            { return 2; }
            if (DigsDiff == 0)
            {
                for (int i = Dig1List.Count - 1; i >= 0; i--)
                {
                    long Diff = Dig1List[i] - Dig2List[i];
                    if (Diff > 0)
                    {
                        return 1;
                    }
                    if (Diff < 0)
                    {
                        return 2;
                    }
                }
                return 0;
            }

            return 0;

        }

        public static long GetLongFromList(List<long> DigList, int DigCount, int MyDiv)
        {

            long FullLong = 0;
            long AddLong = 0;
            int DigListFirstLength = CalcIntExt.IntLength(DigList[DigList.Count - 1]); //obliczane w każdej pętli //^


            if (DigListFirstLength == DigCount)
            { return DigList[DigList.Count - 1]; } //^


            if (DigListFirstLength > DigCount)
            { return (DigList[DigList.Count - 1] / (long)Math.Pow(10, DigListFirstLength - DigCount)); } //^
            else
            {

                if (DigList.Count > 1)
                {

                    FullLong = (DigList[DigList.Count - 1] * (long)Math.Pow(10, (MyDiv - DigListFirstLength))); //^
                    AddLong = (DigList[DigList.Count - 2] / (long)Math.Pow(10, (DigListFirstLength))); //^2
                    FullLong += AddLong;
                    return (FullLong / (long)Math.Pow(10, MyDiv - DigCount));
                }
                else
                {
                    return DigList[DigList.Count - 1]; //^
                }

            }


        }

        public static decimal GetDecimalFromList(List<long> DigList)
        {
            decimal FulDec = 0;

            decimal MyDiv = 1000000000000000000;
            if (DigList.Count > 1)
            {

                FulDec = (DigList[DigList.Count - 1] * MyDiv); //^1
                FulDec += DigList[DigList.Count - 2]; //^2
                return FulDec;
            }
            else
            {
                return DigList[DigList.Count - 1];
            }

        }
    }

    public class CalcConvert
    {
        public static List<List<long>> StringsToLongLists(string Dig1Orig, string Dig2Orig, int FragLength)
        {
            List<List<long>> MyOutput = new List<List<long>>();
            List<long> Dig1List = new List<long>();
            List<long> Dig2List = new List<long>();
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




        public static List<long> StringToLongList(string Dig1Orig, int FragLength)
        {
            List<long> Dig1List = new List<long>();

            int CommonLength = Dig1Orig.Length;

            if (CommonLength > FragLength) //cut number string into n-digit int array (from right to left)
            {
                int ModLength = CommonLength % FragLength;
                int PartCount = CommonLength / FragLength;

                for (int i = PartCount - 1; i >= 0; i--)
                {
                    int start = ModLength + (i * FragLength);
                    Dig1List.Add(Convert.ToInt64(Dig1Orig.Substring(start, FragLength))); //get n-digit string to list
                }

                if (ModLength > 0)
                {
                    Dig1List.Add(Convert.ToInt64(Dig1Orig.Substring(0, ModLength))); //get n-digit string to list
                }
            }
            else
            {
                Dig1List.Add(Convert.ToInt64(Dig1Orig)); //if number is smaller then n digit
            }

            return Dig1List;
        }

        public static string LongListToString(List<long> DigList, string StringPlaces)
        {
            string MyOutput;
            List<string> MyOutputList = new List<string>();

            int loops = DigList.Count - 1;

            for (int i = loops; i >= 0; i--)
            {


                if (i == loops) //(i == DigList.Count - 1)
                {
                    MyOutputList.Add(Convert.ToString(DigList[i]));
                }
                else
                {
                    MyOutputList.Add(DigList[i].ToString(StringPlaces));
                }

            }


            MyOutput = string.Join("", MyOutputList);

            if (MyOutput == "")
            { MyOutput = "0"; }
            return MyOutput;
        }

        public static List<long> ConvertLists(List<long> DigList, long FromMyDiv, long ToMyDiv)
        {
            List<long> MyResult = new List<long>();
            int Loops = DigList.Count - 1;
            if (FromMyDiv > ToMyDiv)
            {
                for (int i = 0; i <= Loops; i++)
                {
                    MyResult.Add(DigList[i] % ToMyDiv);
                    long NextDig = DigList[i] / ToMyDiv;

                    if (NextDig > 0 && i <= Loops)
                    { MyResult.Add(NextDig); }
                }


                if (MyResult.Count == 1)
                { return MyResult; }

                return MyResult;
            }
            else if (FromMyDiv < ToMyDiv)
            {
                for (int i = 0; i <= Loops; i++)
                {
                    MyResult.Add(DigList[i]);

                    if (i < Loops && DigList[i + 1] > 0)
                    { MyResult[MyResult.Count - 1] += DigList[i + 1] * FromMyDiv; }
                    i += 1;
                }

                if (MyResult.Count == 1)
                { return MyResult; }

                return MyResult;
            }


            return DigList;
        }

        public static List<long> CleanListt(List<long> DigList)
        {

            if (DigList.Count == 1)
            {
                return DigList;
            }


            while (DigList[DigList.Count - 1] == 0)
            {
                DigList.RemoveAt(DigList.Count - 1);
                if (DigList.Count == 1)
                { break; }
            }


            return DigList;
        }

    }
}

