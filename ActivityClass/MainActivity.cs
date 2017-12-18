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
using Android.Telephony;
using Android.Support.V7.App;
using Android.Media;
using Calligraphy;

namespace FreeHand
{
    [Activity(Label = "MainActivity", Theme = "@android:style/Theme.DeviceDefault")]
    public class MainActivity : Activity
    {                            
        Button btnRun;
        private Config _config;       
        private static readonly string TAG = typeof(MainActivity).FullName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Info(TAG, "OnCreate: initializing ");
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                                         .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                         .SetFontAttrId(Resource.Attribute.fontPath)
                                         .Build());

            SetContentView(Resource.Layout.Main_layout);

            RequestPermission();
            LoadConfig();
            InitUiListener();
            InitData();
        }

        private void InitData()
        {
            if (_config.MainServiceRunning)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void InitUiListener()
        {
            btnRun = FindViewById<Button>(Resource.Id.btn_run);
            Button btnSetting = FindViewById<Button>(Resource.Id.btn_setting);
            btnRun.Click +=  delegate {
                 RunClick();
            };

            btnSetting.Click += delegate {
                //Intent settingIntent = new Intent(this, typeof(Messenge.Mail.MailSetting));
                //Intent settingIntent = new Intent(this, typeof(ActivityClass.SettingClass.BlockNumberActivity));
                Intent intent = new Intent(this, typeof(ActivityClass.SettingClass.SettingActivity));
                StartActivity(intent);
            };
        }

        private void RunClick()
        {
            if (!_config.MainServiceRunning)
            {
                Start();             
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            btnRun.SetText(Resource.String.stop_app);
            Model.Commom.StopMainService();
            _config.MainServiceRunning = false;
        }

        private void Start()
        {
            btnRun.SetText(Resource.String.start_app);
            Model.Commom.StartMainService();
            _config.MainServiceRunning = true;
        }

        private void LoadConfig()
        {
            _config = Config.Instance();
            _config.Load();
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

        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.Save();
            base.OnStop();
        }
        void RequestPermission()
        {
            //Intent checkPermissionIntent = new Intent(this, typeof(CheckPermission));
            //StartActivityForResult(checkPermissionIntent,CHECK_PERMISSION);
        }



          

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}