using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ExtractR.Droid.Helpers
{
    public static class PathHelper
    {

        /// <summary>
        /// Stores the orignally supplied file name.
        /// This could be potentially useful for saving the final export.
        /// </summary>
        public static string OriginalPDFName { get; set; }

        public static string ExtractGalleryPath =
            System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "ExtractR");

        public static string ExtractRDirectory = System.IO.Path.Combine(
            Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "ExtractR");

        public static string ExtractRExportDirectory = System.IO.Path.Combine(ExtractRDirectory, "Exports");

        public static string ExtractRTempDirectory = System.IO.Path.Combine(ExtractRDirectory, "temp");
        public static string GetFileNameFromURIWithoutExtension(this Android.Net.Uri uri)
        {
            return System.IO.Path.GetFileNameWithoutExtension(uri.Path);
        }

        public static string GetFileNameWithoutExtension(this string input)
        {
            return System.IO.Path.GetFileNameWithoutExtension(input);
        }
        public static bool TryGetDirectoryFromURI(Android.Net.Uri uri, out string result)
        {
            throw new NotImplementedException();

        }
        /// <summary>
        /// Returns the directory path if operation is successful regardless of whether the directory exists or not.
        /// </summary>
        /// <returns></returns>
        public static string GetOrCreateMainExtractRDirectory()
        {
            try
            {
                if (!System.IO.Directory.Exists(ExtractRDirectory))
                {
                    System.IO.Directory.CreateDirectory(ExtractRDirectory);

                }
                return ExtractRDirectory;
            }
            catch (Exception exception)
            {
                return null;
            }

        }
        public static string GetOrCreateExtractRExportDirectory()
        {
            try
            {
                if (!System.IO.Directory.Exists(ExtractRExportDirectory))
                    System.IO.Directory.CreateDirectory(ExtractRExportDirectory);

                return ExtractRExportDirectory;
            }
            catch
            {
                return null;
            }
        }
        public static string GetOrCreateExtractRTempDirectory()
        {
            try
            {
                if (!System.IO.Directory.Exists(ExtractRTempDirectory))
                    System.IO.Directory.CreateDirectory(ExtractRTempDirectory);

                return ExtractRTempDirectory;
            }
            catch
            {
                return null;
            }
        }

        public static string GetFileLength(string fullPath)
        {
            try
            {
                using (Java.IO.File file = new Java.IO.File(fullPath))
                {

                    long lengthInKB = file.Length() / 1000;

                    return (lengthInKB.ToString("0"));
                }
            }
            catch
            {
                return (null);
            }

        }

        public static void DeleteAllTempFiles()
        {
            try
            {
                //Try deleting all the temporary files.
                foreach (var file in System.IO.Directory.EnumerateFiles(GetOrCreateExtractRTempDirectory()))
                {
                    System.IO.File.Delete(file);
                }
            }
            catch
            {
                return;
            }
        }

        public static List<string> GetAllExportedFiles()
        {
            return System.IO.Directory.EnumerateFiles(ExtractRExportDirectory).ToList();
        }
    }
}