using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExtractR.Droid.Helpers;
using ExtractR.Implementations;
using ExtractR.Interfaces;
using Org.BouncyCastle.Ocsp;

namespace ExtractR.Droid.ERFragments
{
    public class NewExportFragment : Android.Support.V4.App.Fragment
    {
        private const int FOLDER_REQUEST_CODE = 1;
        private readonly MainActivity mainActivity;
        Button zipButton;
        ProgressBar exportProgressBar;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public NewExportFragment(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.new_export_fragment, container, false);

            exportProgressBar = view.FindViewById<ProgressBar>(Resource.Id.exportProgressBar);

            zipButton = view.FindViewById<Button>(Resource.Id.zipButton);
            zipButton.Click += ZipButton_Click;
            return view;
        }

        private async void ZipButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                mainActivity.RunOnUiThread(() =>
                {
                    if (exportProgressBar.Visibility == ViewStates.Gone)
                        exportProgressBar.Visibility = ViewStates.Visible;
                });

                string directory = PathHelper.GetOrCreateExtractRExportDirectory();

                if (directory != null)
                {
                    //The fileNames are guaranteed to be different.
                    string saveDir = System.IO.Path.Combine(directory, PathHelper.OriginalPDFName + Guid.NewGuid().ToString() + ".zip");
                    bool tryExport = ExportZipFormat(saveDir, PathHelper.ExtractRTempDirectory);

                    if (tryExport)
                    {
                        mainActivity.RunOnUiThread(() =>
                        {
                            exportProgressBar.Visibility = ViewStates.Gone;

                            ShowSuccess();
                        });
                    }
                    else
                    {
                        mainActivity.RunOnUiThread(() =>
                        {
                            exportProgressBar.Visibility = ViewStates.Gone;
                            ShowFailureToSave();
                        });
                    }

                }

            });
        }
        private void ShowFailureToSave()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Context);

            builder.SetTitle("Export Failed")
                .SetMessage("ExtractR is sadly unable to save your file right now. Please try again.")
                .SetIcon(Context.GetDrawable(Resource.Drawable.abc_ab_share_pack_mtrl_alpha))
                .Show();
        }
        public void ShowSuccess()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Context);

            builder.SetTitle("Work Exported")
                .SetMessage("We exported your file successfully.")
                .SetIcon(Context.GetDrawable(Resource.Drawable.abc_ab_share_pack_mtrl_alpha))
                .Show();
        }
        public bool ExportZipFormat(string toDirectory, string fromDirectory)
        {
            IExtractRZipExporter extractRZipExporter = new ExtractRZipExporter(toDirectory, fromDirectory);

            return extractRZipExporter.ExportZip();
        }
    }
}