namespace RandomVariable
{
    public class RandomVariableStatisticCalculator : IRandomVariableStatisticCalculator
    {
        public RandomVariableStatisticCalculator()
        {
        }

        public RandomVariableStatistic CalculateStatistic(string expression, params StatisticKind[] statisticForCalculate)
        {
            var randomVariableStatistic = new RandomVariableStatistic();
            foreach (var statistic in statisticForCalculate)
            {
                switch (statistic)
                {
                    case StatisticKind.ExpectedValue:
                        randomVariableStatistic.ExpectedValue = Calculator.CalculateRandomVariable(expression, statistic);
                        break;

                    case StatisticKind.Variance:
                        randomVariableStatistic.Variance = Calculator.CalculateRandomVariable(expression, statistic);
                        break;

                    case StatisticKind.ProbabilityDistribution:
                        randomVariableStatistic.ProbabilityDistribution = Calculator.CalculateDistribution(expression);
                        break;
                }
            }
            return randomVariableStatistic;
        }
    }
}