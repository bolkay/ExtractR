﻿using ExtractR.Implementations;
using System;
using System.Diagnostics;
using System.IO;

namespace ExtractR.Sample
{
    class Program
    {
        //pdf path.
        private  static string ImagesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");
        private const string SourceFile = @"C:\Users\KTBolarinwa\Downloads\Essentials\Essentials\Attachments\Filled Application Form- Bolarinwa Kayode.pdf";
        static void Main(string[] args)
        {
            //Save images as pdf file.
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine("Starting....");
            stopwatch.Start();
            ExtractRPDFExporter pDFExporter = new ExtractRPDFExporter();

            var tryExport = pDFExporter.ExportPDF(ImagesDir, Path.Combine(ImagesDir, "result.pdf"));
            stopwatch.Stop();
            
            Console.WriteLine("Elapsed in ms: "+stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        private static void TestExtraction()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ExtractR");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string savePath = Path.Combine(dir,
                $"{Path.GetFileNameWithoutExtension(SourceFile)}");

            using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(SourceFile)))
            {
                ImageExtractor imageExtractor = new ImageExtractor(memoryStream);

                var imageDataDict = imageExtractor.ExtractElementsData().Result;

                var savedImages = imageExtractor.ProcessData(imageDataDict, savePath).Result;

                savedImages.ForEach(x => Console.WriteLine(x));
            }
        }
    }
}
