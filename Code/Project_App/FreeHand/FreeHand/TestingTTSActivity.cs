using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Java.Util;
namespace FreeHand
{
    [Activity(Label = "TestingTTSActivity")]
    public class TestingTTSActivity : Activity
    {
        private static string TAG = "TestingTTSActivity";
        private Button btn_getInstance, btn_getEngines, btn_getVoices, btn_Speak;
        private Spinner spn_engines, spn_lang;
        private EditText txt_input;
        private TextToSpeechLib ttsLib;
        private IList<TextToSpeech.EngineInfo> _listEngines;
        private TextToSpeech.EngineInfo _selectEngine;
		private List<string> _listLang;
        private Locale _selectLang;
        //private IList<String> _voices;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_Testing);
            SetBehavior();
            ttsLib = TextToSpeechLib.Instance(this);
            _listLang = new List<string> { };
            btn_getInstance.Click += delegate {
                ttsLib = TextToSpeechLib.Instance(this);
            };

            btn_getEngines.Click += delegate {
                GetEngines();        
			};

            spn_engines.ItemSelected += async (object sender, AdapterView.ItemSelectedEventArgs e)  => 
			{
                _selectEngine = _listEngines[(int)e.Id];
                int i = await GetLanguageSupportByEngine(_selectEngine);
                await ttsLib.CreateTtsAsync();
			};
            spn_lang.ItemSelected += async (object sender, AdapterView.ItemSelectedEventArgs e) => 
            {

                //var s = ((Spinner)sender).GetItemAtPosition((int)e.Id);
                //Log.Debug(TAG, s.ToString());
                //_selectLang = new Locale("en-US");
                //_selectLang = _languageSupport.F(t => t.DisplayLanguage == langAvailable[(int)e.Id]);
                _selectLang = new Locale(_listLang[(int)e.Id]);
                Log.Debug(TAG,_listLang[(int)e.Id]);
                bool status = await ttsLib.SetLang(_selectLang);
            };
            btn_Speak.Click +=  async delegate
            {
                //ttsLib.CreateTtsAsync(this,);
                int i  = await ttsLib.SpeakMessenger("ss");  
                Log.Info(TAG,"result speak "+i.ToString());
            };
            // Create your application here
        }

        protected void GetEngines(){
			_listEngines = ttsLib.GetEngines(this);
			var listNameEngine = new List<string>();
			foreach (var engine in _listEngines)
			{
				listNameEngine.Add(engine.Label);
			}
			if (listNameEngine.Count == 0) listNameEngine.Add("NONE");
			var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, listNameEngine);
			spn_engines.Adapter = adapter;
        }

        protected async Task<int> GetLanguageSupportByEngine(TextToSpeech.EngineInfo engine){
            var languageSupport = await ttsLib.GetLanguageSupportByEngineAsync(this,engine);
            var langDisplay = new List<string>();
            //_listLang.Clear();
            foreach (var lang in languageSupport)
			{
                langDisplay.Add(lang.DisplayLanguage);
                _listLang.Add(lang.ISO3Language);
			}

            if (_listLang.Count == 0) _listLang.Add("NONE");
            //_listLang = _listLang.OrderBy(t => t).Distinct().ToList();
            langDisplay = _listLang.OrderBy(t => t).Distinct().ToList();
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLang);
            spn_lang.Adapter = adapter;
            return 0;
        }
        protected void SetBehavior(){
            btn_Speak = (Button)FindViewById(Resource.Id.btn_speak);
            btn_getVoices = (Button)FindViewById(Resource.Id.btn_getVoices);
            btn_getEngines = (Button)FindViewById(Resource.Id.btn_getEngines);
            btn_getInstance = (Button)FindViewById(Resource.Id.btn_getInstance);

            //spn_voices = (Spinner) FindViewById(Resource.Id.spn_voices);
            spn_engines = (Spinner) FindViewById(Resource.Id.spn_engines);
            spn_lang = (Spinner) FindViewById(Resource.Id.spn_lang);
            txt_input = (EditText) FindViewById(Resource.Id.txt_input);
        }
    }
}
