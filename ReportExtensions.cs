using System.IO;
using Demo.Report.Renderers;
using System.Reflection;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Demo.Report
{
    public static class ReportExtensions
    {
        public static void RenderAndSave<T>(this IRenderable renderable, string fpath)
            where T: IDocumentRenderer, new()
        {
            using (FileStream fs = new FileStream(fpath, FileMode.Create))
            {
                IDocumentRenderer renderer = new T();
                renderer.RenderDocument(renderable, fs);
            }
        }

        public static Stream GetResourceStream(this Assembly asm, string resource)
        {
            return asm.GetManifestResourceStream(resource);
        }

        public static string AddDelimiter(this string s, char delimiter, Func<string, bool> predicate)
        {
            string str = s.Trim();
            if (predicate.Invoke(str))
                str += delimiter;
            return str + " ";
        }

        public static string Center(this string s, int width)
        {
            int spaceNeeded;
            StringBuilder sb = new StringBuilder(s);
            while (true)
            {
                spaceNeeded = width - sb.Length;
                // too long, truncate it
                if (spaceNeeded < 0)
                {
                    return sb.ToString().Substring(0, width);
                }
                // just right, break loop
                else if (spaceNeeded == 0)
                {
                    break;
                }
                // just right after we add one space to end, break loop
                else if (spaceNeeded == 1)
                {
                    sb.Insert(0, " ");
                    break;
                }
                // need more padding (at least 2 spaces), pad both sides
                else if (spaceNeeded > 1)
                {
                    sb.Insert(0, " ");
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }

        public static bool EndsWithAlphanum(this string s)
        {
            return Regex.IsMatch(s, "[A-Za-z0-9]$");
        }

        public static bool StartsWithAlphaNum(this string s)
        {
            return Regex.IsMatch(s, "^[A-Za-z0-9]");
        }
    }
}
