using System;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.TES4.AST.Expression.Operators
{
    class TES4ComparisonExpressionOperator : TES4ExpressionOperator
    {
        private readonly Func<decimal, decimal, bool> func;
        private TES4ComparisonExpressionOperator(string name, Func<decimal, decimal, bool> func)
            : base(name)
        {
            this.func = func;
        }

        public bool Evaluate(decimal left, decimal right)
        {
            return func(left, right);
        }

        public static readonly TES4ComparisonExpressionOperator
            OPERATOR_EQUAL = new TES4ComparisonExpressionOperator("==", (l, r) => l == r),
            OPERATOR_NOT_EQUAL = new TES4ComparisonExpressionOperator("!=", (l, r) => l != r),
            OPERATOR_GREATER = new TES4ComparisonExpressionOperator(">", (l, r) => l > r),
            OPERATOR_GREATER_OR_EQUAL = new TES4ComparisonExpressionOperator(">=", (l, r) => l >= r),
            OPERATOR_LESS = new TES4ComparisonExpressionOperator("<", (l, r) => l < r),
            OPERATOR_LESS_OR_EQUAL = new TES4ComparisonExpressionOperator("<=", (l, r) => l <= r);

        private static readonly TES4ComparisonExpressionOperator[] all = new TES4ComparisonExpressionOperator[]
        {
            OPERATOR_EQUAL,
            OPERATOR_NOT_EQUAL,
            OPERATOR_GREATER,
            OPERATOR_GREATER_OR_EQUAL,
            OPERATOR_LESS,
            OPERATOR_LESS_OR_EQUAL
        };

        public static TES4ComparisonExpressionOperator GetFirst(string name)
        {
            return all.Where(o => o.Name == name).First();
        }
    }
}