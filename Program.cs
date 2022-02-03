using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new RandomVariableStatisticCalculator();
            Console.WriteLine(c.CalculateStatistic("1d6", StatisticKind.ExpectedValue));
        }
    }
}
