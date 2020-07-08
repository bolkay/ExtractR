using ExtractR.Implementations;
using System;
using System.IO;

namespace ExtractR.Sample
{
    class Program
    {
        //pdf path.
        private const string SourceFile = @"C:\Users\KTBolarinwa\Downloads\Essentials\Essentials\Attachments\Filled Application Form- Bolarinwa Kayode.pdf";
        static void Main(string[] args)
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
            Console.ReadKey();
        }
    }
}
