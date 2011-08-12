using System.IO;

namespace Demo.Report
{
    public interface IDocumentRenderer
    {
        void RenderDocument(IRenderable renderable, Stream stream);
    }
}
