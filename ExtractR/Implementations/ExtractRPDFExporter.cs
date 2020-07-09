using ExtractR.Listeners;
using iText.IO.Image;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtractR.Implementations
{
    public class ExtractRPDFExporter
    {
        /// <summary>
        /// Exports all images in the directory as a composite PDF File.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool ExportPDF(string directory, string resultingFileName)
        {
            try
            {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(resultingFileName));


                Document document = new Document(pdfDocument, PageSize.A4);

                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageNumberEventHandler(document));

                var files = Directory.EnumerateFiles(directory)
                    .Where(x => x.EndsWith("png") || x.EndsWith("jpg") || x.EndsWith("gif"));

                var pageArea = document.GetPageEffectiveArea(PageSize.A4);
                foreach (var file in files)
                {
                    Image image = new Image(ImageDataFactory.Create(new Uri(file)))
                        .ScaleToFit(pageArea.GetWidth(), pageArea.GetHeight());

                    document.Add(image);

                    document.Add(new Paragraph().SetMarginTop(30f));

                }

                pdfDocument.Close();

                File.SetLastWriteTime(resultingFileName, DateTime.Now);

                return true;

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
