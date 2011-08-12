using System.Collections.Generic;
using System.Linq;

namespace Demo.Report.ReportItems
{
    public class DataTuple : IReportItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

        public DataTuple(IEnumerable<string> items)
        {
            Label = items.ElementAtOrDefault(0) ?? "";
            Value = items.ElementAtOrDefault(1) ?? "";
            Unit = items.ElementAtOrDefault(2) ?? "";
        }

        public DataTuple(string label, string value, string unit)
        {
            Label = label;
            Value = value;
            Unit = unit;
        }
    }
}