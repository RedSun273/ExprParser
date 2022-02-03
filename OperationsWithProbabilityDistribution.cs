using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomVariable
{
    public class OperationsWithProbabilityDistribution
    {
        public static Dictionary<double, double> CalculateProbabilityDistribution(int leftHandSideVertex, int rightHandSideVertex)
        {
            var resultProbabilityDistributionNodes = new Dictionary<double, double>();

            for (int i = 1; i <= rightHandSideVertex; i++)
            {
                resultProbabilityDistributionNodes[i] = 1 / (double)rightHandSideVertex;
            }

            var copyStartProbabilityDistributionNodes = new Dictionary<double, double>(resultProbabilityDistributionNodes);
            for (int i = 0; i < Math.Abs(leftHandSideVertex) - 1; i++)
            {
                var tempProbabilityDistributionNodes = new Dictionary<double, double>();
                var keysResultDictionary = resultProbabilityDistributionNodes.Keys.ToList();
                var keysStartDictionary = copyStartProbabilityDistributionNodes.Keys.ToList();

                for (int j = 0; j < keysResultDictionary.Count; j++)
                {
                    for (int k = 0; k < keysStartDictionary.Count; k++)
                    {
                        var probability = resultProbabilityDistributionNodes[keysResultDictionary[j]] * copyStartProbabilityDistributionNodes[keysStartDictionary[k]];
                        if (tempProbabilityDistributionNodes.ContainsKey(keysResultDictionary[j] + keysStartDictionary[k]))
                        {
                            tempProbabilityDistributionNodes[keysResultDictionary[j] + keysStartDictionary[k]] +=
                            probability;
                        }
                        else
                            tempProbabilityDistributionNodes[keysResultDictionary[j] + keysStartDictionary[k]] =
                            probability;
                    }
                }
                resultProbabilityDistributionNodes =
                    new Dictionary<double, double>(tempProbabilityDistributionNodes);
            }

            if (leftHandSideVertex < 0)
            {
                return GetResultBetweenNumberAndProbabilityDistribution(resultProbabilityDistributionNodes, Token.Multiply, -1, false);
            }
            return resultProbabilityDistributionNodes;
        }

        public static Dictionary<double, double> GetResultBetweenNumberAndProbabilityDistribution(Dictionary<double, double> leftHandSideVertex, Token operation, double rightHandSideVertex, bool revesed)
        {
            var resultDictionary = new Dictionary<double, double>();
            List<double> keysList = leftHandSideVertex.Keys.ToList();

            for (int i = 0; i < keysList.Count; i++)
            {
                double keyForResultDictionary = 0;

                switch (operation)
                {
                    case Token.Add:
                        keyForResultDictionary = keysList[i] + rightHandSideVertex;
                        break;

                    case Token.Multiply:
                        keyForResultDictionary = keysList[i] * rightHandSideVertex;
                        break;

                    case Token.Subtract:
                        keyForResultDictionary = revesed ? rightHandSideVertex - keysList[i] : keysList[i] - rightHandSideVertex;
                        break;

                    case Token.Divide:
                        keyForResultDictionary = revesed ? rightHandSideVertex / keysList[i] : keysList[i] / rightHandSideVertex;
                        break;
                }
                if (resultDictionary.ContainsKey(keyForResultDictionary))
                {
                    resultDictionary[keyForResultDictionary] += leftHandSideVertex[keysList[i]];
                }
                else
                    resultDictionary[keyForResultDictionary] = leftHandSideVertex[keysList[i]];
            }
            return resultDictionary;
        }

        public static Dictionary<double, double> GetResultBetweenProbabilityDistributions(Dictionary<double, double> leftHandSideVertex, Token operation, Dictionary<double, double> rightHandSideVertex)
        {
            var keysListFromLHS = leftHandSideVertex.Keys.ToList();
            var keysListFromRHS = rightHandSideVertex.Keys.ToList();

            var resultDictionary = new Dictionary<double, double>();

            for (int i = 0; i < keysListFromLHS.Count; i++)
            {
                for (int j = 0; j < keysListFromRHS.Count; j++)
                {
                    double keyForResultDictionary;

                    keyForResultDictionary = operation == Token.Add ? keysListFromLHS[i] + keysListFromRHS[j] :
                        keysListFromLHS[i] - keysListFromRHS[j];

                    if (resultDictionary.ContainsKey(keyForResultDictionary))
                    {
                        resultDictionary[keyForResultDictionary] += leftHandSideVertex[keysListFromLHS[i]] * rightHandSideVertex[keysListFromRHS[j]];
                    }
                    else
                        resultDictionary[keyForResultDictionary] = leftHandSideVertex[keysListFromLHS[i]] * rightHandSideVertex[keysListFromRHS[j]];
                }
            }

            return resultDictionary;
        }
    }
}
