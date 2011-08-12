
namespace Demo.Report.ReportItems
{
    public abstract class HeaderBase : IReportItem
    {
        public string Header { get; protected set; }

        protected HeaderBase(string header)
        {
            Header = header;
        }
    }

    public class SectionHeader : HeaderBase
    {
        public SectionHeader(string header) : base(header) {}
    }

    public class TitleHeader : HeaderBase
    {
        public TitleHeader(string header) : base(header) {}
    }

    public class ItemHeader : HeaderBase
    {
        public ItemHeader(string header) : base(header) { }
    }
}
