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
        public enum STATE_SMS 
        {   
            IDLE,
            SPEAK_NUMBER,
            SPEAK_NAME_SENDER,
            SPEAK_CONTENT,
            LISTENT_REQUEST_ANSWER,
            LISTEN_CONTENT_ANSWER,
            READY_REPLY,
            DONE
        }

        public enum PERMISSION_RUN 
        {
            MESSENGE,
            PHONE
        }
        public class PhoneConfig
        {
            private bool smartAlert;
            private bool enable;
            private bool allowAutoAcceptCall;
            public bool IsHandlePhoneRunnig { get; set; }
            private int timeAutoAcceptCall;
            public int MissedCall { set; get; }

            public PhoneConfig()
            {
                smartAlert = false;
                enable = false;
                allowAutoAcceptCall = false;
                timeAutoAcceptCall = 0;
                MissedCall = 0;
            }

            public bool SmartAlert { get => smartAlert; set => smartAlert = value; }
            public bool Enable { get => enable; set => enable = value; }
            public bool AllowAutoAcceptCall { get => allowAutoAcceptCall; set => allowAutoAcceptCall = value; }
            public int TimeAutoAcceptCall { get => timeAutoAcceptCall; set => timeAutoAcceptCall = value; }
        }

        public class SMSConfig
        {
            public bool IsHandleSMSRunnig { get; set; }
            public Model.IMessengeData MessengeBackUp { get; set; }
            public STATE_SMS StateSMS { get; set; }
            public SMSConfig (){
                StateSMS = STATE_SMS.IDLE;
                MessengeBackUp = null;
            }
        }

        public class TTSConfig
        {
            public string EngineName { set; get; }
            public string Lang { set; get; }
        }

        private static readonly string TAG = "Config";
        private bool _updateConfig;
        private bool _writeConfig;
        private AudioManager audioManage;
        public PhoneConfig phoneConfig;
        public SMSConfig smsConfig;
        public TTSConfig ttsConfig;
        private Model.MessengeQueue messengeQueue;
        public bool UpdateConfig { get => _updateConfig; set => _updateConfig = value; }
        public bool WriteConfig { get => _writeConfig; set => _writeConfig = value; }

        public AudioManager AudioManage { get => audioManage; set => audioManage = value; }

        //TTS
        //public MessengeConfig _messengeConfig;		

        private static Config instance;
        string content;

        private Config()
        {
            _writeConfig = false;
            _updateConfig = false;

            phoneConfig = new PhoneConfig();
            smsConfig = new SMSConfig();
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

        public void Clean()
        {
            smsConfig.StateSMS = STATE_SMS.IDLE;
            smsConfig.MessengeBackUp = null;
            smsConfig.IsHandleSMSRunnig = false;

            phoneConfig.IsHandlePhoneRunnig = false;

            messengeQueue = Model.MessengeQueue.GetInstance();
            messengeQueue.Clear();

        }
        //Permission run
        public bool GetPermissionRun(PERMISSION_RUN type)
        {
            bool is_allow = false;
            switch (type)
            {
                case PERMISSION_RUN.PHONE:                        
                        is_allow = true;
                        break;
                case PERMISSION_RUN.MESSENGE:
                    is_allow = !phoneConfig.IsHandlePhoneRunnig;
                    break;  
                default:
                    is_allow = false;
                    break;
            }
            return is_allow;
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
            Log.Info(TAG, "Load Config");
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
