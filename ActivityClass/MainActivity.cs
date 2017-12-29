using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Calligraphy;
namespace FreeHand
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {                            
        Button btnRun;
        TextView btnSetting;
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
            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            //RequestPermission();
            LoadConfig();
            InitUiListener();
            InitData();
        }
               

        private void InitData()
        {
            if (_config.IsMainServiceRunning)
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
            btnSetting = FindViewById<TextView>(Resource.Id.btn_setting);
            btnRun.Click +=  delegate {
                 RunClick();
            };

            btnSetting.Click += delegate {               
                //Intent intent = new Intent(this, typeof(ActivityClass.SettingClass.Setting));
                Intent intent = new Intent(this, typeof(Message.Mail.MailActivity));
                //Intent intent = new Intent(this, typeof(ActivityClass.SettingClass.AddGmailAccountActivity));
                //Intent intent = new Intent(this, typeof(SpeechLibrary.STTActivity));
                StartActivity(intent);
            };
        }

        private void RunClick()
        {
            if (!_config.IsMainServiceRunning)
                Start();
            else
                Stop();            
        }

        private void Stop()
        {
            btnRun.SetText(Resource.String.stop_app);
            Model.Commom.StopMainService();
            _config.IsMainServiceRunning = false;
        }

        private void Start()
        {
            btnRun.SetText(Resource.String.start_app);
            Model.Commom.StartMainService();
            _config.IsMainServiceRunning = true;
        }

        private void LoadConfig()
        {
            _config = Config.Instance();
            _config.Load();
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetLanguage();        
            Log.Debug(TAG, "OnResume");                   
        }
        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");           
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
        }

        void SetLanguage()
        {
            btnSetting.SetText(Resource.String.label_setting);
            if (_config.IsMainServiceRunning){
                btnRun.SetText(Resource.String.start_app);
            }
            else 
                btnRun.SetText(Resource.String.stop_app);
        }

        //Font
        protected override void AttachBaseContext(Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(c));
        }
       
      
    }
}