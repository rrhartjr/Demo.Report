using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Demo.Report.ReportItems;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Demo.Report.Renderers
{
    public static class TextSharpFonts
    {
        public static Font TitleHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
        public static Font SectionHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD | Font.UNDERLINE);
        public static Font ItemHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, Font.BOLD);

        public static Font ItemFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        public static Font ItemBoldFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD);
        public static Font ItemUnderlineFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.UNDERLINE);
        public static Font ItemBoldUnderlineFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD | Font.UNDERLINE);
        public static Font DataFont = FontFactory.GetFont(FontFactory.COURIER, 7);
        public static Font DataBoldFont = FontFactory.GetFont(FontFactory.COURIER, 7, Font.BOLD);
        public static Font DataUnderlineFont = FontFactory.GetFont(FontFactory.COURIER, 7, Font.UNDERLINE);
    }

    public static class TextSharpExtensions
    {
        public static IElement GetElement(this TextSection ts)
        {
            Paragraph p = new Paragraph();

            if (ts.Header != null)
            {
                string header = ts.Header.AddDelimiter(':', s => s.EndsWithAlphanum());
                Phrase h = new Phrase(new Chunk(header, TextSharpFonts.ItemBoldFont));
                p.Add(h);
                p.Add(Chunk.NEWLINE);
            }

            Phrase b = new Phrase(new Chunk(ts.Body, TextSharpFonts.ItemFont));
            p.Add(b);
            p.Add(Chunk.NEWLINE);

            return p;
        }

        public static IElement GetElement(this HeaderBase hb)
        {
            Paragraph p = new Paragraph();

            Font f = TextSharpFonts.SectionHeaderFont;

            if (hb is TitleHeader)
            {
                f = TextSharpFonts.TitleHeaderFont;
                p.Add(new Phrase(hb.Header, f));
                p.Alignment = Element.ALIGN_CENTER;
                p.Leading = f.Size * 2f;
                p.SpacingAfter = f.Size * 1.5f;
            }

            if (hb is SectionHeader)
            {
                p.Add(new Phrase(hb.Header, f));
                p.Leading = f.Size * 1.5f;
            }

            if (hb is ItemHeader)
            {
                p.Add(new Phrase(hb.Header, TextSharpFonts.ItemHeaderFont));
            }

            return p;
        }

        public static IElement GetElement(this DataTuple dt)
        {
            Paragraph p = new Paragraph();

            // add colon if simple string
            string label = dt.Label.AddDelimiter(':', s => s.EndsWithAlphanum());

            p.AddAll(new IElement[] {
                new Chunk(label, TextSharpFonts.ItemBoldFont),
                new Chunk(dt.Value + " ", TextSharpFonts.ItemFont),
                new Chunk(dt.Unit, TextSharpFonts.ItemFont)
            });
            return p;
        }

        private static Paragraph GiveHeaderParagraph(this DataSection ds)
        {
            var widths = ds.ColumnWidths.ToArray();
            var splitted = ds.ColumnHeaders.Select((s, i) =>
            {
                return s.Length > widths[i] ?
                    s.Split(new char[] { ' ' }, StringSplitOptions.None).ToList() :
                    (new[] { s }).ToList();
            }).ToList();

            int maxLength = splitted.Max(arr => arr.Count);

            splitted.ForEach(arr =>
            {
                while (arr.Count < maxLength)
                {
                    arr.Insert(0, "");
                }
            });

            Paragraph pg = new Paragraph();
            pg.SpacingBefore = TextSharpFonts.DataBoldFont.Size;
            pg.Leading = TextSharpFonts.DataBoldFont.Size;
            for (int i = 0; i < maxLength; i++)
            {
                Phrase p = new Phrase();
                for (int j = 0; j < splitted.Count; j++)
                {
                    p.Add(new Chunk(splitted[j][i].Center(widths[j]), TextSharpFonts.DataBoldFont));
                }
                p.Add(Chunk.NEWLINE);
                pg.Add(p);
            }
            return pg;
        }

        public static IEnumerable<IElement> GetElements(this DataSection ds)
        {
            var elems = new List<IElement>();

            // header, if needed
            if (ds.ColumnHeaders.Any())
            {
                elems.Add(ds.GiveHeaderParagraph());
            }

            // process rows
            foreach (IEnumerable<string> ies in ds.Rows)
            {
                // process items
                Phrase p = new Phrase();
                p.Leading = TextSharpFonts.DataFont.Size * 1.1f;
                int j = 0;
                foreach (string s in ies)
                {
                    p.Add(new Chunk(s.Center(ds.ColumnWidths.ToArray()[j]), TextSharpFonts.DataFont));
                    j++;
                }
                p.Add(Chunk.NEWLINE);
                elems.Add(p);
            }
            return elems;
        }
    }

    public class TextSharpPdfDocumentRenderer : IDocumentRenderer
    {
        public void RenderDocument(IRenderable renderable, Stream stream)
        {
            Document document = new Document();
            PdfWriter.GetInstance(document, stream);

            var reportItems = renderable.GetDocumentOutline();
            var headerImage = reportItems.FirstOrDefault(iri => iri is HeaderImage) as HeaderImage;
            Image img = null;
            int pages = 1;

            document.Open();

            if (headerImage != null)
            {
                // if we can find the file
                if (File.Exists(headerImage.ImagePath))
                {
                    // try and load an image
                    try
                    {
                        img = Image.GetInstance(new FileStream(@headerImage.ImagePath, FileMode.Open));
                    }
                    catch
                    {
                        img = null;
                    }
                } 

                // if no valid image, try to get embedded default
                if (img == null)
                {
                    // try to get an embedded default
                    try
                    {
                        img = Image.GetInstance(Assembly.GetExecutingAssembly().GetResourceStream(@"Demo.Report.logo-default.png"));
                    }
                    catch (Exception ex)
                    {
                        img = null;
                    }
                }

                // if we have a valid image here, try to add it
                if (img != null)
                {
                    img.ScaleToFit(document.PageSize.Width - 72f, 150f);
                    img.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
                    document.Add(img);
                } 
                // otherwise, give up
            }

            foreach (IReportItem iri in reportItems)
            {
                if (iri is DataTuple)
                {
                    document.Add((iri as DataTuple).GetElement());
                    continue;
                }

                if (iri is DataSection)
                {
                    foreach (IElement ie in ((iri as DataSection).GetElements()))
                    {
                        document.Add(ie);
                    }
                    continue;
                }

                if (iri is HeaderBase)
                {
                    document.Add((iri as HeaderBase).GetElement());
                    continue;
                }

                if (iri is TextSection)
                {
                    document.Add((iri as TextSection).GetElement());
                    continue;
                }

                if (iri is PageBreak)
                {
                    if (pages > 1 && img != null)
                    {
                        img.ScaleToFit((document.PageSize.Width - 72f) / 2f, 50f);
                        img.SetAbsolutePosition(document.PageSize.Width - img.ScaledWidth - 10f, 10f);
                        document.Add(img);
                    }
                    document.NewPage();
                    pages++;
                    continue;
                }

                if (iri is LineBreak)
                {
                    int i = (iri as LineBreak).Repeat;
                    do
                    {
                        document.Add(new Chunk(Chunk.NEWLINE));
                        i--;
                    } while (i > 0);
                }
            }
            document.Close();
        }
    }
}
