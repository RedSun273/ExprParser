using System;
using System.Collections.Generic;
using System.Text;

namespace RandomVariable
{
    public enum Token
    {
        EOF,
        Add,
        Subtract,
        Multiply,
        Divide,
        OpenParens,
        CloseParens,
        Number,
        RandomVariable,
    }
}
