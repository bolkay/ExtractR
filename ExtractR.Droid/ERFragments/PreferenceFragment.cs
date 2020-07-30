using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ExtractR.Droid.Activities;
using ExtractR.Droid.Helpers;
using ExtractR.Financials.Results;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Newtonsoft.Json;

namespace ExtractR.Droid.ERFragments
{
    public class PreferenceFragment : Android.Support.V4.App.Fragment, CompoundButton.IOnCheckedChangeListener
    {

        public PreferenceFragment()
        {

        }

        public PreferenceFragment(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        MaterialCardView materialCard;
        TextView donationTextView;
        Button donationRefreshButton;
        Switch themeSwitcher;
        Google.Android.Material.Button.MaterialButton aboutButton;
        Google.Android.Material.Button.MaterialButton reportButton;

        ISharedPreferences sharedPreferences;
        private readonly MainActivity mainActivity;

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {

            if (sharedPreferences != null)
            {
                var editor = sharedPreferences.Edit();

                if (isChecked)
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                    editor.PutBoolean(ERThemeManager.ThemeKey, true);
                    themeSwitcher.Text = "Toggle to Turn off the Lights";
                }
                else
                {
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                    editor.PutBoolean(ERThemeManager.ThemeKey, false);
                    themeSwitcher.Text = "Switch on Light Mode";
                }

                editor.PutBoolean("Saved_Switch", isChecked);
                editor.Commit();
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view = inflater.Inflate(Resource.Layout.preference_fragment, container, false);
            themeSwitcher = view.FindViewById<Switch>(Resource.Id.switcher);
            materialCard = view.FindViewById<MaterialCardView>(Resource.Id.donationCard);
            donationTextView = view.FindViewById<TextView>(Resource.Id.donationTextView);
            donationRefreshButton = view.FindViewById<Button>(Resource.Id.donationRefreshButton);
            aboutButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.aboutButton);
            reportButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.reportButton);

            donationRefreshButton.Click += DonationRefreshButton_Click;
            aboutButton.Click += AboutButton_Click;
            reportButton.Click += ReportButton_Click;
            themeSwitcher.SetOnCheckedChangeListener(this);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.Context);

            var stateIsChecked = sharedPreferences.GetBoolean("Saved_Switch", false);

            if (themeSwitcher != null)
            {
                themeSwitcher.Checked = stateIsChecked;

                if (stateIsChecked)
                    themeSwitcher.Text = "Toggle to turn off the lights";
                else
                    themeSwitcher.Text = "Toogle to turn the lights on";
            }
            var userHasDonated = ExtractRAdManager.UserHasDonated(this.Context);

            if (!userHasDonated)
            {
                materialCard.SetBackgroundColor(Android.Graphics.Color.DarkRed);
                donationTextView.SetTextColor(Android.Graphics.Color.GhostWhite);
                donationTextView.Text = Context.GetString(Resource.String.user_no_donation);
                donationRefreshButton.Visibility = ViewStates.Visible;
            }
            else
            {
                materialCard.SetBackgroundColor(Android.Graphics.Color.DarkGreen);
                donationTextView.SetTextColor(Android.Graphics.Color.GhostWhite);
                donationTextView.Text = Context.GetString(Resource.String.user_donated);
                donationRefreshButton.Visibility = ViewStates.Gone;
            }

            return view;
        }

        private bool NightModeIsOn()
        {
            return AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightYes;
        }
        private void ReportButton_Click(object sender, EventArgs e)
        {
            Google.Android.Material.Button.MaterialButton submitButton;
            Google.Android.Material.Button.MaterialButton closeButton;
            Spinner reportSpinner;
            TextInputEditText subjectInput;
            TextInputEditText messageInput;
            View view = LayoutInflater.Inflate(Resource.Layout.report_sender_layout, null);
            closeButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.closeReportWindow);
            submitButton = view.FindViewById<Google.Android.Material.Button.MaterialButton>(Resource.Id.reportSubmit);
            subjectInput = view.FindViewById<TextInputEditText>(Resource.Id.reportSubject);
            messageInput = view.FindViewById<TextInputEditText>(Resource.Id.reportMessage);
            reportSpinner = view.FindViewById<Spinner>(Resource.Id.reportSpinner);

            if (NightModeIsOn())
                closeButton.SetTextColor(Android.Graphics.Color.GhostWhite);
            else
            {
                string accentColor = "#" + Context.GetColor(Resource.Color.colorAccent).ToString("X");
                closeButton.SetTextColor(Android.Graphics.Color.ParseColor(accentColor));
            }

            ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this.Context, Resource.Layout.support_simple_spinner_dropdown_item,
                new string[] { "Feature Request", "Problem with App" });

            reportSpinner.Adapter = arrayAdapter;

            PopupWindow popupWindow = new PopupWindow(view, LinearLayoutCompat.MarginLayoutParams.MatchParent
                , LinearLayoutCompat.MarginLayoutParams.WrapContent);

            closeButton.Click += (s, e) =>
            {
                GetDialogBuilder()
                .SetTitle("Dismiss Report Form?")
                .SetMessage("I would love to this at a later time. Not now.")
                .SetPositiveButton("Yes", (s, e) => { popupWindow.Dismiss(); })
                .SetNegativeButton("No", (s, e) => { return; })
                .SetCancelable(false)
                .Show();
            };

            submitButton.Click += (s, e) =>
            {

            };
            popupWindow.InputMethodMode = InputMethod.Needed;
            popupWindow.Focusable = true;
            popupWindow.ShowAtLocation(this.View, GravityFlags.Center, 0, 0);


        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            GetDialogBuilder()
                .SetTitle("About ExtractR")
                .SetMessage(Context.GetString(Resource.String.about_extractr))
                .SetCancelable(false)
                .SetNeutralButton("Ok", (s, e) => { return; })
                .Show();
        }

        private async void DonationRefreshButton_Click(object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    mainActivity.RunOnUiThread(() =>
                    {
                        donationRefreshButton.Text = "Verifying....";
                    });
                    //Try to refresh the user's transaction.
                    string file = System.IO.File.ReadAllText(System.IO.Path.Combine(PathHelper.GetOrCreateAuthDetailsPath(), "auth.exr"));
                    string json = Encoding.UTF8.GetString(Convert.FromBase64String(file));

                    ProviderAuthorisationResult providerAuthorisationResult =
                        JsonConvert.DeserializeObject<ProviderAuthorisationResult>(json);

                    if (providerAuthorisationResult.Status.ToLower() == "ok")
                    {

                        mainActivity.RunOnUiThread(() =>
                        {
                            GetDialogBuilder()
                            .SetTitle("Transaction Successfully Verified")
                            .SetMessage("Congratulations! We have successfully verified your transaction. " +
                            "You should restart the application for the changes to take effect. " +
                            "Sorry for the inconvenience.")
                            .SetCancelable(false)
                            .SetNeutralButton("OK", (s, e) => { return; })
                            .Show();

                            donationRefreshButton.Text = "I have Donated";
                        });

                        ExtractRAdManager.SetUserHasDonated(this.Context);
                    }
                    else
                    {
                        mainActivity.RunOnUiThread(() =>
                        {
                            GetDialogBuilder()
                            .SetTitle("Transaction Failed")
                            .SetMessage("We are unable to verify your transaction. It looks like your transaction was not successful.")
                            .SetCancelable(false)
                            .SetNeutralButton("OK", (s, e) => { return; })
                            .Show();
                            donationRefreshButton.Text = "I have Donated";
                        });
                    }
                }
                catch
                {
                    mainActivity.RunOnUiThread(() =>
                    {
                        GetDialogBuilder()
                        .SetTitle("An Error Occured")
                        .SetMessage("We are unable to validate the transaction right now. Please try again later.")
                        .SetCancelable(false)
                        .SetNeutralButton("OK", (s, e) => { return; })
                        .Show();


                        donationRefreshButton.Text = "I have Donated";
                    });
                }
            });
        }
        public AndroidX.AppCompat.App.AlertDialog.Builder GetDialogBuilder()
        {
            return new AndroidX.AppCompat.App.AlertDialog.Builder(this.Context);
        }
    }
}