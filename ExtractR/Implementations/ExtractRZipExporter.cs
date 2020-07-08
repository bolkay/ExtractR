using ExtractR.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
namespace ExtractR.Implementations
{
    /// <summary>
    /// Responsible for exporting zip files.
    /// </summary>
    public class ExtractRZipExporter : IExtractRZipExporter
    {
        public ExtractRZipExporter(string to, string dirFrom)
        {
            To = to;
            DirFrom = dirFrom;
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtractRZipExporter()
        {

        }

        private string To { get; }
        private string DirFrom { get; }

        public bool ExportZip(string to, string fromDir)
        {

            try
            {

                ZipFile.CreateFromDirectory(fromDir, to, CompressionLevel.Optimal, false);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ExportZip()
        {
            if (string.IsNullOrEmpty(To) || string.IsNullOrEmpty(DirFrom))
                return false;

            return ExportZip(To, DirFrom);
        }
    }
}
