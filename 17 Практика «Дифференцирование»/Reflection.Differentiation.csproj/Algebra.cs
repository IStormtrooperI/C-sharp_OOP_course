using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{
    public class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
        {
            return Expression.Lambda<Func<double, double>>(
                new Algebra().Differentiate(function.Body),
                function.Parameters
            );
        }

        private Expression Differentiate(Expression expresFunction)
        {
            if (expresFunction is ParameterExpression parameterExpression)
                return Differentiate(parameterExpression);
            if (expresFunction is BinaryExpression binaryExpression)
                return Differentiate(binaryExpression);
            if (expresFunction is ConstantExpression constantExpression)
                return Differentiate(constantExpression);
            if (expresFunction is MethodCallExpression methodCallExpression)
                return Differentiate(methodCallExpression);

            var errorMessage = expresFunction.ToString();
            var indexOfZ = errorMessage.IndexOf('z');
            throw new ArgumentException(errorMessage.Substring(indexOfZ + 2, errorMessage.Length - 3 - indexOfZ));
        }

        private Expression Differentiate(BinaryExpression expresFunction)
        {
            if (expresFunction.NodeType == ExpressionType.Add) {
                return Expression.Add(
                    new Algebra().Differentiate(expresFunction.Left),
                    new Algebra().Differentiate(expresFunction.Right)
                );
            } 
            else if (expresFunction.NodeType == ExpressionType.Subtract)
            {
                return Expression.Subtract(
                    new Algebra().Differentiate(expresFunction.Left),
                    new Algebra().Differentiate(expresFunction.Right)
                );
            }
            else if (expresFunction.NodeType == ExpressionType.Multiply)
            {
                return Expression.Add(
                    Expression.Multiply(
                        new Algebra().Differentiate(expresFunction.Left),
                        expresFunction.Right
                    ),
                    Expression.Multiply(
                        expresFunction.Left,
                        new Algebra().Differentiate(expresFunction.Right)
                    )
                );
            }
            else if (expresFunction.NodeType == ExpressionType.Divide)
            {
                return Expression.Divide(
                    Expression.Subtract(
                        Expression.Multiply(
                            new Algebra().Differentiate(expresFunction.Left),
                            expresFunction.Right
                        ),
                        Expression.Multiply(
                            expresFunction.Left,
                            new Algebra().Differentiate(expresFunction.Right)
                        )
                    ),
                    Expression.Power(
                        expresFunction.Right,
                        Expression.Constant(2)
                    )
                );
            }

            throw new ArgumentException(expresFunction.NodeType.ToString());
        }

        private Expression Differentiate(ConstantExpression expresFunction)
        {
            return Expression.Constant(0.0);
        }

        private Expression Differentiate(ParameterExpression expresFunction)
        {
            return Expression.Constant(1.0);
        }

        private Expression Differentiate(MethodCallExpression expresFunction)
        {
            var parameters = expresFunction.Arguments.First();
            var diffParameters = new Algebra().Differentiate(parameters);
            var method = expresFunction.Method;

            if (method == typeof(Math).GetMethod("Sin"))
            {
                return Expression.Multiply(
                    diffParameters,
                    Expression.Call(
                        typeof(Math).GetMethod("Cos"),
                        parameters
                    )
                );
            }
            else if (method == typeof(Math).GetMethod("Cos"))
            {
                return Expression.Multiply(
                    Expression.Constant(-1.0),
                    Expression.Multiply(
                        diffParameters,
                        Expression.Call(
                            typeof(Math).GetMethod("Sin"),
                            parameters
                        )
                    )
                );
            }

            throw new ArgumentException(method.Name);
        }
    }
}
