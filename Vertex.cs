using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    public abstract class Vertex
    {
        public abstract bool RandomVariable { get; }
        public abstract double Eval();
        public abstract Dictionary<double, double> EvalDistribution();
    }
}
