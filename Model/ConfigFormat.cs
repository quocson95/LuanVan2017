using System;
namespace FreeHand
{
    public class ConfigFormat
    {
        public ConfigFormat()
        {
        }
        public TTSConfig TtsConfig;

    }


	public class TTSConfig
	{
		public string engineName { set; get; }
		public string lang { set; get; }
	}

	public class MessengeConfig
	{
		//public string 
	}
}
