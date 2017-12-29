using System.IO;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;
using Android.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using FreeHand.Message.Mail;

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

        public readonly string NOT_FOUND = "null";

        /*
         * Phone Config
         * Save all phone data configure of user
         */
        public class Phone
        {
            //Seting User
            public bool IsHandlePhoneRunnig { get; set; }
            public int MissedCall { get; set; }
            public bool SmartAlert { get; set; }
            public bool Enable { get; set; }
            public bool AutoReply { get; set; }
            public string ContentReply { get; set; }

            public IList<Tuple<string, string>> BlackList { get; set; }
            public bool BlockAll { get; set; }
            public bool BlockInList { get; set; }
            //Backup           
            public bool PrevSmartAlert { get; set; }
            public bool PrevAutoReply { get; set; }
            public bool PrevAutoRejectCall { get; set; }


            public Phone()
            {
                SmartAlert = false;
                Enable = false;
                AutoReply = false;

                BlockAll = false;
                BlockInList = false;

                ContentReply = "Sample";
                MissedCall = 0;
                Backup();
                BlackList = new List<Tuple<string, string>>();
            }

            public void Backup()
            {
                Log.Info(TAG, "Phone BackUp");
                PrevSmartAlert = SmartAlert;
                PrevAutoReply = AutoReply;
            }

            public void Restore()
            {
                Log.Info(TAG, "Phone Restore");
                SmartAlert = PrevSmartAlert;
                AutoReply = PrevAutoReply;
            }
        }


        /*
         * SMS Config
         * Save all sms data configure of user
         */
        public class SMS
        {
            //Setting user
            public bool Enable { set; get; }
            public bool AllowSpeakName { set; get; }
            public bool AllowSpeakNumber { set; get; }
            public bool AllowSpeakContent { set; get; }
            public bool AllowAutoReply { set; get; }
            //Previous state            
            public bool PrevAllowSpeakName { set; get; }
            public bool PrevAllowSpeakNumber { set; get; }
            public bool PrevAllowSpeakContent { set; get; }
            public bool PrevAllowAutoReply { set; get; }

            public string CustomContetnReply { set; get; }

            //System configure

            public IList<Tuple<string, string>> BlockList { get; set; }
            public bool IsHandleSMSRunnig { get; set; }
            public object MessengeBackUp { get; set; }
            public STATE_SMS StateSMS { get; set; }
            public SMS()
            {
                StateSMS = STATE_SMS.IDLE;
                MessengeBackUp = null;
                IsHandleSMSRunnig = false;
                BlockList = new List<Tuple<string, string>>();

                Backup();
            }

            public void Clean()
            {
                IsHandleSMSRunnig = false;
                //MessengeBackUp = null;
                Model.MessengeQueue messengeQueue = Model.MessengeQueue.GetInstance();
                messengeQueue.CleanSMS();
                StateSMS = STATE_SMS.IDLE;

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

        /*
         * Speech Config
         * Save all Speech data configure of user
         * TTS: Text To Speech
         * SST: Speech To Text
         */

        public class Speech
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
            public Speech()
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

        public class Account
        {
            public IList<IMailAction> LstMail { get; set; }
            public Account()
            {
                LstMail = new List<IMailAction>();
            }

            public void Restore(IList<Tuple<string, string>> list)
            {
                Task.Run(() =>
                {
                    LstMail.Clear();
                    foreach (var item in list)
                    {
                        IMailAction gmail = new GmailAction(item.Item1, item.Item2);
                        LstMail.Add(gmail);
                    }
                });
            }
        }

        /*
         * Mail Config
         * Save all Speech data configure of user         
         */
        public class Mail
        {
            public IList<Tuple<string, string>> lstAccount;
            public bool Enable { get; set; }
            public bool AutoReply { get; set; }
            public string ContentReply { get; set; }
            public bool AllowSpeakAddr { get; set; }
            public bool AllowSpeakName { get; set; }
            public bool AllowSpeakContent { get; set; }

            public bool PrevAutoReply {get;set;}
            public bool PrevAllowSpeakAddr { get; set; }
            public bool PrevAllowSpeakName { get; set; }
            public bool PrevAllowSpeakContent { get; set; }

            public Mail()
            {
                lstAccount = new List<Tuple<string, string>>();
                Enable = false;
                AutoReply = false;
                ContentReply = "";
                AllowSpeakAddr = false;
                AllowSpeakName = false;
                AllowSpeakContent = false;
                Backup();
            }

            public void Backup()
            {
                PrevAutoReply = AutoReply;
                PrevAllowSpeakAddr = AllowSpeakAddr;
                PrevAllowSpeakName = AllowSpeakName;
                PrevAllowSpeakContent = AllowSpeakContent;
            }

            public void Restore()
            {
                AutoReply = PrevAutoReply;
                AllowSpeakAddr = PrevAllowSpeakAddr;
                AllowSpeakName = PrevAllowSpeakName;
                AllowSpeakContent = PrevAllowSpeakContent;
            }

            public void Clean()
            {
                Model.MessengeQueue _messQueue = Model.MessengeQueue.GetInstance();
                _messQueue.CleanMail();
            }
                       
        }


        /*
         * Config contrutor
         * 
         */
        private static readonly string TAG = "Config";
        public bool IsMainServiceRunning { set; get; }
        public Phone phone;
        public SMS sms;
        public Speech speech;
        public Account account;
        public Mail mail;
        public bool IsUpdateCfg { get; set; }

        private static Config instance;

        private Config()
        {
            IsMainServiceRunning = false;
            phone = new Phone();
            sms = new SMS();
            speech = new Speech();
            mail = new Mail();
            account = new Account();
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
            Model.MessengeQueue _messQueue = Model.MessengeQueue.GetInstance();
            _messQueue.Clear();
            phone.IsHandlePhoneRunnig = false;

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
                    is_allow = !phone.IsHandlePhoneRunnig;
                    break;
                case PERMISSION_RUN.NOTIFY_MISS_CALL:
                    is_allow = !(phone.IsHandlePhoneRunnig || sms.IsHandleSMSRunnig);
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
            SaveMailConfig();
            SaveSpeechConfig();
            SaveStateApp();

        }

       

        private void SaveStateApp()
        {
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean("StateApp", IsMainServiceRunning);
            prefEditor.Commit();
        }

        public void Load()
        {
            Log.Info(TAG, "Load Config");

            //Task configWork = new Task(() => { 
            LoadPhoneConfig();
            LoadSMSConfig();
            LoadMailConfig();
            LoadSpeechConfig();
            LoadStateApp();
            //});
            //configWork.Start();

        }


        private void LoadStateApp()
        {
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            IsMainServiceRunning = prefs.GetBoolean("StateApp", false);
        }

        public void SaveSMSConfig()
        {
            Log.Info(TAG, "Save SMS Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            string value = JsonConvert.SerializeObject(sms);
            prefEditor.PutString("SMSConfig", value);
            prefEditor.Commit();
        }

        public void SaveMailConfig()
        {
            Log.Info(TAG, "Save Mail Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            string value = JsonConvert.SerializeObject(mail);
            prefEditor.PutString("MailConfig", value);
            prefEditor.Commit();
        }

        public void SavePhoneConfig()
        {
            Log.Info(TAG, "Save Phone Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            string value = JsonConvert.SerializeObject(phone);
            prefEditor.PutString("PhoneConfig", value);
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
            prefEditor.PutString("TTSConfig.LangSelectBySTT", speech.LangSelectBySTT);
            string langSupportJson = JsonConvert.SerializeObject(speech.LangSupportBySTT);
            prefEditor.PutString("TTSConfig.LangSupportBySTT", langSupportJson);
            prefEditor.Commit();
        }

        private void SaveTTSConfig()
        {
            Log.Info(TAG, "Save TTS Config");
            var prefs = Application.Context.GetSharedPreferences("Freehand", FileCreationMode.Private);
            var prefEditor = prefs.Edit();

            prefEditor.PutFloat("TTSConfig.SeekSpeed", speech.SeekSpeed);
            prefEditor.PutFloat("TTSConfig.SeekPitch", speech.SeekPitch);

            prefEditor.PutString("TTSConfig.EngineNameSelect", speech.EngineNameSelect);
            prefEditor.PutString("TTSConfig.LangSelectByTTS", speech.LangSelectByTTS);

            string valueDefault = JsonConvert.SerializeObject(speech.ListEngineName);
            prefEditor.PutString("TTSConfig.ListEngineName", valueDefault);
            valueDefault = JsonConvert.SerializeObject(speech.LangSupportByTTS);
            prefEditor.PutString("TTSConfig.LangSupportByTTS", valueDefault);
            prefEditor.Commit();
        }

        private void LoadSMSConfig()
        {
            Log.Info(TAG, "Load SMS Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            string value = prefs.GetString("SMSConfig", NOT_FOUND);
            if (!value.Equals(NOT_FOUND))
            {
                sms = JsonConvert.DeserializeObject<SMS>(value);
            }
        }


        private void LoadMailConfig()
        {
            Log.Info(TAG, "Load Mail Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            string value = prefs.GetString("MailConfig", NOT_FOUND);
            if (!value.Equals(NOT_FOUND))
            {
                mail = JsonConvert.DeserializeObject<Mail>(value);
                account.Restore(mail.lstAccount);
            }
        }


        private void LoadPhoneConfig()
        {
            Log.Info(TAG, "Load Phone Config");
            var prefs = Application.Context.GetSharedPreferences("FreeHand", FileCreationMode.Private);
            string value = prefs.GetString("PhoneConfig", NOT_FOUND);
            if (!value.Equals(NOT_FOUND))
            {
                phone = JsonConvert.DeserializeObject<Phone>(value);
            }

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
            speech.EngineNameSelect = prefs.GetString("TTSConfig.EngineNameSelect", NOT_FOUND);
            speech.LangSelectByTTS = prefs.GetString("TTSConfig.LangSelectByTTS", "en");
            speech.SeekPitch = prefs.GetFloat("TTSConfig.SeekPitch", 1);
            speech.SeekSpeed = prefs.GetFloat("TTSConfig.SeekSpeed", 1);
            string valueDefault;
            valueDefault = prefs.GetString("TTSConfig.ListEngineName", NOT_FOUND);
            TTSLib tts = TTSLib.Instance();
            if (valueDefault.Equals(NOT_FOUND))
            {
                speech.ListEngineName = await tts.GetEngines(Application.Context);
            }
            else
            {
                speech.ListEngineName = JsonConvert.DeserializeObject<List<string>>(valueDefault);
            }

            if (speech.EngineNameSelect.Equals(NOT_FOUND))
            {
                speech.EngineNameSelect = speech.ListEngineName[0];
            }


            valueDefault = prefs.GetString("TTSConfig.LangSupportByTTS", NOT_FOUND);
            if (valueDefault.Equals(NOT_FOUND))
            {
                speech.LangSupportByTTS = await tts.GetLanguageSupportByEngineAsync(Application.Context, speech.EngineNameSelect);
            }
            else
            {
                speech.LangSupportByTTS = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueDefault);
            }
            valueDefault = speech.LangSelectByTTS;
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
            speech.LangSelectBySTT = prefs.GetString("TTSConfig.LangSelectBySTT", NOT_FOUND);
            string valueDefault;

            valueDefault = prefs.GetString("TTSConfig.LangSupportBySTT", NOT_FOUND);

            if (valueDefault.Equals(NOT_FOUND))
            {
                STTLib stt = STTLib.Instance();
                speech.LangSupportBySTT = await stt.GetLanguageSupportDisplayLanguage(Application.Context);
            }
            else
            {
                speech.LangSupportBySTT = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueDefault);
            }

            if (speech.LangSelectBySTT.Equals(NOT_FOUND))
            {
                speech.LangSelectBySTT = speech.LangSupportBySTT.Where(pair => pair.Value.Equals("English"))
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
            }
        }
    }
}