using System;
using u4.Core;

FastList<int> numbers = new FastList<int>();
numbers.Add(1);
numbers.Add(2);
numbers.Add(3);
numbers.Add(4);
numbers.Add(5);

Console.WriteLine(numbers.Array.Length);
Console.WriteLine(numbers.Length);