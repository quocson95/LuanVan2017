
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Speech.Tts;
using Android.Speech;
using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;
using Android.Widget;
using Calligraphy;
using Android.Util;
using Java.Util;
namespace FreeHand
{
    [Activity(Label = "Setting_Speech",Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class Setting_Speech : Activity
    {
        private static readonly string TAG = "Setting_Speech";
        private TextToSpeechLib _tts;
        private Config _config;      
        private string _selectEngine, _selectLang;
        private Spinner spn_eng,spn_lang;
        private bool _changeEngine;
        Locale tmp;
        private List<string> _listEngine,_listLangDisplayName,_listLangCode;
        private ICollection<Locale> _listLangCollect;
        private TaskCompletionSource<Java.Lang.Object> _tcs;  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Load Font
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("Fonts/HELR45W.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
            SetContentView(Resource.Layout.Speech);
            _changeEngine = false;
            _tts = TextToSpeechLib.Instance();
            _config = Config.Instance();
            // Create your application here
            Button btn_engine = FindViewById<Button>(Resource.Id.item_1);
            Button btn_test = FindViewById<Button>(Resource.Id.item_3);
            Button btn_ok = FindViewById<Button>(Resource.Id.item_4);
            spn_eng = FindViewById<Spinner>(Resource.Id.spinner_engine);
            spn_lang = FindViewById<Spinner>(Resource.Id.spinner_lang);

            btn_engine.Click += async delegate {
                await BtnEngineClick();
                //GetLangSTT();
               

            };

            btn_test.Click += async delegate {
                var locale = new Locale(_config.ttsConfig.Lang);
                if (_changeEngine) await _tts.GetTTS(this);
                _changeEngine = false;
                //await SampleText();
                /*TODO
                 * ADD funtion get messenger sample for test tts config
                 */

                _tts.SetLang(locale);
                _config.smsConfig.IsHandleSMSRunnig = false;
                var k = await _tts.SpeakMessenger("this is messenger test");
            };

            spn_eng.ItemSelected += async (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {                
                Log.Info(TAG, "Select Engine");
                _changeEngine = true;
                _selectEngine = _listEngine[(int)e.Id];
                _listLangDisplayName = new List<string>();
                _listLangCode = new List<string>();
                _listLangCollect = await _tts.GetLanguageSupportByEngineAsync(this, _selectEngine);

                foreach (var it in _listLangCollect)
                {
                    tmp = it;
                    _listLangDisplayName.Add(it.DisplayName);
                    _listLangCode.Add(it.ISO3Language);
                }
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangDisplayName);
                spn_lang.Adapter = adapter;

            };

            spn_lang.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {      
                _selectLang = _listLangCode[(int)e.Id];

            };
            btn_ok.Click += delegate {
                _config.ttsConfig.Lang = _selectLang;
                _config.ttsConfig.EngineName = _selectEngine;
                _config.UpdateConfig = true;
                _config.WriteConfig = true;
            };
        }

        private async Task BtnEngineClick()
        {
            Log.Info(TAG,"Get Engine");
            _listEngine = await _tts.GetEngines(this);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listEngine);
            spn_eng.Adapter = adapter;
        }   
        public async Task<int> SampleText()
        {
            _tcs = null;

            Intent intent = new Intent(TextToSpeech.Engine.ActionGetSampleText);

            intent.PutExtra("language", _config.ttsConfig.Lang);
            intent.PutExtra("country", tmp.Country);
            intent.PutExtra("variant", tmp.Variant);
            intent.SetPackage(_config.ttsConfig.EngineName);
            _tcs = null;
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try
            {
                
                StartActivityForResult(intent, 10);
                await _tcs.Task;
            }
            catch (ActivityNotFoundException ex)
            {
                Log.Info(TAG, "Failed to get sample text, no activity found for " + intent + ")");
            }
            return 0;
        }
        public void GetLangSTT()
        {
            Intent intent = new Intent(RecognizerIntent.ActionGetLanguageDetails);

            StartActivityForResult(intent, 10);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 10)
            {
                Log.Info(TAG,"OnActivityResult");
            }
        }

    }
}
