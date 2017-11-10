using System.IO;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;
using Android.Media;
namespace FreeHand
{

    public class Config
    {
        public class PhoneConfig
        {
            private bool smartAlert;
            private bool enable;
            private bool allowAutoAcceptCall;
            private int timeAutoAcceptCall;
            public PhoneConfig()
            {
                smartAlert = false;
                enable = false;
                allowAutoAcceptCall = false;
                timeAutoAcceptCall = 0;
            }

            public bool SmartAlert { get => smartAlert; set => smartAlert = value; }
            public bool Enable { get => enable; set => enable = value; }
            public bool AllowAutoAcceptCall { get => allowAutoAcceptCall; set => allowAutoAcceptCall = value; }
            public int TimeAutoAcceptCall { get => timeAutoAcceptCall; set => timeAutoAcceptCall = value; }
        }

        public class TTSConfig
        {
            public string EngineName { set; get; }
            public string Lang { set; get; }
        }
        private static readonly string TAG = "Config";
        private bool _updateConfig;
        private bool _writeConfig;
        private bool _runningSMSHandle;
        private bool _runningCallHanle;
        private AudioManager audioManage;

        public PhoneConfig phoneConfig;
        public TTSConfig ttsConfig;
       

        public bool UpdateConfig { get => _updateConfig; set => _updateConfig = value; }
        public bool WriteConfig { get => _writeConfig; set => _writeConfig = value; }
        public bool RunningSMSHandle { get => _runningSMSHandle; set => _runningSMSHandle = value; }
        public AudioManager AudioManage { get => audioManage; set => audioManage = value; }
        public bool RunningCallHanle { get => _runningCallHanle; set => _runningCallHanle = value; }

        //TTS
        //public MessengeConfig _messengeConfig;		

        private static Config instance;
        string content;

        private Config()
        {
            _writeConfig = false;
            _updateConfig = false;
            _runningSMSHandle = false;
            _runningCallHanle = false;
            phoneConfig = new PhoneConfig();
            ttsConfig = new TTSConfig();
        }

        public static Config Instance()
        {
            if (instance == null)
            {
                instance = new Config();
            }
            return instance;
        }

        //Phone Config


        public void Read(Context context)
        {
            //Log.Info(TAG, "Start Read Config");
            //using (StreamReader sr = new StreamReader(context.Assets.Open("config.js")))
            //{
            //    content = sr.ReadToEnd();
            //}
            //Log.Info(TAG, "Content config.js" + content);
            //configFormat = JsonConvert.DeserializeObject<ConfigFormat>(content);
        }

        public void save()
        {
            Log.Info(TAG, "Save Config");
            savePhoneConfig();
        }

        public void load()
        {
            Log.Info(TAG,"Load Config");
            loadPhoneConfig();
            loadTTSConfig();
        }
        private void savePhoneConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("PhoneConfig.Enable", phoneConfig.Enable);
            prefEditor.PutBoolean("PhoneConfig.AllowAutoAcceptCall", phoneConfig.AllowAutoAcceptCall);
            prefEditor.PutBoolean("PhoneConfig.SmartAlert", phoneConfig.SmartAlert);
            prefEditor.PutInt("PhoneConfig.TimeAutoAcceptCall", phoneConfig.TimeAutoAcceptCall);
            prefEditor.Commit();
        }
            
        private void saveTTSConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("TTSConfig.EngineName", ttsConfig.EngineName);
            prefEditor.PutString("TTSConfig.Lang", ttsConfig.Lang);
            prefEditor.Commit();
        }

        private void loadPhoneConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            phoneConfig.Enable = prefs.GetBoolean("PhoneConfig.Enable", false);
            phoneConfig.AllowAutoAcceptCall = prefs.GetBoolean("PhoneConfig.AllowAutoAcceptCall", false);
            phoneConfig.SmartAlert = prefs.GetBoolean("PhoneConfig.SmartAlert", false);
            phoneConfig.TimeAutoAcceptCall = prefs.GetInt("PhoneConfig.TimeAutoAcceptCall", 10);
        }

        private void loadTTSConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            ttsConfig.EngineName = prefs.GetString("TTSConfig.EngineName", "Default");
            ttsConfig.Lang = prefs.GetString("TTSConfig.Lang", "en-US");
        }

    }

}
