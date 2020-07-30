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

namespace ExtractR.Droid.Helpers
{
    public class UserActionHelper
    {
        public static void StartAction(string actionType, string filePath, Context context)
        {

            if (!string.IsNullOrEmpty(filePath))
            {
                Android.Net.Uri uriToShare = AndroidX.Core.Content.FileProvider.GetUriForFile(context,
                    "com.dotnetflow.extractr.droid.fileprovider",
                    new Java.IO.File(filePath));

                Intent intent = new Intent(actionType);

                intent.PutExtra(Intent.ExtraText, $"Sharing : { filePath.GetFileNameWithoutExtension()}");

                if (filePath.EndsWith("zip"))
                    intent.SetType("application/zip");
                else
                    intent.SetType("application/pdf");

                if (actionType == Intent.ActionSend)
                    intent.PutExtra(Intent.ExtraStream, uriToShare);

                if (actionType == Intent.ActionView)
                    intent.SetData(uriToShare);

                intent.SetFlags(ActivityFlags.GrantReadUriPermission);

                context.StartActivity(Intent.CreateChooser(intent, "EXTRACTR"));
            }
        }
    }
}