using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    public class VertexNumber : Vertex
    {
        private double number;
        public override bool RandomVariable { get; }
        public VertexNumber(double number, bool randomVariable)
        {
            this.number = number;
            RandomVariable = randomVariable;
        }
        public override double Eval()
        {
            return number;
        }

        public override Dictionary<double, double> EvalDistribution()
        {
            return new Dictionary<double, double>();
        }
    }
}
