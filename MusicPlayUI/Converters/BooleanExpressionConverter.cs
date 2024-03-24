using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MusicPlayUI.Converters
{
    internal class BooleanExpressionConverter : MarkupExtension,
            IMultiValueConverter,
            IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new object[] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Parse(parameter.ToString(), values).Eval(values);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected virtual void ProcessException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private IExpression Parse(string s, object[] args)
        {
            return new Parser().Parse(s, args);
        }

        interface IExpression
        {
            bool Eval(object[] args);
        }

        class BinaryOperation : IExpression
        {
            private Func<decimal, decimal, bool> _operation;
            private decimal _left;
            private decimal _right;

            public BinaryOperation(string operation, decimal left, decimal right)
            {
                _left = left;
                _right = right;
                _operation = operation switch
                {
                    "==" => (a, b) => a == b,
                    "!=" => (a, b) => a != b,
                    ">=" => (a, b) => a >= b,
                    "<=" => (a, b) => a <= b,
                    ">" => (a, b) => a > b,
                    "<" => (a, b) => a < b,
                    _ => throw new ArgumentException("Invalid operation " + operation),
                };
            }

            public bool Eval(object[] args)
            {
                return _operation(_left, _right);
            }
        }

        class Variable
        {
            private int _index;

            public Variable(string text)
            {
                if (!int.TryParse(text, out _index) || _index < 0)
                {
                    throw new ArgumentException(String.Format("'{0}' is not a valid parameter index", text));
                }
            }

            public Variable(int n)
            {
                _index = n;
            }

            public decimal Eval(object[] args)
            {
                if (_index >= args.Length)
                {
                    throw new ArgumentException(String.Format("MathConverter: parameter index {0} is out of range. {1} parameter(s) supplied", _index, args.Length));
                }

                return System.Convert.ToDecimal(args[_index]);
            }
        }

        class Parser
        {
            private string text;
            private int pos;
            private object[] _args;

            public IExpression Parse(string text, object[] args)
            {
                _args = args;
                try
                {
                    pos = 0;
                    this.text = text;
                    var result = ParseExpression();
                    return result;
                }
                catch (Exception ex)
                {
                    string msg =
                        String.Format("MathConverter: error parsing expression '{0}'. {1} at position {2}", text, ex.Message, pos);

                    throw new ArgumentException(msg, ex);
                }
            }

            private IExpression ParseExpression()
            {
                string left = "";
                string right = "";
                string operation = "";

                decimal _left = decimal.MaxValue;
                decimal _right = decimal.MaxValue;

                int term = 0;
                while (true)
                {
                    if (pos >= text.Length)
                    {
                        if(term == 2)
                        {
                            if(_left == decimal.MaxValue)
                                _left =  System.Convert.ToDecimal(left);
                            if(_right == decimal.MaxValue)
                                _right = System.Convert.ToDecimal(right);

                            return new BinaryOperation(operation, _left, _right);
                        }
                        else
                        {
                            throw new ArgumentException("Unexpected end of expression");
                        }
                    }

                    var c = text[pos];

                    decimal variable = decimal.MaxValue;
                    if (c == 'x' || c == 'a') variable = CreateVariable(0, _args);
                    else if (c == 'y' || c == 'b') variable = CreateVariable(1, _args);
                    else if (c == 'z' || c == 'c') variable = CreateVariable(2, _args);
                    else if (c == 't' || c == 'd') variable = CreateVariable(3, _args);

                    if(variable != decimal.MaxValue)
                    {
                        if(term == 0)
                        {
                            _left = variable;
                            term = 1;
                        }
                        else
                        {
                            _right = variable;
                            pos = text.Length;
                        }
                        continue;
                    }

                    if (c == '=' || c == '!' || c == '<' || c == '>')
                    {
                        operation += c;

                        bool canBeSingleChar = c == '<' || c == '>';
                        if (term == 2)
                        {
                            throw new ArgumentException("Not supported operation");
                        }
                        if(operation.Length == 2 || (canBeSingleChar && text[pos + 1] == ' '))
                        {
                            term = 2;
                        }
                        else
                        {
                            term = 1;
                        }
                    }
                    else if(term == 0)
                    {
                        left += c;
                    }
                    else
                    {
                        right += c;
                    }
                    ++pos;
                }
            }

            private decimal CreateVariable(int n, object[] args)
            {
                ++pos;
                SkipWhiteSpace();
                return new Variable(n).Eval(args);
            }

            private void SkipWhiteSpace()
            {
                while (pos < text.Length && Char.IsWhiteSpace((text[pos]))) ++pos;
            }

            private void RequireEndOfText()
            {
                if (pos != text.Length)
                {
                    throw new ArgumentException("Unexpected character '" + text[pos] + "'");
                }
            }
        }
    }
}
