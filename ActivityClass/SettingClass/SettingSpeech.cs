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
    [Activity(Label = "Setting_Speech", Theme = "@style/MyTheme.Mrkeys")]
    public class SettingSpeech : Activity
    {
        private static readonly string TAG = "Setting_Speech";
        private TTSLib _tts;
        private STTLib _stt;
        private Config _config;
        Model.ScriptLang _scriptLang;
        private Spinner _spin_enigne_master, _spin_lang_speak,_spin_lang_listen;
        private TextView _tv_engine_master, _tv_lang_listen, _labelSpeedValue, _labelPitchValue, _label_lang_voice, _label_speed, _label_pitch,_label_testing,_speak_testing;
        EditText _inputSpeakTxt, _outputListenTxt;
        private SeekBar _seekSpeed, _seekPitch;
        private IList<string> _listLangTTS, _listLangSST;
        //private TaskCompletionSource<Java.Lang.Object> _tcs;  
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                                          .SetDefaultFontPath("Fonts/HELR45W.ttf")
                                          .SetFontAttrId(Resource.Attribute.fontPath)
                                          .Build());

            SetContentView(Resource.Layout.Setting_Speech_Layout);
            
            _tts = TTSLib.Instance();
            _stt = STTLib.Instance();
            _config = Config.Instance();
            _scriptLang = Model.ScriptLang.Instance();
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
            _label_lang_voice = FindViewById<TextView>(Resource.Id.label_lang_voicespeak);
            _seekSpeed = FindViewById<SeekBar>(Resource.Id.seek_speed);
            _seekPitch = FindViewById<SeekBar>(Resource.Id.seek_pitch);
            _label_speed = FindViewById<TextView>(Resource.Id.label_speed);
            _label_pitch = FindViewById<TextView>(Resource.Id.label_pitch);
            _label_testing = FindViewById<TextView>(Resource.Id.test_setting);
            _speak_testing = FindViewById<TextView>(Resource.Id.test_btn_txt);

            //TextView
            _labelPitchValue = FindViewById<TextView>(Resource.Id.value_pitch);
            _labelSpeedValue = FindViewById<TextView>(Resource.Id.value_speed);

            //Test
            _inputSpeakTxt = FindViewById<EditText>(Resource.Id.input_speak);

        }
        private void SetListenerUI()
        {
            SetActionTTSUI();
            SetActionSTTUI();
            SetActionTestSpeak();

        }

        private void SetActionTestSpeak()
        {
            _speak_testing.Click += async (sender, e) => 
            {
                if (!string.IsNullOrEmpty(_inputSpeakTxt.Text))
                {
                    await _tts.GetTTS();
                    await _tts.SpeakMessenger(_inputSpeakTxt.Text);
                }
            };
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
            var listEngine = _config.speech.ListEngineName;
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, listEngine);
            _spin_enigne_master.Adapter = adapter;
            var index = listEngine.IndexOf(_config.speech.EngineNameSelect);
            _spin_enigne_master.SetSelection(index);

            //Set Label Seek
            _seekSpeed.Progress = (int)(_config.speech.SeekSpeed * 20);
            _seekPitch.Progress = (int)(_config.speech.SeekPitch * 20);
            _labelSpeedValue.Text = _config.speech.SeekSpeed.ToString();
            _labelPitchValue.Text = _config.speech.SeekPitch.ToString();


        }
        private void SetDataSTT()
        {
            //if (_config.speech.isSupportSTT)
            //{
            _listLangSST = new List<string>();
            //GetLangSTT();               
            var dictionaryLangSupport = _config.speech.LangSupportBySTT;
            if (dictionaryLangSupport != null)
            {
                foreach (var item in dictionaryLangSupport)
                {
                    _listLangSST.Add(item.Value);
                }
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangSST);
                _spin_lang_listen.Adapter = adapter;
                var index = _config.speech.LangSupportBySTT.Keys.ToList().IndexOf(_config.speech.LangSelectBySTT);
                _spin_lang_listen.SetSelection(index);
            }
            //}
        }
               

        private void SetActionTTSUI()
        {            
            _spin_enigne_master.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {                
                _config.speech.EngineNameSelect = _config.speech.ListEngineName[(int)e.Id];
                Log.Info(TAG, "Select Engine {0}",_config.speech.EngineNameSelect);
                _listLangTTS = new List<string>();
                var dictionaryLangSupport = _config.speech.LangSupportByTTS;

                foreach (var it in dictionaryLangSupport)
                {
                    _listLangTTS.Add(it.Value);
                }
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _listLangTTS);
                _spin_lang_speak.Adapter = adapter;
                //var index = supportLanguageName.IndexOf(_config.ttsConfig.LangSelectByTTS);
                var index = _config.speech.LangSupportByTTS.Keys.ToList().IndexOf(_config.speech.LangSelectByTTS);
                _spin_lang_speak.SetSelection(index);

                _config.IsUpdateCfg = true;

            };

            _spin_lang_speak.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Select Lang Speak id" + e.Id.ToString());
                _config.speech.LangSelectByTTS = _config.speech.LangSupportByTTS.Where(pair => pair.Value == _listLangTTS[(int)e.Id])
                    .Select(pair => pair.Key)
                    .FirstOrDefault();

                _config.IsUpdateCfg = true;

                if (_config.speech.LangSelectByTTS.Equals("vi"))
                    _scriptLang.vi_VN();
                else _scriptLang.Default();
            };

            _seekPitch.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                if (e.FromUser)
                {
                    var var1 = e.Progress / 2;
                    _config.speech.SeekPitch = ((float)((float)var1 / 10.0));
                    _labelPitchValue.Text = _config.speech.SeekPitch.ToString();

                    _config.IsUpdateCfg = true;
                }
            };

            _seekSpeed.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
                if (e.FromUser)
                {
                    var var1 = e.Progress / 2;
                    _config.speech.SeekSpeed = ((float)((float)var1 / 10.0));
                    _labelSpeedValue.Text = _config.speech.SeekSpeed.ToString();
                    _config.IsUpdateCfg = true;

                }

            };

        }

        private void SetActionSTTUI()
        {            

            _spin_lang_listen.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Log.Info(TAG, "Selected Lang Listen id " + e.Id.ToString());
                _config.speech.LangSelectBySTT = _config.speech.LangSupportBySTT.Where(pair => pair.Value == _listLangSST[(int)e.Id])
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

        protected override void OnResume()
        {
            base.OnResume();
            Log.Info(TAG,"OnResume");
            //private TextView _tv_engine_master, _tv_lang_listen, _labelSpeedValue, _labelPitchValue;   
            SetLanguage();
        }

        void SetLanguage()
        {
            _tv_engine_master.SetText(Resource.String.label_setting_speech_voicespeak);
            _label_lang_voice.SetText(Resource.String.label_setting_speech_voicespeak_language);
            _label_speed.SetText(Resource.String.label_setting_speech_voicespeak_speed);
            _label_pitch.SetText(Resource.String.label_setting_speech_voicespeak_spitch);
            _tv_lang_listen.SetText(Resource.String.label_setting_speech_listenvoice);
            _tv_lang_listen.SetText(Resource.String.label_setting_speech_listenvoice_language);
            _label_testing.SetText(Resource.String.label_setting_speech_testvoice);
            _speak_testing.SetText(Resource.String.label_setting_speech_testvoice_speak);
        }

        protected override void OnStop()
        {
            Log.Info(TAG, "OnStop");
            _config.Save();
            _tts.ShutDown();
            base.OnStop();
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            Context c = Model.LocaleHelper.onAttach(@base);
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(c));
        }

    }

}
