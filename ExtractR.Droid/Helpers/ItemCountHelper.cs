using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Models;

namespace ExtractR.Droid.Helpers
{
    public static class ItemCountHelper
    {
        public static void UpdateExportItemsCount(MainActivity mainActivity, List<ImageFileNameModel> imageFileNameModels)
        {
            mainActivity.itemCountView.Text = imageFileNameModels.Count < 99 ? imageFileNameModels.Count.ToString() : "99+";
        }
    }
}