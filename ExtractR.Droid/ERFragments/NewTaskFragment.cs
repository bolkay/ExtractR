using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Animation;
using Android.Support.Design.Card;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExtractR.Core;
using ExtractR.Droid.Activities;
using ExtractR.Droid.Adapters;
using ExtractR.Droid.Helpers;
using ExtractR.Droid.Models;
using ExtractR.Implementations;
using Java.IO;
using Java.Lang;

namespace ExtractR.Droid.ERFragments
{
    public class NewTaskFragment : Android.Support.V4.App.Fragment
    {
        public List<ImageFileNameModel> ImageFileNameModels = new List<ImageFileNameModel>();
        public bool CouldBeRefreshed = false; //The workspace could be refreshed.

        public AndroidX.RecyclerView.Widget.RecyclerView _recyclerView;
        MainActivity mainActivity;
        AndroidX.RecyclerView.Widget.LinearLayoutManager LinearLayoutManager;
        public NewTaskFragment(AppCompatActivity context)
        {
            this.context = context;
        }

        public static int FileRequestCode = 1;

        Button fileChooserBtn;

        private readonly AppCompatActivity context;

        public CardView fileChooserCard;
        private const string FILE_CHOOSER_VISIBILTY = "FILE_CHOOSER_VISIBILITY";
        public CardView processingGroup;
        public TextView processingTextView;
        private ProgressBar processingProgressBar;

        private Bundle SavedState = null;
        private bool SelectButtonIsVisible = true;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            mainActivity = context as MainActivity;

        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            SavedState = outState;
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            //Save the visibilty of the fileChooser group since it is not handled by default.
            if (fileChooserCard != null && SavedState != null)
            {
                //Saved state has some stuff.
                var shouldbeVisible = SavedState.GetBoolean(FILE_CHOOSER_VISIBILTY);
                if (!shouldbeVisible)
                {
                    fileChooserCard.Visibility = ViewStates.Gone;
                }

                if (!ImageFileNameModels.Any())
                    fileChooserCard.Visibility = ViewStates.Gone;
            }

            //Show menu
            if (mainActivity != null)
            {
                if (mainActivity.menu != null)
                    mainActivity.menu.FindItem(Resource.Id.action_refresh).SetVisible(true);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.newtask_fragment, container, false);

            fileChooserCard = view.FindViewById<CardView>(Resource.Id.fileChooserGroup);

            fileChooserBtn = view.FindViewById<Button>(Resource.Id.fileChooserButton);

            processingGroup = view.FindViewById<CardView>(Resource.Id.extractRProcessingGroup);

            processingTextView = view.FindViewById<TextView>(Resource.Id.processingTextView);

            processingProgressBar = view.FindViewById<ProgressBar>(Resource.Id.processingBar);

            _recyclerView = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.imageRecyclerView);

            LinearLayoutManager = new AndroidX.RecyclerView.Widget.LinearLayoutManager(this.Context);

            _recyclerView.SetLayoutManager(LinearLayoutManager);

            _recyclerView.NestedScrollingEnabled = false;

            _recyclerView.HasFixedSize = true;

            var decoration = new AndroidX.RecyclerView.Widget.DividerItemDecoration(this.Context, LinearLayoutManager.Orientation);
            int pixelMargin = Context.Resources.GetDimensionPixelSize(Resource.Dimension.divider_margin_vertical);
            decoration.Drawable = (new InsetDrawable
                (Context.GetDrawable(Resource.Drawable.abc_list_divider_material), 0, pixelMargin, 0, pixelMargin));

            _recyclerView.AddItemDecoration(decoration);

            FoundImagesAdapter foundImagesAdapter = new FoundImagesAdapter(ImageFileNameModels, this);

            foundImagesAdapter.ItemClick += FoundImagesAdapter_ItemClick;

            _recyclerView.SetAdapter(foundImagesAdapter);


            AndroidX.RecyclerView.Widget.ItemTouchHelper itemTouchHelper = new AndroidX.RecyclerView.Widget.ItemTouchHelper(new RVItemTouchCallback(0, ItemTouchHelper.Right | ItemTouchHelper.Left,
                _recyclerView, this, ImageFileNameModels, mainActivity));

            itemTouchHelper.AttachToRecyclerView(_recyclerView);

            fileChooserBtn.Click += FileChooserBtn_Click;

            return view;
        }

        private void FoundImagesAdapter_ItemClick(object sender, FoundImagesAdapterClickEventArgs e)
        {
            var item = ImageFileNameModels.ElementAt(e.Position);

            //Send the full path in intent to the image activity.
            Intent intent = new Intent(context, typeof(ImageDetailActivity))
                .PutExtra("filePath", item.FullPath);

            context.StartActivity(intent);
        }

        private void FileChooserBtn_Click(object sender, EventArgs e)
        {
            Intent fileIntent = new Intent(Intent.ActionOpenDocument);
            fileIntent.SetType("application/pdf");

            StartActivityForResult(fileIntent, FileRequestCode);
        }

        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (null == data)
            {
                SelectButtonIsVisible = true;

                SavedState.PutBoolean(FILE_CHOOSER_VISIBILTY, SelectButtonIsVisible);
            }

            if (requestCode == FileRequestCode && resultCode == (int)Result.Ok)
            {

                //Before performing a new task, clean up the temp.
                PathHelper.DeleteAllTempFiles();

                //Save the filename to memory.
                PathHelper.OriginalPDFName = data.Data.GetFileNameFromURIWithoutExtension();
                await Task.Run(() =>
                {
                    try

                    {
                        context.RunOnUiThread(() =>
                        {
                            //Turn off the fileChooser
                            fileChooserCard.Visibility = ViewStates.Gone;

                            SelectButtonIsVisible = false;

                            SavedState.PutBoolean(FILE_CHOOSER_VISIBILTY, SelectButtonIsVisible);
                        });

                        //Try extract images from the supplied pdf.
                        //Create the temp folder.

                        string directory = PathHelper.GetOrCreateExtractRTempDirectory();

                        //Build the save Path.
                        string savePath = string.Empty;

                        if (!string.IsNullOrEmpty(directory))
                        {
                            savePath = System.IO.Path.Combine(directory, PathHelper.OriginalPDFName);
                        }

                        var stream = context.ContentResolver.OpenInputStream(data.Data);

                        ImageExtractor imageExtractor = new ImageExtractor(stream);

                        imageExtractor.ImageSaved += (s, e) =>
                        {
                            context.RunOnUiThread(() =>
                            {
                                //Try updating the progressbar.
                                processingProgressBar.SetProgress(e.Progress, true);

                            });
                        };

                        if (imageExtractor != null)
                        {
                            context.RunOnUiThread(() =>
                            {
                                //Show the processing group.
                                if (processingGroup.Visibility == ViewStates.Gone)
                                    processingGroup.Visibility = ViewStates.Visible;

                                //Set the text in the processing textview.
                                processingTextView.Text = $"ExtractR is scanning your document...";

                            });

                            //Extract data from the pdf.
                            var elements = imageExtractor.ExtractElementsData();

                            context.RunOnUiThread(() =>
                            {
                                if (elements.Result.Count == 0)
                                {
                                    //No need to waste time. Break out of the function.
                                    ReportNothingFound();
                                    return;
                                }
                                //Update the text in the processing textview again to reflect the total elements found.
                                processingTextView.Text = $"Processing {elements.Result.Count} possible images...";

                                processingProgressBar.SetProgress(0, true);
                                processingProgressBar.Max = elements.Result.Count;
                            });

                            if (elements != null)
                            {
                                var savedImagePaths = imageExtractor.ProcessData(elements.Result, savePath).Result;

                                //Remove all current items.
                                ImageFileNameModels.Clear();

                                foreach (var path in savedImagePaths)
                                {
                                    System.Diagnostics.Debug.WriteLine(path);

                                    ImageFileNameModels.Add(new ImageFileNameModel
                                    {
                                        FileName = path,
                                        FullPath = System.IO.Path.Combine(PathHelper.ExtractRDirectory, path)
                                    }); ;
                                }
                            }

                            context.RunOnUiThread(() =>
                            {
                                processingGroup.Visibility = ViewStates.Gone;

                                var count = ImageFileNameModels.Count;
                                mainActivity.toolbar.Subtitle = count > 0 ? $"Found {count} possible images." : "No image found.";
                                ItemCountHelper.UpdateExportItemsCount(mainActivity, ImageFileNameModels);
                                _recyclerView.GetAdapter().NotifyDataSetChanged();
                            });

                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                });
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
        private void ReportNothingFound()
        {
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this.Context);
            builder.SetTitle("No image found")
                .SetMessage("Unfortunately, EtxractR couldn't find any image to process. Try again with another file, perhaps?")
                .SetIcon(this.Context.GetDrawable(Resource.Drawable.notification_template_icon_bg))
                .SetNeutralButton("Understood", (s, e) =>
                {
                    //Call refresh
                    mainActivity.RefreshTaskFragment(this);
                })
                .SetCancelable(false)
                .Show();

            CouldBeRefreshed = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}