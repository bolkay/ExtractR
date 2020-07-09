using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ExtractR.Implementations
{
    /// <summary>
    /// Basically moves items from the temp working path to the gallery or other destination.
    /// </summary>
    public class ExtractRImageExporter
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
                        File.Move(file, Path.Combine(destinationDirectory, file));
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }
}
