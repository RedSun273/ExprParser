using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RandomVariable
{
    public class Tokenizer
    {
        private readonly TextReader reader;
        private char currentChar;
        public Token Token { get; private set; }
        public double Number { get; private set; }

        public Tokenizer(TextReader reader)
        {
            this.reader = reader;
            NextChar();
            NextToken();
        }

        private void NextChar()
        {
            int c = reader.Read();
            currentChar = c == -1 ? '\0' : (char)c;
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(currentChar))
            {
                NextChar();
            }

            switch (currentChar)
            {
                case '\0':
                    Token = Token.EOF;
                    return;

                case '+':
                    NextChar();
                    Token = Token.Add;
                    return;

                case '-':
                    NextChar();
                    Token = Token.Subtract;
                    return;

                case '*':
                    NextChar();
                    Token = Token.Multiply;
                    return;

                case '/':
                    NextChar();
                    Token = Token.Divide;
                    return;

                case '(':
                    NextChar();
                    Token = Token.OpenParens;
                    return;

                case ')':
                    NextChar();
                    Token = Token.CloseParens;
                    return;

                case 'd':
                    NextChar();
                    Token = Token.RandomVariable;
                    return;

            }

            if (char.IsDigit(currentChar) || currentChar == '.')
            {
                var numberBuilder = new StringBuilder();
                var haveDecimalPoint = false;
                while (char.IsDigit(currentChar) || (!haveDecimalPoint && currentChar == '.'))
                {
                    numberBuilder.Append(currentChar);
                    haveDecimalPoint = currentChar == '.';
                    NextChar();
                }

                Number = double.Parse(numberBuilder.ToString(), CultureInfo.InvariantCulture);
                Token = Token.Number;
            }
        }
    }
}
