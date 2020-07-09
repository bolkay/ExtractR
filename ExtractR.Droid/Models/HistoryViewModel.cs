using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Helpers;

namespace ExtractR.Droid.Models
{
    public class HistoryViewModel
    {
        public string FileSizeAndDayModified => $"Size on disk : {PathHelper.GetFileLength(FileName)} Kb";
        public string FileName { get; set; }

    }
}