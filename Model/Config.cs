using System.IO;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;
using Android.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            PHONE,
            NOTIFY_MISS_CALL
        }

        public readonly string NOT_FOUND = "NOT_FOUND";
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
            //Setting user
            public bool Enable { set; get; }
            public bool AllowSpeakName { set; get; }
            public bool AllowSpeakNumber { set; get; }
            public bool AllowSpeakContent { set;  get;}
            public bool AllowAutoReply { set; get; }
            public string CustomContetnReply { set; get; }

            //System configure
            public bool IsHandleSMSRunnig { get; set; }
            public Model.IMessengeData MessengeBackUp { get; set; }
            public STATE_SMS StateSMS { get; set; }
            public SMSConfig (){
                StateSMS = STATE_SMS.IDLE;
                MessengeBackUp = null;
                IsHandleSMSRunnig = false;
            }

            public void Clean()
            {
                IsHandleSMSRunnig = false;
                MessengeBackUp = null;
                StateSMS = STATE_SMS.IDLE;
                Model.MessengeQueue messengeQueue = Model.MessengeQueue.GetInstance();
                messengeQueue.Clear();
            }
        }

        public class TTSConfig
        {
            public string EngineNameSelect { set; get; }           
            public IList<string> ListEngineName { set; get; } 
            public string LangSelectBySTT { set; get; }
            public string LangSelectByTTS { set; get; }
            public Dictionary<string, string> LangSupportBySTT { set; get; }
            public Dictionary<string, string> LangSupportByTTS { set; get; }
            public float SeekPitch;
            public float SeekSpeed;
            public TTSConfig()
            {
                LangSelectBySTT = null;
                LangSelectByTTS = null;
                LangSupportBySTT = null;
                LangSupportByTTS = null;
                EngineNameSelect = null;
                ListEngineName = null;
                SeekPitch = 0;
                SeekSpeed = 0;

            }

        }


        private static readonly string TAG = "Config";
        private bool _updateConfig;
        private bool _writeConfig;
        public bool MainServiceRunning { set; get; }
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

        private Config()
        {
            _writeConfig = false;
            _updateConfig = false;
            MainServiceRunning = false;
            audioManage = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
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

        /*
         * Clean Queue Mess when Off Application
         */
        public void Clean()
        {
            smsConfig.Clean();

            phoneConfig.IsHandlePhoneRunnig = false;



        }


        /*
         * Init Speech Engine for STT and TTS
         */

        public void InitSpeechData()
        {
            
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
                case PERMISSION_RUN.NOTIFY_MISS_CALL:
                    is_allow = !(phoneConfig.IsHandlePhoneRunnig || smsConfig.IsHandleSMSRunnig);
                    break;
                default:
                    is_allow = false;
                    break;
            }
            return is_allow;
        }
        //Phone Config

               

        public void Save()
        {
            Log.Info(TAG, "Save Config");
            SavePhoneConfig();
            SaveSpeechConfig();
        }

        public void Load()
        {
            Log.Info(TAG, "Load Config");

            //Task configWork = new Task(() => { 
                LoadPhoneConfig();
                LoadSpeechConfig(); 
            //});
            //configWork.Start();

        }

        private void SaveSMSConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("SMS", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("Enable",smsConfig.Enable);
            prefEditor.PutBoolean("AllowSpeakName",smsConfig.AllowSpeakName);
            prefEditor.PutBoolean("AllowSpeakNumber",smsConfig.AllowSpeakNumber);
            prefEditor.PutBoolean("AllowSpeakContent",smsConfig.AllowSpeakContent);
            prefEditor.PutBoolean("AllowAutoReply",smsConfig.AllowAutoReply);
            prefEditor.PutString("CustomContetnReply",smsConfig.CustomContetnReply);
        }
        private void SavePhoneConfig()
        {
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("PhoneConfig.Enable", phoneConfig.Enable);
            prefEditor.PutBoolean("PhoneConfig.AllowAutoAcceptCall", phoneConfig.AllowAutoAcceptCall);
            prefEditor.PutBoolean("PhoneConfig.SmartAlert", phoneConfig.SmartAlert);
            prefEditor.PutInt("PhoneConfig.TimeAutoAcceptCall", phoneConfig.TimeAutoAcceptCall);
            prefEditor.Commit();
        }

        private void SaveSpeechConfig()
        {
            SaveSTTConfig();
            SaveTTSConfig();
        }

        private void SaveSTTConfig()
        {
            Log.Info(TAG, "Save STT Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("TTSConfig.LangSelectBySTT", ttsConfig.LangSelectBySTT);
            string langSupportJson = JsonConvert.SerializeObject(ttsConfig.LangSupportBySTT);
            prefEditor.PutString("TTSConfig.LangSupportBySTT", langSupportJson);
            prefEditor.Commit();
        }

        private void SaveTTSConfig()
        {
            Log.Info(TAG, "Save TTS Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();

            prefEditor.PutFloat("TTSConfig.SeekSpeed", ttsConfig.SeekSpeed);
            prefEditor.PutFloat("TTSConfig.SeekPitch", ttsConfig.SeekPitch);

            prefEditor.PutString("TTSConfig.EngineNameSelect", ttsConfig.EngineNameSelect);
            prefEditor.PutString("TTSConfig.LangSelectByTTS", ttsConfig.LangSelectByTTS);

            string valueDefault = JsonConvert.SerializeObject(ttsConfig.ListEngineName);
            prefEditor.PutString("TTSConfig.ListEngineName", valueDefault);
            valueDefault = JsonConvert.SerializeObject(ttsConfig.LangSupportByTTS);
            prefEditor.PutString("TTSConfig.LangSupportByTTS", valueDefault); 
            prefEditor.Commit();
        }

        private void LoadSMSConfig()
        {
        //    public bool Enable { set; get; }
        //public bool AllowSpeakName { set; get; }
        //public bool AllowSpeakNumber { set; get; }
        //public bool AllowSpeakContent { set; get; }
        //public bool AllowAutoReply { set; get; }
        //public string CustomContetnReply { set; get; }
            Log.Info(TAG, "Load SMS Config");
            var prefs = Application.Context.GetSharedPreferences("SMS", FileCreationMode.Private);
            smsConfig.Enable = prefs.GetBoolean("Enable", false);
            smsConfig.AllowSpeakName = prefs.GetBoolean("AllowSpeakName", false);
            smsConfig.AllowSpeakNumber = prefs.GetBoolean("AllowSpeakNumber", false);
            smsConfig.AllowSpeakContent = prefs.GetBoolean("AllowSpeakContent", false);
            smsConfig.CustomContetnReply = prefs.GetString("CustomContetnReply", "");
        }

        private void LoadPhoneConfig()
        {
            Log.Info(TAG, "Load Phone Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            phoneConfig.Enable = prefs.GetBoolean("PhoneConfig.Enable", false);
            phoneConfig.AllowAutoAcceptCall = prefs.GetBoolean("PhoneConfig.AllowAutoAcceptCall", false);
            phoneConfig.SmartAlert = prefs.GetBoolean("PhoneConfig.SmartAlert", false);
            phoneConfig.TimeAutoAcceptCall = prefs.GetInt("PhoneConfig.TimeAutoAcceptCall", 10);
        }

        void LoadSpeechConfig()
        {
            //Speech To Text
            LoadSTTConfig();
            //Text To Speech
            LoadTTSConfig();        
        }



        async void LoadTTSConfig()
        {
            Log.Info(TAG, "Load Speech TTS Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            ttsConfig.EngineNameSelect = prefs.GetString("TTSConfig.EngineNameSelect", NOT_FOUND);
            ttsConfig.LangSelectByTTS = prefs.GetString("TTSConfig.LangSelectByTTS", NOT_FOUND);
            ttsConfig.SeekPitch = prefs.GetFloat("TTSConfig.SeekPitch", 1);
            ttsConfig.SeekSpeed = prefs.GetFloat("TTSConfig.SeekSpeed", 1);
            string valueDefault;
            valueDefault = prefs.GetString("TTSConfig.ListEngineName", NOT_FOUND);
            TextToSpeechLib tts = TextToSpeechLib.Instance();
            if (valueDefault.Equals(NOT_FOUND))
            {
                ttsConfig.ListEngineName = await tts.GetEngines(Application.Context);
            }
            else {
                ttsConfig.ListEngineName = JsonConvert.DeserializeObject<List<string>>(valueDefault);
            }

            if (ttsConfig.EngineNameSelect.Equals(NOT_FOUND))
            {
                ttsConfig.EngineNameSelect = ttsConfig.ListEngineName[0];
            }


            valueDefault = prefs.GetString("TTSConfig.LangSupportByTTS", NOT_FOUND);
            if (valueDefault.Equals(NOT_FOUND))
            {                
                ttsConfig.LangSupportByTTS = await tts.GetLanguageSupportByEngineAsync(Application.Context, ttsConfig.EngineNameSelect);
            }
            else {
                ttsConfig.LangSupportByTTS = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueDefault);
            }

            if (ttsConfig.LangSelectByTTS.Equals(NOT_FOUND)){
                ttsConfig.LangSelectByTTS = ttsConfig.LangSupportByTTS.Where(pair => pair.Value == "English")
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
            }
        }

        async void LoadSTTConfig()
        {
            Log.Info(TAG, "Load Speech STT Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            ttsConfig.LangSelectBySTT = prefs.GetString("TTSConfig.LangSelectBySTT", NOT_FOUND);
            string valueDefault;

            valueDefault = prefs.GetString("TTSConfig.LangSupportBySTT", NOT_FOUND);
            if (valueDefault.Equals("null")) 
                return;
            if (valueDefault.Equals(NOT_FOUND))
            {
                STTLib stt = STTLib.Instance();
                ttsConfig.LangSupportBySTT = await stt.GetLanguageSupportDisplayLanguage(Application.Context);
            }
            else {
                ttsConfig.LangSupportBySTT = JsonConvert.DeserializeObject<Dictionary<string,string>>(valueDefault);
            }

            if (ttsConfig.LangSelectBySTT.Equals(NOT_FOUND))
            {
                ttsConfig.LangSelectBySTT = ttsConfig.LangSupportBySTT.Where(pair => pair.Value == "English")
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
            }
        }


    }

}
