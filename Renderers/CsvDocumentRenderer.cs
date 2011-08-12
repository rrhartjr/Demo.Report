using System;
using System.IO;
using Demo.Report.ReportItems;
using System.Linq;
using System.Text;

namespace Demo.Report.Renderers
{
    public static class CsvExtensions
    {
        public static string CsvEntry(this string s)
        {
            // enclose in quotes and pad with leading space if doesnt start with alphanum
            return
                s.StartsWithAlphaNum() ?
                string.Format("\"{0}\",", s) :
                string.Format("\" {0}\",", s);
        }
    }

    public class CsvDocumentRenderer : IDocumentRenderer
    {
        public void RenderDocument(IRenderable renderable, Stream stream)
        {
            // create csv document
            var reportItems = renderable.GetDocumentOutline();
            var writer = new StreamWriter(stream, Encoding.UTF8);

            // iterate report items
            foreach (IReportItem iri in reportItems)
            {
                if (iri is DataTuple)
                {
                    var dt = iri as DataTuple;
                    writer.Write(
                        dt.Label.CsvEntry() +
                        dt.Value.CsvEntry() +
                        dt.Unit.CsvEntry() +
                        System.Environment.NewLine
                    );
                    continue;
                }

                if (iri is DataSection)
                {
                    var ds = iri as DataSection;

                    // header row
                    writer.Write(string.Join(",", ds.ColumnHeaders) + System.Environment.NewLine);

                    // for each row
                    foreach (var list in ds.Rows)
                    {
                        writer.Write(string.Join(string.Empty, list.Select(s => s.CsvEntry()).ToArray()) + System.Environment.NewLine);
                    }
                    continue;
                }

                if (iri is SectionHeader)
                {
                    writer.Write(string.Format("### {0} ###" + System.Environment.NewLine, (iri as SectionHeader).Header));
                    continue;
                }

                if (iri is TitleHeader)
                {
                    writer.Write(string.Format("--- {0} ---" + System.Environment.NewLine, (iri as TitleHeader).Header));
                    continue;
                }

                if (iri is ItemHeader)
                {
                    writer.Write(string.Format("- {0} -" + System.Environment.NewLine, (iri as ItemHeader).Header));
                    continue;
                }

                if (iri is TextSection)
                {
                    TextSection ts = iri as TextSection;
                    writer.Write(ts.Header + System.Environment.NewLine);
                    writer.Write(ts.Body.CsvEntry() + System.Environment.NewLine);
                }

                if (iri is PageBreak)
                {
                    writer.Write(string.Concat(Enumerable.Repeat(System.Environment.NewLine, 5)));
                }

                if (iri is LineBreak)
                {
                    writer.Write(System.Environment.NewLine);
                }
            }

            writer.Close();
        }
    }
}
