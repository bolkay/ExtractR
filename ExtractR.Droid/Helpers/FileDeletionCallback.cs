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
    public class FileDeletionCallback:BaseTransientBottomBar.BaseCallback
    {
        private readonly bool shouldDelete;
        private readonly string filePath;

        public FileDeletionCallback(bool shouldDelete, string filePath)
        {
            this.shouldDelete = shouldDelete;
            this.filePath = filePath;
        }
        public override void OnDismissed(Java.Lang.Object transientBottomBar, int e)
        {
            base.OnDismissed(transientBottomBar, e);

            if (shouldDelete)
                System.IO.File.Delete(filePath);
        }
    }
}