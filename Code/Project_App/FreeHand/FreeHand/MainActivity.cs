using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Util;

namespace FreeHand
{
    [Activity(Label = "FreeHand", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private static readonly string TAG = "MainActivity";
        		 
        SMSBroadcastReceiver smsReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.myButton);

            //button.Click += delegate { button.Text = $"{count++} clicks!"; };
            TextView callSetting = FindViewById<TextView>(Resource.Id.call_setting);
            callSetting.Click += delegate {
                {
                    Intent callSettingIntent = new Intent(this,typeof(TestingTTSActivity));
                    StartActivity(callSettingIntent);
                }
            };
            smsReceiver = new SMSBroadcastReceiver();
            this.RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
            int count = 0;
            var imageButton = FindViewById<ImageButton>(Resource.Id.btn_power);
            var switchPower = FindViewById<Switch>(Resource.Id.power_switch);
            imageButton.Click += delegate {
                count++;
                if (count % 2 == 1) { // Start Application
                    imageButton.SetImageResource(Resource.Drawable.start);
                    switchPower.Toggle();
                    Toast.MakeText(this, "Application Started", ToastLength.Long).Show();
                }
                else { // Stop Application
                    imageButton.SetImageResource(Resource.Drawable.end);
                    switchPower.Toggle();
                    Toast.MakeText(this, "Application Stopped", ToastLength.Long).Show();
                }
                };

        }
		protected override void OnResume()
		{
			base.OnResume();
            Log.Debug(TAG,"OnResume");
		
			//RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
		}
		protected override void OnPause()
		{
			UnregisterReceiver(smsReceiver);
            Log.Debug(TAG, "OnPause");
			// Code omitted for clarity
			base.OnPause();
		}

	}
}

