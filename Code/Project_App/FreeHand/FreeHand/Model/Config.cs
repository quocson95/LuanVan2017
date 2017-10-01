using System.IO;
using Android.Content;
using Newtonsoft.Json;
using Android.Util;

namespace FreeHand
{
    public class Config
    {
        private static string TAG = "Config";
        private ConfigFormat configFormat;

        public ConfigFormat ConfigFormat
        {
            get
            {
                return configFormat;
            }
        }

        //public MessengeConfig _messengeConfig;		

        private static Config instance;
		string content;

		private Config()
		{			
            
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
