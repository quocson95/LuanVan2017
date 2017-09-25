
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Speech.Tts;
using Java.Util;
using System.Threading.Tasks;
namespace FreeHand
{
    [Activity(Label = "TestingTTSActivity")]
    public class TestingTTSActivity : Activity
    {
        private Button btn_getInstance, btn_getEngines, btn_getVoices, btn_Speak;
        private Spinner spn_engines, spn_voices, spn_lang;
        private EditText txt_input;
        private TextToSpeechLib ttsLib;
        private IList<TextToSpeech.EngineInfo> _listEngines;
        private ICollection<Locale> _languageSupport;
        private IList<String> _voices;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_Testing);
            SetBehavior();
            btn_getInstance.Click += delegate {
                ttsLib = TextToSpeechLib.Instance(this);
            };

            btn_getEngines.Click += delegate {
                GetEngines();        
			};

            spn_engines.ItemSelected += async (object sender, AdapterView.ItemSelectedEventArgs e)  => 
			{
                TextToSpeech.EngineInfo engine = _listEngines[(int)e.Id];
                int i = await GetLanguageSupportByEngine(engine);
			};
            spn_lang.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {

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
            _languageSupport = await ttsLib.GetLanguageSupportByEngineAsync(this,engine);
            var listLang = new List<string>();
            foreach (var lang in _languageSupport)
			{
                listLang.Add(lang.DisplayLanguage);
			}
            if (listLang.Count == 0) listLang.Add("NONE");
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, listLang);
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
