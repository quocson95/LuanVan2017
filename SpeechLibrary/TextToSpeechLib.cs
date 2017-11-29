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
		//private string _engineName;
        private static readonly Int16 REQUEST_CODE = 1995, LANG_REQUEST = 2017;
        private static TextToSpeechLib instance; //Singleton obj
        private Config _config;
        private Context _mainContext;
        private Dictionary<string, string> _supportLanguage = null;
        public TextToSpeech _textToSpeech;
        private TaskCompletionSource<Java.Lang.Object> _tcs;       		
        private TaskCompletionSource<Java.Lang.Object> _tcs_speak;            

        public TextToSpeechLib()
        {
            _config = Config.Instance();
        }

        //public void SetEngine(string engine){
            
        //}

        public void SetMainContext(Context contex)
        {
            _mainContext = contex;
        }
		public void SetLang(Locale lang)
		{

			_tcs = null;
			//_tcs = new TaskCompletionSource<Java.Lang.Object>();
			_textToSpeech.SetLanguage(lang);
			//if ((int)await _tcs.Task != (int)OperationResult.Success)

        }

        private void SetPitch()
        {
            _textToSpeech.SetPitch(_config.ttsConfig.SeekPitch);
        }

        private void SetSpeed()
        {
            _textToSpeech.SetSpeechRate(_config.ttsConfig.SeekSpeed);
        }

        public static TextToSpeechLib Instance()
        {
            if (instance == null) instance = new TextToSpeechLib();
            return instance;
        }


        public async Task<TextToSpeech> GetTTS(Context context){            
            if (_textToSpeech != null)
            {
                try{
                    _textToSpeech.Stop();
                    _textToSpeech.Shutdown();
                    _textToSpeech = null;
                }  
                catch 
                {
                    Log.Info(TAG, "Error when GetTTS");
                }
            } 
            _textToSpeech = await CreateTtsAsync(context,this,_config.ttsConfig.EngineNameSelect);
            var locale = new Locale(_config.ttsConfig.LangSelectByTTS);
            SetLang(locale);
            SetPitch();
            SetSpeed();
            if (_textToSpeech != null)
            {
                _textToSpeech.SetOnUtteranceProgressListener(new UtteranceProgressLs(this));
                Log.Info(TAG, "Set callback for textToSpeech");
            }
            return _textToSpeech;
        }

        public async Task ReInitTTS()
        {
            await GetTTS(_mainContext);
        }

        private async Task<TextToSpeech> CreateTtsAsync(Context context,TextToSpeech.IOnInitListener listen,string engine)
        {
            //Reinitilize
            //if (_textToSpeech != null)
            //{
            //    try
            //    {                    
            //        _textToSpeech.Stop();
            //        _textToSpeech.Shutdown();
            //        _textToSpeech = null;
            //    }
            //    catch 
            //    {
            //        /*don't care */
            //    }

            //}
            TextToSpeech tts;
            _tcs = null;
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            string engineName = _config.ttsConfig.EngineNameSelect;
            if (string.IsNullOrEmpty(engine)) tts = new TextToSpeech(context, listen);
            else tts = new TextToSpeech(context, listen, engine);
            if ((int)await _tcs.Task != (int)OperationResult.Success)
            {
                Log.Debug(TAG, "Engine: " + engineName + " failed to initialize.");
                tts = null;
            }           
            _tcs = null;
            return tts;
        }

        //Get Engines support by Device
        public async Task<List<string>> GetEngines(Context c){
            Log.Debug(TAG, "Trying to get Engine: ");
            TextToSpeech tts = await CreateTtsAsync(c, this, null);
            IList<TextToSpeech.EngineInfo> engines = tts.Engines;
            var listNameEngine = new List<string>();
            foreach (var engine in engines)
            {
                listNameEngine.Add(engine.Label);
            }
            if (listNameEngine.Count == 0) listNameEngine.Add("NONE");
            try
            {
                tts.Shutdown();
            }
            catch 
            { 
                /* don't care */ 
            }
            return listNameEngine;
        }

        public async Task<Dictionary<string,string>> GetLanguageSupportByEngineAsync(Context c, string engine)
        {
            Log.Debug(TAG, "GetLanguageSupportByEngineAsync: ");
            if (_supportLanguage == null)
            {

                TextToSpeech _tts = await CreateTtsAsync(c, this, engine);
                ICollection<Locale> langCollect = null;
                List<string> lang = new List<string>();

                if (_tts != null)
                {
                    _supportLanguage = new Dictionary<string, string>();
                    langCollect = _tts.AvailableLanguages;
                    AddToSupportLanguageList(langCollect);
                    //Clear TTS
                    try
                    {
                        _tts.Shutdown();
                    }
                    catch { /* don't care */ }
                    _tts = null;
                }
            }
            return _supportLanguage;

        }

        private void AddToSupportLanguageList(ICollection<Locale> langCollect)
        {
            foreach(var item in langCollect){
                if (!_supportLanguage.ContainsKey(item.Language))
                {
                    _supportLanguage.Add(item.Language,item.DisplayLanguage);
                }
            }
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

		public async Task<int> SpeakMessenger(string msg)
		{
			if (_textToSpeech == null)
			{
                Log.Info(TAG," textToSpeech is null");
				return 0;
			}

			if (_textToSpeech.IsSpeaking)
			{
				try
				{
					_textToSpeech.Stop();
                    _tcs_speak.TrySetResult(new Java.Lang.Integer(1));           
				}
				catch {/*Dont Care*/};
			}
			ICharSequence cs = new Java.Lang.String(msg);
			//textToSpeech.Speak(cs, QueueMode.Flush,null, null);
			//var p = new Dictionary<string, string>();
			Bundle bundle = new Bundle();
			bundle.PutString(TextToSpeech.Engine.KeyParamUtteranceId, "123");
			//p.Add(TextToSpeech.Engine.KeyParamUtteranceId, "ThisUtterance");
            _tcs_speak = null;
            _tcs_speak = new TaskCompletionSource<Java.Lang.Object>();
            _textToSpeech.Speak(cs, QueueMode.Flush, bundle, "this");
            _textToSpeech.PlaySilentUtterance(1000, QueueMode.Add, null);
            int status = (int) await _tcs_speak.Task;
            _tcs_speak = null;
            return status;
			//            //textToSpeech.Speak("this is name of sender", QueueMode.Flush, null);
			//            textToSpeech.PlaySilentUtterance(2000,QueueMode.Add,null);
			//            cs = new Java.Lang.String("this is content of messenge");
			//            textToSpeech.Speak(cs, QueueMode.Add, null, null);
			//textToSpeech.PlaySilentUtterance(2000, QueueMode.Add, null);
		}


		
        public async Task Stop(){
            if (_textToSpeech == null) return;
            try {
                _textToSpeech.Stop();
                _textToSpeech.Shutdown();
            }
            catch (System.Exception e)
            {
                Log.Info(TAG,"Err when Stop TTS "+e);  
            } 
        }

		void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
		{
			Log.Debug(TAG, "OnInit() status = " + status);			
            _tcs.TrySetResult(new Java.Lang.Integer((int)status));

		}

		//Control error when speak       
		public void DoSomething()
		{
            //Log.Info(TAG, "Do something called");
            _tcs_speak.SetResult(new Java.Lang.Integer(1));			
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
