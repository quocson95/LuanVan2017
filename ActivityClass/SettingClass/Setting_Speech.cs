using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Calligraphy;

namespace FreeHand
{
    [Activity(Label = "Setting_Speech",Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class Setting_Speech : Activity
    {
        private static readonly string TAG = "Setting_Speech";
        private TextToSpeechLib _tts;
        private STTLib _stt;
        private Config _config;      
        private Spinner _spin_enigne_master, _spin_lang_speak,_spin_lang_listen;
        private TextView _tv_engine_master, _tv_lang_listen;       
        private SeekBar _seekSpeed, _seekPitch;
        private int _seekSpeedValue, _seekPitchValue;
        private TextView _labelSpeedValue, _labelPitchValue;
        private IList<string> _listLangTTS, _listLangSST;
        //private TaskCompletionSource<Java.Lang.Object> _tcs;  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Load Font
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("Fonts/HELR45W.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());
            SetContentView(Resource.Layout.Setting_Speech_Layout);
            _tts = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();
            _config = Config.Instance();
            // Create your application here
            //Task configWork = new Task(() =>
            //{
                InitUI();
                SetListenerUI();
                InitDataUI();
            //});
            //configWork.Start();
            //spn_lang = FindViewById<Spinner>(Resource.Id.spinner_lang);



            //spn_lang.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            //{      
            //    _selectLang = _listLangCode[(int)e.Id];

            //};
            //btn_ok.Click += delegate {
            //    _config.ttsConfig.Lang = _selectLang;
            //    _config.ttsConfig.EngineName = _selectEngine;
            //    _config.UpdateConfig = true;
            //    _config.WriteConfig = true;
            //};
        }

        private void InitUI()
        {
            _tv_engine_master = FindViewById<TextView>(Resource.Id.btn_engine_master);
            _tv_lang_listen = FindViewById<TextView>(Resource.Id.label_listen_lang);
            _spin_enigne_master = FindViewById<Spinner>(Resource.Id.spinner_engine_master);
            _spin_lang_speak = FindViewById<Spinner>(Resource.Id.spinner_speak_lang);
            _spin_lang_listen = FindViewById<Spinner>(Resource.Id.spinner_listen_lang);

            _seekSpeed = FindViewById<SeekBar>(Resource.Id.seek_speed);
            _seekPitch = FindViewById<SeekBar>(Resource.Id.seek_pitch);

            //TextView
            _labelPitchValue = FindViewById<TextView>(Resource.Id.value_pitch);
            _labelSpeedValue = FindViewById<TextView>(Resource.Id.value_speed);

            //Init Value

        }
        private void SetListenerUI()
        {
            SetActionTTSUI();
            SetActionSTTUI();

        }
        private void InitDataUI()
        {
            //Engine
            SetDataTTS();
            SetDataSTT();

                       
        }

        private void  SetDataTTS()
        {
            //Engine
            var listEngine = _config.ttsConfig.ListEngineName;
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, listEngine);
            _spin_enigne_master.Adapter = adapter;
            var index = listEngine.IndexOf(_config.ttsConfig.LangSelectByTTS);
            _spin_enigne_master.SetSelection(index);

            //Set Label Seek
            _seekSpeed.Progress = (int)(_config.ttsConfig.SeekSpeed * 20);
            _seekPitch.Progress = (int)(_config.ttsConfig.SeekPitch * 20);
            _labelSpeedValue.Text = _config.ttsConfig.SeekSpeed.ToString();
            _labelPitchValue.Text = _config.ttsConfig.SeekPitch.ToString();


        }
        private void SetDataSTT()
        {
            _listLangSST = new List<string>();
            //GetLangSTT();               
            var dictionaryLangSupport = _config.ttsConfig.LangSupportBySTT;

            foreach (var item in dictionaryLangSupport)
            {
                _listLangSST.Add(item.Value);
            }
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangSST);
            _spin_lang_listen.Adapter = adapter;
            var index = _config.ttsConfig.LangSupportBySTT.Keys.ToList().IndexOf(_config.ttsConfig.LangSelectBySTT);
            _spin_lang_listen.SetSelection(index);
        }
               

        private void SetActionTTSUI()
        {            
            _spin_enigne_master.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Select Engine");               
                _listLangTTS = new List<string>();
                var dictionaryLangSupport = _config.ttsConfig.LangSupportByTTS;

                foreach (var it in dictionaryLangSupport)
                {
                    _listLangTTS.Add(it.Value);
                }
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangTTS);
                _spin_lang_speak.Adapter = adapter;
                //var index = supportLanguageName.IndexOf(_config.ttsConfig.LangSelectByTTS);
                var index = _config.ttsConfig.LangSupportByTTS.Keys.ToList().IndexOf(_config.ttsConfig.LangSelectByTTS);
                _spin_lang_speak.SetSelection(index);

                _config.UpdateConfig = true;

            };

            _spin_lang_speak.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Select Lang Speak id" + e.Id.ToString());
                _config.ttsConfig.LangSelectByTTS = _config.ttsConfig.LangSupportByTTS.Where(pair => pair.Value == _listLangTTS[(int)e.Id])
                    .Select(pair => pair.Key)
                    .FirstOrDefault();

                _config.UpdateConfig = true;
            };

            _seekPitch.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                if (e.FromUser)
                {
                    var var1 = e.Progress / 2;
                    _config.ttsConfig.SeekPitch = ((float)((float)var1 / 10.0));
                    _labelPitchValue.Text = _config.ttsConfig.SeekPitch.ToString();

                    _config.UpdateConfig = true;
                }
            };

            _seekSpeed.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                if (e.FromUser)
                {
                    var var1 = e.Progress / 2;
                    _config.ttsConfig.SeekSpeed = ((float)((float)var1 / 10.0));
                    _labelSpeedValue.Text = _config.ttsConfig.SeekSpeed.ToString();
                    _config.UpdateConfig = true;

                }

            };

        }

        private void SetActionSTTUI()
        {            

            _spin_lang_listen.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Selected Lang Listen id " + e.Id.ToString());
                _config.ttsConfig.LangSelectBySTT = _config.ttsConfig.LangSupportBySTT.Where(pair => pair.Value == _listLangSST[(int)e.Id])
                    .Select(pair => pair.Key)
                    .FirstOrDefault();
            };
        }
        private void BtnEngineClick()
        {
            Log.Info(TAG,"Get Engine");
        }   

        //public void GetLangSTT()
        //{
        //    Intent intent = new Intent(RecognizerIntent.ActionGetLanguageDetails);

        //    this.SendOrderedBroadcast(intent,null, new LanguageDetailsChecker(spin_lang_listen,this), null, Result.Ok, null, null);
        //}



        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 10)
            {
                Log.Info(TAG,"OnActivityResult");
            }
        }

        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.Save();
            base.OnStop();
        }

    }

}
