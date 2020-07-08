using ExtractR.Droid.Helpers;
using System.Threading.Tasks;

namespace ExtractR.Droid.Models
{
    public class ImageFileNameModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public string FileSize => $"File Size: {PathHelper.GetFileLength(FullPath)} Kb";
    }
}