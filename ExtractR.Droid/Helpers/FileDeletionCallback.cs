using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.ERFragments;
using ExtractR.Droid.Models;
using Java.IO;

namespace ExtractR.Droid.Helpers
{
    public class FileDeletionCallback : BaseTransientBottomBar.BaseCallback
    {

        private readonly string filePath;
        private readonly HistoryFragment historyFragment;
        private readonly MainActivity mainActivity;
        private readonly List<ImageFileNameModel> imageFileNameModels;
        private readonly List<HistoryViewModel> historyViewModels;

        public FileDeletionCallback(string filePath, MainActivity mainActivity = null, List<ImageFileNameModel> imageFileNameModels = null)
        {
            this.filePath = filePath;
            this.mainActivity = mainActivity;
            this.imageFileNameModels = imageFileNameModels;
        }
        public FileDeletionCallback(string filePath, HistoryFragment historyFragment, List<HistoryViewModel> historyViewModels)
        {
            this.filePath = filePath;
            this.historyFragment = historyFragment;
            this.historyViewModels = historyViewModels;
        }
        public override void OnDismissed(Java.Lang.Object transientBottomBar, int e)
        {
            if (PermissionHelper.ShouldDelete)
            {
                System.IO.File.Delete(filePath);

                if (mainActivity != null && imageFileNameModels != null)
                    mainActivity.SupportActionBar.Subtitle = $"Current count - {imageFileNameModels.Count}";
            }

            if (historyViewModels != null)
            {
                if (historyViewModels.Count < 1)
                {
                    if (historyFragment.nohistory.Visibility == ViewStates.Gone)
                        historyFragment.nohistory.Visibility = ViewStates.Visible;
                }
            }
            base.OnDismissed(transientBottomBar, e);
        }
    }
}