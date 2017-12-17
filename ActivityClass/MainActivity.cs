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
        private bool APP_RUNNIG;
        Intent MessengeServiceToStart,PhoneCallServiceToStart;
        Intent startMainServiceIntent, stopMainServiceInstant;
        TTSLib _tts;
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

            if (savedInstanceState == null)
            {
                APP_RUNNIG = false;
            }
            //_tts = TextToSpeechLib.Instance();
            //_tts.SetMainContext(this);
            //StartReadConfig();
            //MessengeServiceToStart = new Intent(this, typeof(MessengeService));
            //PhoneCallServiceToStart = new Intent(this, typeof(Phone.PhoneCallService));
            //Log.Info(TAG, "Start service Messenge.");            
            InitUiListener();
        }
        private void InitUiListener()
        {
            Button btnRun = FindViewById<Button>(Resource.Id.btn_run);
            Button btnSetting = FindViewById<Button>(Resource.Id.btn_setting);
            btnRun.Click +=  delegate {
                 RunClick(btnRun);
            };

            btnSetting.Click += delegate {
                //Intent settingIntent = new Intent(this, typeof(Messenge.Mail.MailSetting));
                //Intent settingIntent = new Intent(this, typeof(ActivityClass.SettingClass.BlockNumberActivity));
                Intent intent = new Intent(this, typeof(ActivityClass.SettingClass.SettingActivity));
                StartActivity(intent);
            };
        }

        private void RunClick(Button btn)
        {
            if (!_config.MainServiceRunning)
            {      
                btn.SetText(Resource.String.start_app);
                //Toast.MakeText(this, "Application Started", ToastLength.Long).Show();
                //APP_RUNNIG = true;
                startMainServiceIntent = new Intent(this, typeof(MainService));
                startMainServiceIntent.SetAction(Model.Constants.ACTION_START);
                StartService(startMainServiceIntent);
                _config.MainServiceRunning = true;
                //await StartApplication();
            }
            else
            {
                btn.SetText(Resource.String.stop_app);
                //Toast.MakeText(this, "Application Stop", ToastLength.Long).Show();
                //APP_RUNNIG = false;
                stopMainServiceInstant = new Intent(this, typeof(MainService));
                stopMainServiceInstant.SetAction(Model.Constants.ACTION_STOP);
                StartService(stopMainServiceInstant);
                //StopService(stopMainServiceInstant);
                _config.MainServiceRunning = false;
                //await StopApplication();
            }
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
        async Task StartApplication()
        {
            Log.Info(TAG, "StartApplication");

            await StartInitTTS();
            _config.AudioManage = (AudioManager)GetSystemService(Context.AudioService);
            TelephonyManager tm = (TelephonyManager)this.GetSystemService(Context.TelephonyService);
            StartService(MessengeServiceToStart);
            StartService(PhoneCallServiceToStart);
        }

        void StopApplication()
        {
            Log.Info(TAG, "StopApplication");
            StopService(MessengeServiceToStart);
            StopService(PhoneCallServiceToStart);
            _config.Clean();
            _tts.Stop();

        }

        void StartReadConfig()
        {
            _config = Config.Instance();
            //_config.Read(this);
            _config.Load();
        }
        async Task StartInitTTS()
        {                        
            await _tts.GetTTS();

            //if (string.IsNullOrEmpty(ttsConfig.lang))
            //{
            //    await tts.SetLang(new Java.Util.Locale(ttsConfig.lang));
            //}
        }    

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}