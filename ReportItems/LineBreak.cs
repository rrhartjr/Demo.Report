using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Report.ReportItems
{
    public class LineBreak : IReportItem
    {
        public int Repeat { get; set; }

        public LineBreak()
        {
            Repeat = 1;
        }
    }
}
