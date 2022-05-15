using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis {
    public static class Analysis
    {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> dates)
        {
            var defaultValue = default(T);
            int count = 0;
            foreach (var date in dates)
            {
                if (count++ > 0)
                    yield return new Tuple<T, T>(defaultValue, date);
                defaultValue = date;
            }
        }

        public static int MaxIndex<T>(this IEnumerable<T> dates)
            where T : IComparable, new()
        {
            var dateList = dates.ToList();
            if (dateList.Count == 0)
                throw new InvalidOperationException();
            return dateList.IndexOf(dateList.Max());
        }

        private static void CheckLength<T>(IEnumerable<T> dates) {
            if (dates.Count() < 2)
                throw new InvalidOperationException();
        }

        public static int FindMaxPeriodIndex(params DateTime[] dates) {
            CheckLength(dates);
            return dates.Pairs().Select(t => t.Item2 - t.Item1).MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] dates) {
            CheckLength(dates);
            return dates.Pairs().Select(t => (t.Item2 - t.Item1) / t.Item1).Average();
        }
    }
}