using ExtractR.Core;
using ExtractR.Events;
using ExtractR.Listeners;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
namespace ExtractR.Implementations
{
    public class ImageExtractor : ExtractorBase
    {
        public event EventHandler<ImageSavedEventArgs> ImageSaved;

        public Dictionary<byte[], string> ImageDataAndExtension;
        public ImageExtractor(string sourcePath)
        {
            ImageDataAndExtension = new Dictionary<byte[], string>();
            SourcePath = sourcePath;
        }
        public ImageExtractor(Stream inputStream)
        {
            InputStream = inputStream;
            ImageDataAndExtension = new Dictionary<byte[], string>();
        }
        public ImageExtractor()
        {
        }
        private string SourcePath { get; }
        private Stream InputStream { get; }

        public override Task<Dictionary<byte[], string>> ExtractElementsData(string sourceFilePath = null)
        {
            try
            {
                //Create pdf reader.
                PdfReader pdfReader = null;
                if (InputStream == null)
                    pdfReader = new PdfReader(SourcePath ?? sourceFilePath);
                else
                    pdfReader = new PdfReader(InputStream);

                //PDF Doc
                PdfDocument pdfDocument = new PdfDocument(pdfReader);

                //Event listener.
                IEventListener eventListener = new CustomImageListener(ImageDataAndExtension);

                //Processor
                PdfCanvasProcessor pdfCanvasProcessor = new PdfCanvasProcessor(eventListener);

                for (int k = 1; k <= pdfDocument.GetNumberOfPages(); k++)
                {
                    //Process.
                    pdfCanvasProcessor.ProcessPageContent(pdfDocument.GetPage(k));
                }

                return Task.FromResult(ImageDataAndExtension);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);

                return null;
            }
        }

        public override Task<List<string>> ProcessData(Dictionary<byte[], string> keyValues, string savePath)
        {
            List<string> savedFiles = new List<string>();

            int count = 0;

            string tempSavePath = savePath;
            foreach (var data in keyValues.Keys)
            {
                count++;

                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    string currentSavePath = tempSavePath + $"{count}.png";

                    using (FileStream fileStream = new FileStream(currentSavePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        memoryStream.CopyTo(fileStream);
                    }

                    savedFiles.Add(currentSavePath);

                    OnImageSaved(this, new ImageSavedEventArgs { Progress = count, TotalElementsProcessed = keyValues.Count });

                    Console.WriteLine("Saved: " + currentSavePath);
                }

            }

            return Task.FromResult(savedFiles);
        }

        private void OnImageSaved(object sender, ImageSavedEventArgs imageFoundEventArgs)
        {
            ImageSaved?.Invoke(this, imageFoundEventArgs);
        }

        public void ConvertPDFToJpg(string pdfFile, string savePath)
        {

        }
    }
}
