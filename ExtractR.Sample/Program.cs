using ExtractR.Financials.Core;
using ExtractR.Financials.Implementations;
using ExtractR.Financials.Models;
using ExtractR.Implementations;
using ExtractR.Mailer;
using System;
using System.Diagnostics;
using System.IO;

namespace ExtractR.Sample
{
    class Program
    {
        //pdf path.
        private static string ImagesDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Screenshots");
        private const string SourceFile = @"C:\Users\KTBolarinwa\Downloads\Essentials\Essentials\Attachments\Filled Application Form- Bolarinwa Kayode.pdf";
        static void Main(string[] args)
        {

            IMailSender mailSender = new MailSender();
            bool sendMessage = mailSender.TrySendSimpleMail("Just testing here in sample", "Hello for testing", "bolkay10@gmail.com", "ktbolarinwa@gmail.com",
                true).Result;
            if (sendMessage)
                Console.WriteLine("Message sent successfully.");

            Console.ReadKey();
        }

        private static void Test_Paystack()
        {
            Console.WriteLine("Creating objects.....");
            Paystack paystack = new Paystack(Environment.GetEnvironmentVariable("paystack_secret"));
            AuthorisationDetails authorisationDetails = new PaystackAuthorisationDetails
            {
                Amount = "150000",
                Channels = new string[] { "bank", "card" },
                Email = "ktbolarinwa@gmail.com"
            };

            Console.WriteLine("Getting Authorisation Url.....");

            var getUrl = paystack.AuthoriseAsync(authorisationDetails).Result;

            if (!string.IsNullOrEmpty(getUrl.AuthEndpoint))
                Process.Start(new ProcessStartInfo { UseShellExecute = true, Verb = "open", FileName = getUrl.AuthEndpoint });
            else
                Console.WriteLine("Model returned null....");
        }

        private static void SaveImagesInPDF()
        {
            //Save images as pdf file.
            Stopwatch stopwatch = new Stopwatch();
            Console.WriteLine("Starting....");
            stopwatch.Start();
            ExtractRPDFExporter pDFExporter = new ExtractRPDFExporter();

            var tryExport = pDFExporter.ExportPDF(ImagesDir, Path.Combine(ImagesDir, "result.pdf"));
            stopwatch.Stop();

            Console.WriteLine("Elapsed in ms: " + stopwatch.ElapsedMilliseconds);
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
