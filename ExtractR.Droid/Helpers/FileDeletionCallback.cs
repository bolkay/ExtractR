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
using Java.IO;

namespace ExtractR.Droid.Helpers
{
    public class FileDeletionCallback : BaseTransientBottomBar.BaseCallback
    {

        private readonly string filePath;

        public FileDeletionCallback(string filePath)
        {
            this.filePath = filePath;
        }
        public override void OnDismissed(Java.Lang.Object transientBottomBar, int e)
        {
            if (PermissionHelper.ShouldDelete)
                System.IO.File.Delete(filePath);

            base.OnDismissed(transientBottomBar, e);
        }
    }
}