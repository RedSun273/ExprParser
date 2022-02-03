using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RandomVariable
{
    public class ExpressionParser
    {
        private Tokenizer tokenizer;
        private StatisticKind statisticKind;
        private readonly Dictionary<Token, Func<double, double, double>> operationDictionaryWithExpectedValue =
            new Dictionary<Token, Func<double, double, double>>()
            {
                {Token.RandomVariable,
                    (a,b) =>  a * (1 + b) / 2},
                {Token.Add,
                    (a,b) =>  a + b},
                {Token.Subtract,
                    (a,b) =>  a - b},
                {Token.Multiply,
                    (a,b) =>  a * b},
                {Token.Divide,
                    (a,b) =>  a / b},
            };
        private readonly Dictionary<Tuple<bool, Token, bool>,
                Func<double, double, double>> operationDictionaryWithVariance =
            new Dictionary<Tuple<bool, Token, bool>,
                Func<double, double, double>>()
            {
                {Tuple.Create(false, Token.RandomVariable, false),
                    (a,b) =>  Math.Abs(a) * ((b * b - 1) / 12)},
                {Tuple.Create(true, Token.Add, false),
                    (a,b) =>  a},
                {Tuple.Create(false, Token.Add, true),
                    (a,b) =>  b},
                {Tuple.Create(false, Token.Add, false),
                    (a,b) =>  0},
                {Tuple.Create(true, Token.Add, true),
                    (a,b) =>  Math.Abs(a) + Math.Abs(b)},
                {Tuple.Create(true, Token.Subtract, false),
                    (a,b) =>  a},
                {Tuple.Create(false, Token.Subtract, true),
                    (a,b) =>  b},
                {Tuple.Create(false, Token.Subtract, false),
                    (a,b) =>  0},
                {Tuple.Create(true, Token.Subtract, true),
                    (a,b) =>  Math.Abs(a) + Math.Abs(b)},
                {Tuple.Create(true, Token.Multiply, false),
                    (a,b) =>  a * b * b},
                {Tuple.Create(false, Token.Multiply, true),
                    (a,b) =>  a * a * b},
                {Tuple.Create(false, Token.Multiply, false),
                    (a,b) =>  a * b},
                {Tuple.Create(true, Token.Divide, false),
                    (a,b) =>  a / (b * b)},
                {Tuple.Create(false, Token.Divide, true),
                    (a,b) =>  (a * a) / b},
                {Tuple.Create(false, Token.Divide, false),
                    (a,b) =>  a / b},
            };

        public static Vertex ParseToExpressionTree(string str, StatisticKind statisticKind)
        {
            var tokenizer = new Tokenizer(new StringReader(str));
            var parser = new ExpressionParser(tokenizer, statisticKind);
            return parser.ParseToTreeExpression();
        }

        private ExpressionParser(Tokenizer tokenizer, StatisticKind statisticKind)
        {
            this.statisticKind = statisticKind;
            this.tokenizer = tokenizer;
        }

        private Vertex ParseToTreeExpression()
        {
            var expression = ParseAddSubtract();

            if (tokenizer.Token != Token.EOF)
                throw new Exception("Unexpected characters at end of expression");

            return expression;
        }

        private Vertex ParseAddSubtract()
        {
            var leftHandSideVertex = ParseMultiplyDivide();

            while (true)
            {
                if (tokenizer.Token != Token.Add && tokenizer.Token != Token.Subtract)
                {
                    return leftHandSideVertex;
                }

                var operation = tokenizer.Token;

                tokenizer.NextToken();

                var rightHandSideVertex = ParseMultiplyDivide();

                leftHandSideVertex = ResultVertexAfterOpertaion(leftHandSideVertex, rightHandSideVertex, operation);
            }
        }

        private Vertex ParseMultiplyDivide()
        {
            var leftHandSideVertex = ParseRandomVariable();

            while (true)
            {
                if (tokenizer.Token != Token.Multiply && tokenizer.Token != Token.Divide)
                {
                    return leftHandSideVertex;
                }

                var operation = tokenizer.Token;

                tokenizer.NextToken();

                var rightHandSideVertex = ParseRandomVariable();

                if (leftHandSideVertex.RandomVariable && rightHandSideVertex.RandomVariable)
                {
                    throw new Exception("Invalid expression");
                }
                else
                {
                    leftHandSideVertex = ResultVertexAfterOpertaion(leftHandSideVertex, rightHandSideVertex, operation);
                }
            }
        }

        private Vertex ParseRandomVariable()
        {
            var leftHandSideVertex = ParseUnary();

            if (tokenizer.Token != Token.RandomVariable)
            {
                return leftHandSideVertex;
            }

            var operation = tokenizer.Token;

            tokenizer.NextToken();

            var rightHandSideVertex = ParseUnary();

            if (leftHandSideVertex.RandomVariable || rightHandSideVertex.RandomVariable)
            {
                throw new Exception("Invalid expression");
            }
            else
            {
                leftHandSideVertex = ResultVertexAfterOpertaion(leftHandSideVertex, rightHandSideVertex, operation);
            }

            return leftHandSideVertex;
        }

        private Vertex ParseUnary()
        {

            if (tokenizer.Token == Token.Add)
            {
                tokenizer.NextToken();
                return ParseUnary();
            }

            if (tokenizer.Token == Token.Subtract)
            {
                tokenizer.NextToken();

                var rightHandSideVertex = ParseUnary();

                if (rightHandSideVertex.RandomVariable && statisticKind == StatisticKind.Variance)
                {
                    return new VertexUnary(rightHandSideVertex, (a) => a, rightHandSideVertex.RandomVariable);
                }

                return new VertexUnary(rightHandSideVertex, (a) => -a, rightHandSideVertex.RandomVariable);
            }

            return ParseVertex();
        }

        private Vertex ParseVertex()
        {
            if (tokenizer.Token == Token.Number)
            {
                var vertex = new VertexNumber(tokenizer.Number, false);
                tokenizer.NextToken();
                return vertex;
            }

            if (tokenizer.Token == Token.OpenParens)
            {
                tokenizer.NextToken();

                var vertex = ParseAddSubtract();

                if (tokenizer.Token != Token.CloseParens)
                    throw new Exception("Missing close parenthesis");

                tokenizer.NextToken();

                return vertex;
            }

            throw new Exception($"Unexpect token: {tokenizer.Token}");
        }

        private VertexBinary ResultVertexAfterOpertaion(Vertex leftHandSideVertex, Vertex rightHandSideVertex, Token operation)
        {
            var containsRandomVariable = operation == Token.RandomVariable ? true :
                leftHandSideVertex.RandomVariable || rightHandSideVertex.RandomVariable;

            if (statisticKind == StatisticKind.ExpectedValue)
            {
                return new VertexBinary(leftHandSideVertex, rightHandSideVertex,
                    operationDictionaryWithExpectedValue[operation], containsRandomVariable);
            }
            else if (statisticKind == StatisticKind.Variance)
            {
                return new VertexBinary(leftHandSideVertex, rightHandSideVertex,
                    operationDictionaryWithVariance[Tuple.Create(leftHandSideVertex.RandomVariable, operation, rightHandSideVertex.RandomVariable)],
                    containsRandomVariable);
            }
            else
            {
                return GetVertexBinaryProbabilityDistribution(leftHandSideVertex, rightHandSideVertex, operation);
            }
        }

        private VertexBinary GetVertexBinaryProbabilityDistribution(Vertex leftHandSideVertex, Vertex rhs, Token operation)
        {
            if (operation != Token.RandomVariable && !(leftHandSideVertex.RandomVariable || rhs.RandomVariable))
            {
                return new VertexBinary(leftHandSideVertex, rhs,
                    operationDictionaryWithExpectedValue[operation], false);
            }
            else if (operation == Token.RandomVariable)
            {
                return new VertexBinary(leftHandSideVertex, operation, rhs, true);
            }
            else
            {
                return new VertexBinary(leftHandSideVertex, operation, rhs, true);
            }
        }
    }
}
