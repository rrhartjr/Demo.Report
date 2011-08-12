using System.Collections.Generic;

namespace Demo.Report
{
    public interface IRenderable
    {
        IList<IReportItem> GetDocumentOutline();
    }
}
