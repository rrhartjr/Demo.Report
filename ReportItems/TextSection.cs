using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Report.ReportItems
{
    public class TextSection : IReportItem
    {
        public string Header { get; set; }
        public string Body { get; set; }

        public TextSection(IEnumerable<string> items)
        {
            Header = items.ElementAtOrDefault(0) ?? "";
            Body = items.ElementAtOrDefault(1) ?? "";
        }

        public TextSection(string body, string header = null)
        {
            Header = header;
            Body = body;
        }
    }
}
