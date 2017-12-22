
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace FreeHand.SpeechLibrary
{
    [Activity(Label = "STTActivity")]
    public class STTActivity : Activity,IRecognitionListener
    {
        Spinner _spin_lang_listen;
        TextView txt;
        Config _config;
        Button btn, getlang;
        string _answer;
        private SpeechRecognizer _speech;
        private STTLib _stt;
        private TaskCompletionSource<Java.Lang.Object> _tcs;

        private IList<string> _listLangSST;
        static readonly string TAG = typeof(STTActivity).FullName;
        protected override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Test_STTLayout);
            // Create your application here
           

            _spin_lang_listen = FindViewById<Spinner>(Resource.Id.lang);
            txt = FindViewById<TextView>(Resource.Id.textView1);
            btn = FindViewById<Button>(Resource.Id.button1);
            getlang = FindViewById<Button>(Resource.Id.button2);
            _config = Config.Instance();
            _stt = STTLib.Instance();

            _spin_lang_listen.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Selected Lang Listen id " + e.Id.ToString());
                _config.speech.LangSelectBySTT = _config.speech.LangSupportBySTT.Where(pair => pair.Value == _listLangSST[(int)e.Id])
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
            };

            btn.Click +=async delegate {
                await listenRequest(this);
            };

            getlang.Click += async delegate {
                await _stt.GetLanguageSupportDisplayLanguage(this);
            };
            _listLangSST = new List<string>();
            //GetLangSTT();               
            var dictionaryLangSupport = _config.speech.LangSupportBySTT;

            foreach (var item in dictionaryLangSupport)
            {
                _listLangSST.Add(item.Value);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangSST);
            _spin_lang_listen.Adapter = adapter;
            var index = _config.speech.LangSupportBySTT.Keys.ToList().IndexOf(_config.speech.LangSelectBySTT);
            _spin_lang_listen.SetSelection(index);
        }

        private async Task<int> listenRequest(Context context)
        {
            _speech = SpeechRecognizer.CreateSpeechRecognizer(context);
            _speech.SetRecognitionListener(this);
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try
            {
                var locale = new Locale(_config.speech.LangSelectBySTT);
                _speech.StartListening(_stt.IntentSTTCustome(_config.speech.LangSelectBySTT));
            }
            catch { /* Dont care */}
            return (int)await _tcs.Task;
        }


        //Speech Interface Implement
        public void OnBeginningOfSpeech()
        {
            Log.Info(TAG, "onBeginningOfSpeech");
        }

        public void OnBufferReceived(Byte[] buffer)
        {
            Log.Info(TAG, "onBufferReceived: " + buffer);
        }

        public void OnEndOfSpeech()
        {
            Log.Info(TAG, "onEndOfSpeech");
        }

        public void OnError(SpeechRecognizerError err)
        {
            Log.Debug(TAG, "FAILED " + err.ToString());
            _tcs.TrySetResult(-1);


        }

        public void OnEvent(Int32 flag, Bundle bundle)
        {
            Log.Info(TAG, "onEvent");
        }

        public void OnPartialResults(Bundle bundle)
        {
            Log.Info(TAG, "onPartialResults");
        }

        public void OnReadyForSpeech(Bundle bundel)
        {
            Log.Info(TAG, "onReadyForSpeech");
        }

        public void OnResults(Bundle bundle)
        {
            IList<string> result = bundle.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            Log.Info(TAG, "onResults ");
            _answer = result[0];
            txt.Text = _answer;
            _tcs.SetResult(0);

        }

        public void OnRmsChanged(Single single)
        {
            Log.Info(TAG, "onRmsChanged: " + single);
            //progressBarControl();

        }
    }
}
