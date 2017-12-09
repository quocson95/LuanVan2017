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
            //Seting User
            public bool IsHandlePhoneRunnig { get; set; }           
            public int MissedCall { get; set; }
            public bool SmartAlert { get; set ; }
            public bool Enable { get; set; }
            public bool AllowAutoAcceptCall { get; set; }
            public int TimeAutoAcceptCall { get; set; }
            public bool AutoReply { get; set; }
            public string ContentReply { get; set; }
            public bool AutoRejectCall { get; set; }
            //Backup           
            public bool PrevSmartAlert { get; set; }          
            public bool PrevAllowAutoAcceptCall { get; set; }
            public int PrevTimeAutoAcceptCall { get; set; }
            public bool PrevAutoReply { get; set; }
            public string PrevContentReply { get; set; }
            public bool PrevAutoRejectCall { get; set; }


            public PhoneConfig()
            {
                SmartAlert = false;
                Enable = false;
                AllowAutoAcceptCall = false;
                AutoReply = false;
                AutoRejectCall = false;
                ContentReply = "Sample";
                TimeAutoAcceptCall = 0;
                MissedCall = 0;
                Backup();
            }

            public void Backup()
            {           
                Log.Info(TAG,"Phone BackUp");
                PrevSmartAlert = SmartAlert;
                PrevAllowAutoAcceptCall = AllowAutoAcceptCall;
                PrevTimeAutoAcceptCall = TimeAutoAcceptCall;
                PrevAutoReply = AutoReply;
                PrevContentReply = ContentReply;
                PrevAutoRejectCall = AutoRejectCall;
            }

            public void Restore()
            {
                Log.Info(TAG, "Phone Restore");        
                SmartAlert = PrevSmartAlert;
                AllowAutoAcceptCall = PrevAllowAutoAcceptCall;
                TimeAutoAcceptCall = PrevTimeAutoAcceptCall;
                AutoReply = PrevAutoReply;
                ContentReply = PrevContentReply;
                AutoRejectCall = PrevAutoRejectCall;
            }
        }

        public class SMSConfig
        {
            //Setting user
            public bool Enable { set; get; }
            public bool AllowSpeakName { set; get; }
            public bool AllowSpeakNumber { set; get; }
            public bool AllowSpeakContent { set;  get;}
            public bool AllowAutoReply { set; get; }
            //Previous state            
            public bool PrevAllowSpeakName { set; get; }
            public bool PrevAllowSpeakNumber { set; get; }
            public bool PrevAllowSpeakContent { set; get; }
            public bool PrevAllowAutoReply { set; get; }

            public string CustomContetnReply { set; get; }

            //System configure
            public bool IsHandleSMSRunnig { get; set; }
            public Model.IMessengeData MessengeBackUp { get; set; }
            public STATE_SMS StateSMS { get; set; }
            public SMSConfig (){
                StateSMS = STATE_SMS.IDLE;
                MessengeBackUp = null;
                IsHandleSMSRunnig = false;

                Backup();
            }

            public void Clean()
            {
                IsHandleSMSRunnig = false;
                MessengeBackUp = null;
                StateSMS = STATE_SMS.IDLE;
                Model.MessengeQueue messengeQueue = Model.MessengeQueue.GetInstance();
                messengeQueue.Clear();
            }

            public void Backup()
            {
                Log.Info(TAG, "SMS BackUp");
                PrevAllowAutoReply = AllowAutoReply;
                PrevAllowSpeakName = AllowSpeakName;
                PrevAllowSpeakNumber = AllowSpeakNumber;
                PrevAllowSpeakContent = AllowSpeakContent;
                
            }
            public void Restore()
            {
                AllowAutoReply = PrevAllowAutoReply;
                AllowSpeakName = PrevAllowSpeakName;
                AllowSpeakNumber = PrevAllowSpeakNumber;
                AllowSpeakContent = PrevAllowSpeakContent;
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
            public bool isSupportSTT;
            public bool isSupportTTS;
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

        //TODO Using Json to save config
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
            SaveSMSConfig();
            SaveSpeechConfig();

        }

        public void Load()
        {
            Log.Info(TAG, "Load Config");

            //Task configWork = new Task(() => { 
                LoadPhoneConfig();
                LoadSMSConfig();
                LoadSpeechConfig(); 
            //});
            //configWork.Start();

        }

        public void SaveSMSConfig()
        {
            Log.Info(TAG, "Save SMS Config");
            var prefs = Application.Context.GetSharedPreferences("SMS", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("Enable",smsConfig.Enable);
            prefEditor.PutBoolean("AllowSpeakName",smsConfig.AllowSpeakName);
            prefEditor.PutBoolean("AllowSpeakNumber",smsConfig.AllowSpeakNumber);
            prefEditor.PutBoolean("AllowSpeakContent",smsConfig.AllowSpeakContent);
            prefEditor.PutBoolean("AllowAutoReply",smsConfig.AllowAutoReply);
            prefEditor.PutString("CustomContetnReply",smsConfig.CustomContetnReply);

            //Previous State
            prefEditor.PutBoolean("PrevAllowSpeakName", smsConfig.PrevAllowSpeakName);
            prefEditor.PutBoolean("PrevAllowSpeakNumber", smsConfig.PrevAllowSpeakNumber);
            prefEditor.PutBoolean("PrevAllowSpeakContent", smsConfig.PrevAllowSpeakContent);
            prefEditor.PutBoolean("PrevAllowAutoReply", smsConfig.PrevAllowAutoReply);
            prefEditor.Commit();
        }
        public void SavePhoneConfig()
        {
            Log.Info(TAG, "Save Phone Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            //prefEditor.PutBoolean("Enable", phoneConfig.Enable);
            //prefEditor.PutBoolean("AllowAutoAcceptCall", phoneConfig.AllowAutoAcceptCall);
            //prefEditor.PutBoolean("SmartAlert", phoneConfig.SmartAlert);
            //prefEditor.PutInt("TimeAutoAcceptCall", phoneConfig.TimeAutoAcceptCall);
            //prefEditor.PutBoolean("AutoReply",phoneConfig.AutoReply);
            //prefEditor.PutString("ContentReply",phoneConfig.ContentReply);
            string value = JsonConvert.SerializeObject(phoneConfig);
            prefEditor.PutString("PhoneConfig", value);

            //Backup
    //    public bool PrevIsHandlePhoneRunnig { get; set; }
        //public int PrevMissedCall { get; set; }
        //public bool PrevSmartAlert { get; set; }
        //public bool PrevEnable { get; set; }
        //public bool PrevAllowAutoAcceptCall { get; set; }
        //public int PrevTimeAutoAcceptCall { get; set; }
        //public bool PrevAutoReply { get; set; }
        //public string PrevContentReply { get; set; }
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
            smsConfig.AllowAutoReply = prefs.GetBoolean("AllowAutoReply", false);
            smsConfig.CustomContetnReply = prefs.GetString("CustomContetnReply", "");

            //Previous State
            smsConfig.PrevAllowSpeakName = prefs.GetBoolean("PrevAllowSpeakName", false);
            smsConfig.PrevAllowSpeakNumber = prefs.GetBoolean("PrevAllowSpeakNumber", false);
            smsConfig.PrevAllowSpeakContent = prefs.GetBoolean("PrevAllowSpeakContent", false);
            smsConfig.PrevAllowAutoReply = prefs.GetBoolean("PrevAllowAutoReply", false);
        }

        private void LoadPhoneConfig()
        {
            Log.Info(TAG, "Load Phone Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            string value = prefs.GetString("PhoneConfig", NOT_FOUND);
            if (!value.Equals(NOT_FOUND))
            {
                phoneConfig = JsonConvert.DeserializeObject<PhoneConfig>(value);
            }
            //phoneConfig.Enable = prefs.GetBoolean("Enable", false);
            //phoneConfig.AllowAutoAcceptCall = prefs.GetBoolean("AllowAutoAcceptCall", false);
            //phoneConfig.SmartAlert = prefs.GetBoolean("SmartAlert", false);
            //phoneConfig.TimeAutoAcceptCall = prefs.GetInt("TimeAutoAcceptCall", 10);
            //phoneConfig.AutoReply = prefs.GetBoolean("AutoReply", false);
            //phoneConfig.ContentReply = prefs.GetString("ContentReply", "Sample phone reply");
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
            ttsConfig.LangSelectByTTS = prefs.GetString("TTSConfig.LangSelectByTTS", "en");
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
            valueDefault = ttsConfig.LangSelectByTTS;
            //if (valueDefault.Equals(NOT_FOUND)){
            //    Log.Info(TAG,"a");
            //    ttsConfig.LangSelectByTTS = ttsConfig.LangSupportByTTS.Where(pair => pair.Value.Equals("English"))
            //        .Select(pair => pair.Key)
            //        .FirstOrDefault();
            //}
        }

        async void LoadSTTConfig()
        {
            Log.Info(TAG, "Load Speech STT Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            ttsConfig.LangSelectBySTT = prefs.GetString("TTSConfig.LangSelectBySTT", NOT_FOUND);
            string valueDefault;

            valueDefault = prefs.GetString("TTSConfig.LangSupportBySTT", NOT_FOUND);

            if (string.IsNullOrEmpty(valueDefault) != false)
            {
                ttsConfig.isSupportSTT = true;
                if (valueDefault.Equals(NOT_FOUND))
                {
                    STTLib stt = STTLib.Instance();
                    ttsConfig.LangSupportBySTT = await stt.GetLanguageSupportDisplayLanguage(Application.Context);
                }
                else
                {
                    ttsConfig.LangSupportBySTT = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueDefault);
                }

                if (ttsConfig.LangSelectBySTT.Equals(NOT_FOUND))
                {
                    ttsConfig.LangSelectBySTT = ttsConfig.LangSupportBySTT.Where(pair => pair.Value.Equals("English"))
                        .Select(pair => pair.Key)
                        .FirstOrDefault();
                }
            }
            else {
                ttsConfig.isSupportSTT = false;
            }
        }


    }

}
