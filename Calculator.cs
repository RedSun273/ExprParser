using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    public class Calculator
    {
        public static Dictionary<double, double> CalculateDistribution(string str)
        {
            var probabilityDistribution = ExpressionParser.ParseToExpressionTree(str, StatisticKind.ProbabilityDistribution).EvalDistribution();
            return new Dictionary<double, double>(probabilityDistribution);
        }

        public static double CalculateRandomVariable(string str, StatisticKind statisticKind)
        {
            var calculatedRandomVariable = ExpressionParser.ParseToExpressionTree(str, statisticKind).Eval();
            return calculatedRandomVariable;
        }
    }
}
