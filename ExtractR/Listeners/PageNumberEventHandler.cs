using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;

namespace ExtractR.Listeners
{
    public class PageNumberEventHandler : IEventHandler
    {
        private readonly Document document;

        public PageNumberEventHandler(Document document)
        {
            this.document = document;
        }
        public void HandleEvent(Event @event)
        {
            PdfDocumentEvent pdfDocumentEvent = (PdfDocumentEvent)@event;

            PdfDocument pdfDocument = pdfDocumentEvent.GetDocument();

            PdfPage pdfPage = pdfDocumentEvent.GetPage();

            Rectangle pageSize = pdfPage.GetPageSize();

            PdfCanvas pdfCanvas = new PdfCanvas(pdfPage)
                  .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12f)
                  .BeginText()
                  .MoveText((pageSize.GetLeft() + document.GetLeftMargin() + pageSize.GetWidth() - document.GetRightMargin()) / 2
                  , pageSize.GetTop() - document.GetTopMargin() + 10f)
                  .ShowText($"Page Number : {pdfDocument.GetPageNumber(pdfPage)}")
                  .EndText();

            pdfCanvas.Release();
        }
    }
}
