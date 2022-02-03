using System;
using System.Collections.Generic;

namespace RandomVariable
{
    public class VertexUnary : Vertex
    {
        private Vertex rightHandSideVertex;
        private Func<double, double> operationFunc;
        public override bool RandomVariable { get; }

        public VertexUnary(Vertex rightHandSideVertex, Func<double, double> operation, bool randomVariable)
        {
            this.rightHandSideVertex = rightHandSideVertex;
            operationFunc = operation;
            RandomVariable = randomVariable;
        }

        public override double Eval()
        {
            var rightHandSideVertexValue = rightHandSideVertex.Eval();

            var result = operationFunc(rightHandSideVertexValue);
            return result;
        }

        public override Dictionary<double, double> EvalDistribution()
        {
            if (rightHandSideVertex.RandomVariable)
            {
                return OperationsWithProbabilityDistribution.
                    GetResultBetweenNumberAndProbabilityDistribution(rightHandSideVertex.EvalDistribution(), Token.Multiply, -1, false);
            }
            return new Dictionary<double, double>();
        }

    }
}