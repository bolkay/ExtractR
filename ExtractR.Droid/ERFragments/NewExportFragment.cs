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
namespace ExtractR.Droid.ERFragments
{
    public class NewExportFragment : Android.Support.V4.App.Fragment
    {

        private readonly ExportActivity exportActivity;
        Google.Android.Material.Button.MaterialButton zipButton;
        Google.Android.Material.Button.MaterialButton pdfButton;
        Google.Android.Material.Button.MaterialButton imageButton;
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

            zipButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.zipButton);

            imageButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.imageExportButton);

            zipButton.Click += ZipButton_Click;

            pdfButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.pdfButton);

            pdfButton.Click += PdfButton_Click;

            imageButton.Click += ImageButton_Click;
            return view;
        }

        private bool CanSave()
        {
            return System.IO.Directory.GetFiles(PathHelper.GetOrCreateExtractRTempDirectory()).Any();
        }
        private async void ImageButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                exportActivity.RunOnUiThread(() =>
                {
                    if (exportProgressBar.Visibility == ViewStates.Gone)
                        exportProgressBar.Visibility = ViewStates.Visible;
                });

                //Export directory
                string exportDirectory = PathHelper.GetOrCreateMainExtractRDirectory();

                if (!string.IsNullOrEmpty(exportDirectory) && CanSave())
                {
                    var trySave = ExportImages
                    (exportDirectory,
                        PathHelper.ExtractRTempDirectory);

                    if (trySave)
                    {
                        exportActivity.RunOnUiThread(() =>
                        {
                            exportProgressBar.Visibility = ViewStates.Gone;

                            ShowSuccess("We exported your images. You can find them in gallery.");
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
                else
                {
                    exportActivity.RunOnUiThread(() =>
                    {
                        exportProgressBar.Visibility = ViewStates.Gone;

                        ShowFailureToSave();
                    });
                }
            });
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

                if (!string.IsNullOrEmpty(exportDirectory) && CanSave())
                {
                    var trySave = ExportPDF(System.IO.Path.Combine(exportDirectory, Guid.NewGuid().ToString() + ".pdf"),
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
                else
                {
                    exportActivity.RunOnUiThread(() =>
                    {
                        exportProgressBar.Visibility = ViewStates.Gone;

                        ShowFailureToSave();
                    });
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

                if (directory != null && CanSave())
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
                else
                {
                    exportActivity.RunOnUiThread(() =>
                    {
                        exportProgressBar.Visibility = ViewStates.Gone;
                        ShowFailureToSave();
                    });
                }

            });
        }
        private void ShowFailureToSave()
        {
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this.Context);

            builder.SetTitle("Export Failed")
                .SetMessage("ExtractR is sadly unable to save your file right now. Please try again.")
                .SetIcon(Context.GetDrawable(Resource.Drawable.abc_ab_share_pack_mtrl_alpha))
                .SetCancelable(false)
                .SetNeutralButton("Let me try again", (s, e) => { return; })
                .Show();
        }
        public void ShowSuccess(string message = null)
        {
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this.Context);

            builder.SetTitle("Success")
                .SetMessage(message ?? "We exported your file successfully. Please go to the history tab to have a look.")
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
        public bool ExportImages(string dir, string fromDir)
        {
            try
            {
                ExtractRImageExporter extractRImageExporter = new ExtractRImageExporter();
                bool tryExportImages = extractRImageExporter.ExportImages(dir, fromDir);

                return tryExportImages;
            }
            catch
            {
                return false;
            }
        }
    }
}