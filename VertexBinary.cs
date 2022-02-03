using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    class VertexBinary : Vertex
    {
        private Vertex leftHandSideVertex;
        private Vertex rightHandSideVertex;
        private Func<double, double, double> operationFunc;
        private Token operation;
        public override bool RandomVariable { get; }

        public VertexBinary(Vertex leftHandSideVertex, Vertex rightHandSideVertex, Func<double, double, double> operation, bool randomVariable)
        {
            this.leftHandSideVertex = leftHandSideVertex;
            this.rightHandSideVertex = rightHandSideVertex;
            operationFunc = operation;
            RandomVariable = randomVariable;
        }

        public VertexBinary(Vertex leftHandSideVertex, Token operation, Vertex rightHandSideVertex, bool randomVariable)
        {
            this.leftHandSideVertex = leftHandSideVertex;
            this.rightHandSideVertex = rightHandSideVertex;
            this.operation = operation;
            RandomVariable = randomVariable;
        }

        public override double Eval()
        {
            var leftHandSideVertexValue = leftHandSideVertex.Eval();
            var rightHandSideVertexValue = rightHandSideVertex.Eval();

            var result = operationFunc(leftHandSideVertexValue, rightHandSideVertexValue);
            return result;
        }

        public override Dictionary<double, double> EvalDistribution()
        {
            if (!leftHandSideVertex.RandomVariable && rightHandSideVertex.RandomVariable)
            {
                return OperationsWithProbabilityDistribution.
                    GetResultBetweenNumberAndProbabilityDistribution(rightHandSideVertex.EvalDistribution(), operation, leftHandSideVertex.Eval(), true);
            }
            else if (leftHandSideVertex.RandomVariable && !rightHandSideVertex.RandomVariable)
            {
                return OperationsWithProbabilityDistribution.
                    GetResultBetweenNumberAndProbabilityDistribution(leftHandSideVertex.EvalDistribution(), operation, rightHandSideVertex.Eval(), false);
            }
            else if (leftHandSideVertex.RandomVariable && rightHandSideVertex.RandomVariable)
            {
                return OperationsWithProbabilityDistribution.
                    GetResultBetweenProbabilityDistributions(leftHandSideVertex.EvalDistribution(), operation, rightHandSideVertex.EvalDistribution());
            }
            else if (operation == Token.RandomVariable)
            {
                return OperationsWithProbabilityDistribution.
                    CalculateProbabilityDistribution((int)leftHandSideVertex.Eval(), (int)rightHandSideVertex.Eval());
            }
            else
            {
                return new Dictionary<double, double>();
            }
        }
    }
}
