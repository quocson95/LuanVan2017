using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech.Tts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Java.Util;
using Java.Lang;
using Android.Util;

namespace FreeHand
{
    [Activity(Label = "TextToSpeechLib")]
    public class TextToSpeechLib : Activity, TextToSpeech.IOnInitListener
    {
        private static readonly string TAG = "TextToSpeechLib";
		private Context _context;
		//private string _engineName;
        private static readonly Int16 REQUEST_CODE = 1995, LANG_REQUEST = 2017;
        private static TextToSpeechLib instance; //Singleton obj
        private TTSConfig _ttsConfig;
        public TextToSpeech textToSpeech;
        private TaskCompletionSource<Java.Lang.Object> _tcs;

		public TextToSpeechLib() { }

        private TextToSpeechLib(Context context)
        {
            this._context = context;
            Config conf = Config.Instance();
            _ttsConfig = conf.GetTTSConfig();
        }

        //public void SetEngine(string engine){
            
        //}

		public async Task<bool> SetLang(Locale lang)
		{

			_tcs = null;
			_tcs = new TaskCompletionSource<Java.Lang.Object>();
			textToSpeech.SetLanguage(lang);
			if ((int)await _tcs.Task != (int)OperationResult.Success)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public static TextToSpeechLib Instance(Context c)
        {
            if (instance == null) instance = new TextToSpeechLib(c);
            return instance;
        }


        public async Task<TextToSpeech> GetTTS(){
            if (textToSpeech != null) return textToSpeech;
            else return await CreateTtsAsync();
        }

        public async Task<TextToSpeech> CreateTtsAsync()
        {
            //Reinitilize
            if (textToSpeech != null)
            {
                try
                {
                    textToSpeech.Stop();
                    textToSpeech.Shutdown();
                    textToSpeech = null;
                }
                catch 
                {
                    /*don't care */
                }

            }
            _tcs = null;
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            string engineName = _ttsConfig.engineName;
            textToSpeech = new TextToSpeech(_context, this, engineName);
            if ((int)await _tcs.Task != (int)OperationResult.Success)
            {
                Log.Debug(TAG, "Engine: " + engineName + " failed to initialize.");
                textToSpeech = null;
            }           
            _tcs = null;
            return textToSpeech;
        }

        //Get Engines support by Device
        public IList<TextToSpeech.EngineInfo> GetEngines(Context c){
            Log.Debug(TAG, "Trying to get Engine: ");
            TextToSpeech _tts = new TextToSpeech(c, null);
            IList<TextToSpeech.EngineInfo> engines = _tts.Engines;
            try
            {
                _tts.Shutdown();
            }
            catch 
            { 
                /* don't care */ 
            }
            return engines;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == REQUEST_CODE || requestCode == LANG_REQUEST)
            {
                _tcs.SetResult(data);
            }
        }

        protected async Task<Intent> StartActivityForResultAsync(Intent intent, int requestCode)
        {
            Intent data = null;
            try
            {
                _tcs = new TaskCompletionSource<Java.Lang.Object>();
                int _requestWanted = requestCode;
                StartActivityForResult(intent, _requestWanted);
                // possible exceptions: ActivityNotFoundException, also got SecurityException from com.turboled
                data = (Intent)await _tcs.Task;
            }
            catch (System.Exception e)
            {
                Log.Debug(TAG, "StartActivityForResult() exception: " + e);
            }
            _tcs = null;
            return data;
        }

        public async Task<ICollection<Locale>>  GetLanguageSupportByEngineAsync(Context c, TextToSpeech.EngineInfo engine){
            Log.Debug(TAG, "Trying to create TTS Engine: ");
            TextToSpeech _tts = await CreateTtsAsync();
            ICollection<Locale> lang = null;
            if (_tts != null){
                lang = _tts.AvailableLanguages;
                //Clear TTS
                try
                {
                    _tts.Shutdown();
                }
                catch{ /* don't care */ }
                _tts = null;
            }
            return lang;
           
        }

		public async Task<int> SpeakMessenger(string msg)
		{
			if (textToSpeech == null)
			{
                Log.Info(TAG," textToSpeech is null");
				return 0;
			}

			if (textToSpeech.IsSpeaking)
			{
				try
				{
					textToSpeech.Stop();
				}
				catch {/*Dont Care*/};
			}
			ICharSequence cs = new Java.Lang.String(msg);
			//textToSpeech.Speak(cs, QueueMode.Flush,null, null);
			//var p = new Dictionary<string, string>();
			Bundle bundle = new Bundle();
			bundle.PutString(TextToSpeech.Engine.KeyParamUtteranceId, "123");
			//p.Add(TextToSpeech.Engine.KeyParamUtteranceId, "ThisUtterance");
			_tcs = null;
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            textToSpeech.Speak(cs, QueueMode.Flush, bundle, "this");
            textToSpeech.PlaySilentUtterance(1000, QueueMode.Add, null);
			int status = (int) await _tcs.Task;
            _tcs = null;
            return status;
			//            //textToSpeech.Speak("this is name of sender", QueueMode.Flush, null);
			//            textToSpeech.PlaySilentUtterance(2000,QueueMode.Add,null);
			//            cs = new Java.Lang.String("this is content of messenge");
			//            textToSpeech.Speak(cs, QueueMode.Add, null, null);
			//textToSpeech.PlaySilentUtterance(2000, QueueMode.Add, null);
		}
		
        public async Task Stop(){
            if (textToSpeech == null) return;
            try {
                textToSpeech.Stop();
                textToSpeech.Shutdown();
            }
            catch (System.Exception e)
            {
                Log.Info(TAG,"Err when Stop TTS "+e);  
            }
        }

		void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
		{
			Log.Debug(TAG, "OnInit() status = " + status);
			if (textToSpeech != null)
			{
				textToSpeech.SetOnUtteranceProgressListener(new UtteranceProgressLs(this));
                Log.Info(TAG,"Set callback for textToSpeech");
			}
			_tcs.SetResult(new Java.Lang.Integer((int)status));

		}

		//Control error when speak       
		public void DoSomething()
		{
            Log.Info(TAG, "Do something called");
            _tcs.SetResult(new Java.Lang.Integer(1));			
		}

		public class UtteranceProgressLs : UtteranceProgressListener
		{
			TextToSpeechLib _parent;

			public UtteranceProgressLs(TextToSpeechLib p_parent)
			{
				_parent = p_parent;
			}

			public override void OnStart(string utteranceId)
			{
				Console.WriteLine("OnStart called");
			}

			public override void OnError(string utteranceId)
			{
				Console.WriteLine("OnError called");
			}

			public override void OnDone(string utteranceId)
			{
				Console.WriteLine("OnDone called");
				_parent.DoSomething();

			}
		}
	}
}
