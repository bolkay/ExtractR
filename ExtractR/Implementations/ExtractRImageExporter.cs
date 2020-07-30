using ExtractR.Interfaces;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ExtractR.Implementations
{
    /// <summary>
    /// Basically moves items from the temp working path to the gallery or other destination.
    /// </summary>
    public class ExtractRImageExporter : IExtractRImageExporter
    {
        public bool ExportImages(string destinationDirectory, string fromDirectory)
        {
            try
            {
                var files = Directory.EnumerateFiles(fromDirectory);

                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        string destFileName = Path.Combine(destinationDirectory, Path.GetFileName(file));

                        File.Move(file, destFileName);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts one or more pages of a PDF document to image.
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public bool ConvertPdfToImage(string pdfPath, Stream inputStream = null)
        {
            if (string.IsNullOrEmpty(pdfPath) && null == inputStream)
                throw new Exception($"Either {nameof(pdfPath)} or {nameof(inputStream)} must be supplied.");

            PdfReader pdfReader = null;
            if (!string.IsNullOrEmpty(pdfPath))
                pdfReader = new PdfReader(inputStream);
            else
                pdfReader = new PdfReader(pdfPath);

            PdfDocument pdfDocument = new PdfDocument(pdfReader);

            Document document = new Document(pdfDocument);

            for (int p = 0; p <= pdfDocument.GetNumberOfPages(); p++)
            {
                //Get the page.
                var page = pdfDocument.GetPage(p);
                PdfFormXObject pdfFormXObject = page.CopyAsFormXObject(pdfDocument);

                Image image = new Image(pdfFormXObject);

            }

            return true;
        }
    }
}
