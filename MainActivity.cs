using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Threading.Tasks;
using FreeHand.Model;
using Android.Views;
using Android.Support.V7.App;
using Android.Graphics;

namespace FreeHand
{
    [Activity(Label = "MainActivity", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        private bool APP_RUNNIG;
        Intent MessengeServiceToStart;
        TextToSpeechLib _tts;
        private static readonly string TAG = "MainActivity";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main_layout);
            RequestPermission();
			StartReadConfig();
            _tts = TextToSpeechLib.Instance();
            _tts.SetMainContext(this);
            MessengeServiceToStart = new Intent(this, typeof(MessengeService));
            InitUiListener();
            Log.Info(TAG, "Start service Messenge.");
            if (savedInstanceState == null) APP_RUNNIG = false;
            //button.Click += delegate { button.Text = $"{count++} clicks!"; };
            //TextView callSetting = FindViewById<TextView>(Resource.Id.call_setting);
            //TextView smsSetting = FindViewById<TextView>(Resource.Id.sms_setting);
            //TextView status = FindViewById<TextView>(Resource.Id.status);
    //        callSetting.Click += delegate {
    //            {
    //                Intent callSettingIntent = new Intent(this, typeof(TestActivity));
    //                StartActivity(callSettingIntent);
    //            }
    //        };
    //        smsSetting.Click += delegate {
    //            Intent smsSettingIntent = new Intent(this, typeof(Test_STT_Activity));
				//StartActivity(smsSettingIntent);
            //};
            //-----------//           
            //int count = 0;
            //if (savedInstanceState == null) APP_RUNNIG = false;
            //var imageButton = FindViewById<ImageButton>(Resource.Id.btn_power);
            //imageButton.Click += async delegate {
            //    if (!APP_RUNNIG)
            //    { // Start Application
            //        imageButton.SetImageResource(Resource.Drawable.start);
            //        //status.Text = "Click to turn off";
            //        //status.SetTextColor(Android.Graphics.Color.Red);
            //        Toast.MakeText(this, "Application Started", ToastLength.Long).Show();
            //        APP_RUNNIG = true;
            //        await StartApplication();
            //    }
            //    else
            //    { // Stop Application
            //        imageButton.SetImageResource(Resource.Drawable.end);
            //        //status.Text = "Click to turn on";
            //        //status.SetTextColor(Android.Graphics.Color.Green);
            //        Toast.MakeText(this, "Application Stopped", ToastLength.Long).Show();
            //        APP_RUNNIG = false;
            //        await StopApplication();
            //    }
            //};
        }
        private void InitUiListener()
        {
            Button btnRun = FindViewById<Button>(Resource.Id.btn_run);
            Button btnSetting = FindViewById<Button>(Resource.Id.btn_setting);
            btnRun.Click += async delegate {
                await HanleMainService(btnRun);
            };

            btnSetting.Click += delegate {
                Intent settingIntent = new Intent(this, typeof(Setting_Speech));
                StartActivity(settingIntent);
            };
        }

        private async Task HanleMainService(Button btn)
        {
            if (!APP_RUNNIG){      
                btn.SetText(Resource.String.start_app);
                Toast.MakeText(this, "Application Started", ToastLength.Long).Show();
                APP_RUNNIG = true;
                await StartApplication();
            }
            else {
                btn.SetText(Resource.String.stop_app);
                Toast.MakeText(this, "Application Stop", ToastLength.Long).Show();
                APP_RUNNIG = false;
                await StopApplication();
            }
        }

        protected override void OnResume()
        {
            //Listen for SMS
            //smsReceiver = new SMSBroadcastReceiver(this,this);
            //this.RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
            //--------//
            base.OnResume();
            Log.Debug(TAG, "OnResume");

            //RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
        }
        protected override void OnPause()
        {

            //UnregisterReceiver(smsReceiver);
            Log.Debug(TAG, "OnPause");
            // Code omitted for clarity
            base.OnPause();
        }
        void RequestPermission()
        {
            //Intent checkPermissionIntent = new Intent(this, typeof(CheckPermission));
            //StartActivityForResult(checkPermissionIntent,CHECK_PERMISSION);
        }
        async Task StartApplication()
        {
            Log.Info(TAG, "StartApplication");
            await StartInitTTS();
            StartService(MessengeServiceToStart);
        }

        async Task StopApplication()
        {
            Log.Info(TAG, "StopApplication");
            StopService(MessengeServiceToStart);
            await StopTTS();

        }

        void StartReadConfig()
        {
            Config config = Config.Instance();
            config.Read(this);
        }
        async Task StartInitTTS()
        {                        
            await _tts.GetTTS(this);

            //if (string.IsNullOrEmpty(ttsConfig.lang))
            //{
            //    await tts.SetLang(new Java.Util.Locale(ttsConfig.lang));
            //}
        }
        async Task StopTTS()
        {            
            await _tts.Stop();
        }


        protected override void OnSaveInstanceState(Bundle savedInstanceState)
        {
            // Save UI state changes to the savedInstanceState.
            // This bundle will be passed to onCreate if the process is
            // killed and restarted.
            this.Resources.GetString(Resource.String.SAVE_APP_RUNNING_STATUS);
            savedInstanceState.PutBoolean("MyBoolean", true);
            base.OnSaveInstanceState(savedInstanceState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            // Restore UI state from the savedInstanceState.
            // This bundle has also been passed to onCreate.
            string name = this.Resources.GetString(Resource.String.SAVE_APP_RUNNING_STATUS);
            APP_RUNNIG = savedInstanceState.GetBoolean(name);
            //Log.Info(TAG, "APP_RUNNIG");
        }
    }
}