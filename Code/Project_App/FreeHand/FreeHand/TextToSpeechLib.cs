using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Java.Util;
using Android.Util;
using Xamarin.Android;

namespace FreeHand
{
    public class TextToSpeechLib : Activity, TextToSpeech.IOnInitListener
    {
        private static readonly string TAG = "TextToSpeechLib";
        private static readonly Int16 REQUEST_CODE = 1995, LANG_REQUEST = 2017;
        private static TextToSpeechLib instance = null; //Singleton obj
        private TextToSpeech textToSpeech;
        private Context context;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private TextToSpeechLib(Context contect)
        {
            this.context = context;
        }

        public static TextToSpeechLib Instance(Context c)
        {
            if (instance == null) instance = new TextToSpeechLib(c);
            return instance;
        }

        public async Task<TextToSpeech> CreateTtsAsync(Context context, string engName)
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
            textToSpeech = new TextToSpeech(context, this, engName);
            if ((int)await _tcs.Task != (int)OperationResult.Success)
            {
                Log.Debug(TAG, "Engine: " + engName + " failed to initialize.");
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
            catch (Exception e)
            {
                Log.Debug(TAG, "StartActivityForResult() exception: " + e);
            }
            _tcs = null;
            return data;
        }

        public async Task<ICollection<Locale>>  GetLanguageSupportByEngineAsync(Context c, TextToSpeech.EngineInfo engine){
            Log.Debug(TAG, "Trying to create TTS Engine: ");
            TextToSpeech _tts = await CreateTtsAsync(c, engine.Name);
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


        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            Log.Debug(TAG, "OnInit() status = " + status);
            _tcs.SetResult(new Java.Lang.Integer((int)status));

        }
    }
}
