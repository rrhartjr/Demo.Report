using System.Collections.Generic;

namespace Demo.Report.ReportItems
{
    public class DataSection : IReportItem
    {
        public IEnumerable<int> ColumnWidths { get; set; }
        public IEnumerable<string> ColumnHeaders { get; set; }
        public IEnumerable<IEnumerable<string>> Rows { get; set; }

        public DataSection()
        {
            ColumnWidths = new int[] { };
            ColumnHeaders = new List<string>();
            Rows = new List<IEnumerable<string>>();
        }
    }
}
