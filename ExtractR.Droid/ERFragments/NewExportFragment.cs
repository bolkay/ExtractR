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
using ExtractR.Droid.Activities;
using ExtractR.Droid.Helpers;
using ExtractR.Implementations;
using ExtractR.Interfaces;
using Java.IO;
using Org.BouncyCastle.Ocsp;

namespace ExtractR.Droid.ERFragments
{
    public class NewExportFragment : Android.Support.V4.App.Fragment
    {

        private readonly ExportActivity exportActivity;
        Button zipButton;
        Button pdfButton;
        ProgressBar exportProgressBar;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public NewExportFragment(ExportActivity exportActivity)
        {
            this.exportActivity = exportActivity;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.new_export_fragment, container, false);

            exportProgressBar = view.FindViewById<ProgressBar>(Resource.Id.exportProgressBar);

            zipButton = view.FindViewById<Button>(Resource.Id.zipButton);

            zipButton.Click += ZipButton_Click;

            pdfButton = view.FindViewById<Button>(Resource.Id.pdfButton);

            pdfButton.Click += PdfButton_Click;
            return view;
        }

        private async void PdfButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                exportActivity.RunOnUiThread(() =>
                {
                    if (exportProgressBar.Visibility == ViewStates.Gone)
                        exportProgressBar.Visibility = ViewStates.Visible;
                });

                //Export directory
                string exportDirectory = PathHelper.GetOrCreateExtractRExportDirectory();

                if (!string.IsNullOrEmpty(exportDirectory))
                {
                    var trySave = ExportPDF(System.IO.Path.Combine(exportDirectory, PathHelper.OriginalPDFName + Guid.NewGuid().ToString() + ".pdf"),
                        PathHelper.ExtractRTempDirectory);

                    if (trySave)
                    {
                        exportActivity.RunOnUiThread(() =>
                        {
                            exportProgressBar.Visibility = ViewStates.Gone;

                            ShowSuccess();
                        });

                    }

                    else
                    {
                        exportActivity.RunOnUiThread(() =>
                        {

                            exportProgressBar.Visibility = ViewStates.Gone;

                            ShowFailureToSave();
                        });
                    }
                }
            });
        }

        private async void ZipButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                exportActivity.RunOnUiThread(() =>
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
                        exportActivity.RunOnUiThread(() =>
                        {
                            exportProgressBar.Visibility = ViewStates.Gone;

                            ShowSuccess();

                        });
                    }
                    else
                    {
                        exportActivity.RunOnUiThread(() =>
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
                .SetCancelable(false)
                .SetNeutralButton("Let me try again", (s, e) => { return; })
                .Show();
        }
        public void ShowSuccess()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Context);

            builder.SetTitle("Success")
                .SetMessage("We exported your file successfully. Please go to the history tab to have a look.")
                .SetIcon(Context.GetDrawable(Resource.Drawable.abc_ab_share_pack_mtrl_alpha))
                .SetCancelable(false)
                .SetNeutralButton("Okay", (s, e) => { return; })
                .Show();
        }
        public bool ExportZipFormat(string toDirectory, string fromDirectory)
        {
            IExtractRZipExporter extractRZipExporter = new ExtractRZipExporter(toDirectory, fromDirectory);

            return extractRZipExporter.ExportZip();
        }

        public bool ExportPDF(string exportPath, string fromDirectory)
        {
            try
            {
                ExtractRPDFExporter pDFExporter = new ExtractRPDFExporter();
                var trySave = pDFExporter.ExportPDF(fromDirectory, exportPath);

                return trySave;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }
}