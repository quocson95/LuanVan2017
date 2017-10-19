using System.IO;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;

namespace FreeHand
{
    public class Config
    {
        private static readonly string TAG = "Config";
        private bool _updateConfig;
        private bool _writeConfig;
        private bool _runningSpeech;
        private ConfigFormat configFormat;

        public ConfigFormat ConfigFormat
        {
            get
            {
                return configFormat;
            }
        }

        public bool UpdateConfig { get => _updateConfig; set => _updateConfig = value; }
        public bool WriteConfig { get => _writeConfig; set => _writeConfig = value; }
        public bool RunningSpeech { get => _runningSpeech; set => _runningSpeech = value; }

        //TTS
        public TTSConfig GetTTSConfig()
        {
            return configFormat.TtsConfig;
        }

        public string GetTtsLang()
        {
            return configFormat.TtsConfig.lang;
        }

        public void SetTtsLang(string lang)
        {
            configFormat.TtsConfig.lang = lang;
        }

        public string GetTtsEngine()
        {
            return configFormat.TtsConfig.engineName;
        }

        public void SetTtsEngine(string engine)
        {
            configFormat.TtsConfig.engineName = engine;
        }

        //public MessengeConfig _messengeConfig;		

        private static Config instance;
        string content;

        private Config()
        {
            _writeConfig = false;
            _updateConfig = false;
            _runningSpeech = false;
        }

        public static Config Instance()
        {
            if (instance == null)
            {
                instance = new Config();
            }
            return instance;
        }

        public void Read(Context context)
        {
            Log.Info(TAG, "Start Read Config");
            using (StreamReader sr = new StreamReader(context.Assets.Open("config.js")))
            {
                content = sr.ReadToEnd();
            }
            Log.Info(TAG, "Content config.js" + content);
            configFormat = JsonConvert.DeserializeObject<ConfigFormat>(content);
        }


    }

}
