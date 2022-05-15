using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        private readonly int numerator;
        private readonly int denominator;
        private readonly bool isNan;
        public bool IsNan
        {
            get; set;
        }

        private static int GetReduction(int numerator, int denominator)
        {
            if (numerator == denominator)
                return numerator;
            if (numerator > denominator)
                return GetReduction(numerator - denominator, denominator);
            return GetReduction(denominator - numerator, numerator);
        }

        public int Numerator
        {
            get; set;
        }
        public int Denominator
        {
            get; set;
        }

        public Rational(int num, int den = 1)
        {
            Numerator = num;
            Denominator = den;

            if (Denominator < 0)
            {
                Numerator *= -1;
                Denominator *= -1;
            }

            if (Denominator == 0)
            {
                IsNan = true;
            }
            else
            {
                IsNan = false;
                if (Numerator != 0)
                {
                    var reduction = GetReduction(Math.Abs(Numerator), Math.Abs(Denominator));
                    Numerator /= reduction;
                    Denominator /= reduction;
                }
                else
                {
                    Denominator = 1;
                }
            }
        }

        public static Rational operator +(Rational firstR, Rational secondR)
        {
            var numirator = firstR.Numerator * secondR.Denominator + secondR.Numerator * firstR.Denominator;
            var denominator = firstR.Denominator * secondR.Denominator;
            var rational = new Rational(numirator, denominator);
            return rational;
        }

        public static Rational operator -(Rational firstR, Rational secondR)
        {
            var numirator = firstR.Numerator * secondR.Denominator - secondR.Numerator * firstR.Denominator;
            var denominator = firstR.Denominator * secondR.Denominator;
            var rational = new Rational(numirator, denominator);
            return rational;
        }

        public static Rational operator *(Rational firstRational, Rational secondRational)
        {
            return new Rational(firstRational.Numerator * secondRational.Numerator,
                firstRational.Denominator * secondRational.Denominator);
        }

        public static Rational operator /(Rational firstRational, Rational secondRational)
        {
            if (secondRational.IsNan)
            {
                return new Rational(0, 0);
            }
            return new Rational(firstRational.Numerator * secondRational.Denominator,
                firstRational.Denominator * secondRational.Numerator);
        }

        public static implicit operator double(Rational rational)
        {
            if (rational.IsNan)
                return double.NaN;
            return (double)rational.Numerator / (double)rational.Denominator;
        }

        public static implicit operator Rational(int num)
        {
            return new Rational(num);
        }

        public static explicit operator int(Rational rational)
        {
            if (rational.IsNan)
                throw new Exception();
            if (rational.Numerator % rational.Denominator != 0)
                throw new Exception();
            return rational.Numerator / rational.Denominator;
        }
    }
}