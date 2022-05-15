using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public interface IStatisticsMaker<TStatistics>
	{
		string Caption { get; }
        Statistic<TStatistics> MakeStatistics(string valueType, IEnumerable<double> data);
	}

	public abstract class ReportMaker<TStatistics>
	{
		protected string Caption { get; }

        protected ReportMaker(string caption) => Caption = caption;

        internal abstract string MakeReport(IEnumerable<Statistic<TStatistics>> statistics);
	}

	public class Statistic<T>
    {
        public string ValueType { get; set; }

        public T Entry { get; set; }
    }

	public class Report<TStatistics, TReportMaker, TStatisticsMaker>
		where TReportMaker : ReportMaker<TStatistics>
		where TStatisticsMaker : IStatisticsMaker<TStatistics>, new()
	{
		public static string MakeReport(IEnumerable<Measurement> data)
		{
			var statisticsMaker = new TStatisticsMaker();
			var reportMaker = (TReportMaker)Activator.CreateInstance(typeof(TReportMaker), statisticsMaker.Caption);

			var statisticsList = new List<Statistic<TStatistics>> {
				statisticsMaker.MakeStatistics("Temperature", data.Select(m => m.Temperature)),
				statisticsMaker.MakeStatistics("Humidity", data.Select(m => m.Humidity))
			};

			return reportMaker.MakeReport(statisticsList);
		}
	}

	public class HtmlReportMaker<TStatistics> : ReportMaker<TStatistics>
	{
		public HtmlReportMaker(string caption) : base(caption) { }

		internal override string MakeReport(IEnumerable<Statistic<TStatistics>> statistics)
        {
			var result = new StringBuilder();
			result.Append($"<h1>{Caption}</h1><ul>");
			foreach (Statistic<TStatistics> statistic in statistics)
			{
				result.Append($"<li><b>{statistic.ValueType}</b>: {statistic.Entry}");
			}
			result.Append("</ul>");
			return result.ToString();
        }
	}

	public class MarkdownReportMaker<TStatistics> : ReportMaker<TStatistics>
	{
		public MarkdownReportMaker(string caption) : base(caption) { }

		internal override string MakeReport(IEnumerable<Statistic<TStatistics>> statistics)
        {
			var result = new StringBuilder();
			result.Append($"## {Caption}\n\n");
			result.Append("");
			foreach (Statistic<TStatistics> statistic in statistics)
			{
				result.Append($" * **{statistic.ValueType}**: {statistic.Entry}\n\n");
			}
			result.Append("");
			return result.ToString();
		}
	}

	public class MeanAndStdMaker : IStatisticsMaker<MeanAndStd>
    {
        public string Caption
        {
			get
			{
				return "Mean and Std";
			}
        }

		public Statistic<MeanAndStd> MakeStatistics(string valueType, IEnumerable<double> data)
        {
			var mean = data.Average();
			var entry = new MeanAndStd
			{
				Mean = mean,
				Std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count() - 1))
			};
			return new Statistic<MeanAndStd>
			{
				ValueType = valueType,
				Entry = entry
			};
        }
	}

	public class MedianMaker : IStatisticsMaker<double>
	{
		public string Caption
        {
            get
            {
				return "Median";
            }
        }

		public Statistic<double> MakeStatistics(string valueType, IEnumerable<double> data)
		{
			var list = data.OrderBy(z => z).ToList();
			double entry;
			if (list.Count % 2 == 0)
				entry = (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
			else
				entry = list[list.Count / 2];
			return new Statistic<double>
			{
				ValueType = valueType,
				Entry = entry
			};
		}
	}

	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			return Report<MeanAndStd, HtmlReportMaker<MeanAndStd>, MeanAndStdMaker>.MakeReport(data);
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
			return Report<double, MarkdownReportMaker<double>, MedianMaker>.MakeReport(data);
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
			return Report<MeanAndStd, MarkdownReportMaker<MeanAndStd>, MeanAndStdMaker>.MakeReport(measurements);
		}

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
			return Report<double, HtmlReportMaker<double>, MedianMaker>.MakeReport(measurements);
		}
	}
}
