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
            var pd = c.CalculateStatistic("1d4", new[] { StatisticKind.ProbabilityDistribution, StatisticKind.ExpectedValue, StatisticKind.Variance });
            foreach (var d in pd.ProbabilityDistribution)
                Console.WriteLine(d.Key + " " + d.Value);
            Console.WriteLine(" ");
            Console.WriteLine(pd.Variance);
            Console.WriteLine(" ");
            Console.WriteLine(pd.ExpectedValue);
            Console.WriteLine(" ");
        }
    }
}
