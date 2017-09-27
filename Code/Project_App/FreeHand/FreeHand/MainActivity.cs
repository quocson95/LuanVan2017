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

