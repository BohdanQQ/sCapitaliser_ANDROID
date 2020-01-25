using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Xamarin.Essentials;
using Android.Widget;
using System.Text;
using Android.Support.V4.App;
using Android.Content;

namespace sCapitaliser_ANDROID
{
    [BroadcastReceiver]
    [Activity(Label = "sCapitaliser", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText input_text;
        CheckBox second_letter;
        public readonly string CHANNEL_ID = "sCapitaliser";
        static readonly int NOTIFICATION_ID = 9517;
        Notification notification = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            input_text = (EditText)FindViewById(Resource.Id.input_text);
            second_letter = (CheckBox)FindViewById(Resource.Id.second_letter);

            ((Button)FindViewById(Resource.Id.convertButton)).Click += convertButon_click;

            CreateNotificationChannel();
        }
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = "sCapitaliser notifications";
            var description = "Used to make quick edgy content!";
            var channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };
            channel.EnableVibration(false);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            bool isActive = false;
            foreach (var noti in notificationManager.GetActiveNotifications()){
                if(noti.Id == NOTIFICATION_ID)
                {
                    isActive = true;
                    break;
                }
            }
            if (id == Resource.Id.action_settings && !isActive)
            {

                // When the user clicks the notification, SecondActivity will start up.
                var resultIntent = new Android.Content.Intent(this, typeof(MainActivity));

                // Construct a back stack for cross-task navigation:
                var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
                stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
                stackBuilder.AddNextIntent(resultIntent);

                // Create the PendingIntent with the back stack:
                var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

                // Build the notification:
                var builder = new NotificationCompat.Builder(this, CHANNEL_ID)
                              .SetAutoCancel(false) // Dismiss the notification from the notification area when the user clicks on it
                              .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                              .SetContentTitle("sCapitaliser") // Set the title
                              .SetSmallIcon(Resource.Drawable.ic_mini) // This is the icon to display
                              .SetContentText("Tap to open sCapitaliser");

                // Finally, publish the notification:
                notification = builder.Build();
                notification.Flags = NotificationFlags.OngoingEvent;
                notificationManager.Notify(NOTIFICATION_ID, notification);
                notificationManager.Dispose();
                notification.Dispose();
                
            }
            else
            {
                notificationManager.Cancel(NOTIFICATION_ID);
            }
            notificationManager.Dispose();

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {

        }

        private async void convertButon_click(object sender, EventArgs eventArgs)
        {
            string text;
            if (input_text.Text.Trim().Length == 0)
            {
                text = Clipboard.HasText ? await Clipboard.GetTextAsync() : "";
            }
            else
            {
                text = input_text.Text;
            }

            bool capital = !second_letter.Checked;
            CreateResult(capital, text, BaseContext);
        }
        public static async void CreateResult(bool capital, string text, Context BaseContext)
        {
            if(text.Trim() == "")
            {
                Toast.MakeText(BaseContext, "Invalid input or clipboard contents!", ToastLength.Short).Show();
                return;
            }
            StringBuilder final = new StringBuilder(text.Length);
            foreach (var letter in text)
            {
                if (char.IsLetter(letter))
                {
                    final.Append(capital ? char.ToUpper(letter) : char.ToLower(letter));
                    capital = !capital;
                }
                else
                {
                    final.Append(letter);
                }
            }
            if (final.Length > 0)
            {
                await Clipboard.SetTextAsync(final.ToString());
                Toast.MakeText(BaseContext, "Success", ToastLength.Short).Show();
            }
            else{
                Toast.MakeText(BaseContext, "Invalid clipboard contents!", ToastLength.Short).Show();
            }
            return;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

