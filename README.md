# Miki-Calculator
Mega-Integer-string (K)Calculator Interface.

Calculator for very long numbers (max lenght of string - int.MaxValue characters). Input and output is in "Integer-string" style.

MikiAdd, MikiSub, MikiMul - returns [0]-Int-string-Result, [1]-Abs(Int-string-Result)
MikiDiv - returns [0]-Int-string-Result, [1]-Rest from division (not modulo)
MikiPow - returns [0]-Int-string-Result

The program is very easy to modify to compute on a decimal-style-string, and I think on longer numbers than int.MaxValue too.

In addision, using Miki-Calculator you can generate pseudo-random passwords from pseudo-random huge numbers, and generate numbers from passwords.
To generate passwords I used Lehmer algorithm.

I will be grateful for your feedback and any help in developing the program.
