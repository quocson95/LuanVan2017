using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Util;
using Java.Util;

namespace FreeHand
{
    public class STTLib

    {
        private readonly static string TAG = "STTLib";
        private static STTLib _instance;
        private Intent _intentSTT;
        protected IList<String> _supportLanguage;
        protected Dictionary<string,string> _supportLanguageAsDisplayLanguage;


        TaskCompletionSource<bool> _task;
        private STTLib()
        {
            _intentSTT = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _intentSTT.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            // put a message on the modal dialog
            _intentSTT.PutExtra(RecognizerIntent.ExtraPrompt, "title");

            // if there is more then 1.5s of silence, consider the speech over
            _intentSTT.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 5000);
            //_intentSTT.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            _intentSTT.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            _intentSTT.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

            // you can specify other languages recognised here, for example
            // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
            // if you wish it to recognise the default Locale language and German
            // if you do use another locale, regional dialects may not be recognised very well
            _intentSTT.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
        }
        public Intent IntentSTTCustome(string lang)
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            // intent a message on the modal dialog
            _intentSTT.PutExtra(RecognizerIntent.ExtraPrompt, "title");

            // if there is more then 1.5s of silence, consider the speech over
            intent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 5000);
            //_intentSTT.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            intent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            intent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

            // you can specify other languages recognised here, for example
            // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
            // if you wish it to recognise the default Locale language and German
            // if you do use another locale, regional dialects may not be recognised very well
            Java.Util.Locale locale = new Java.Util.Locale(lang);
            Log.Info(TAG, "lang intent " + lang);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, locale);
            return intent;
        }
        public static STTLib Instance()
        {
            if (_instance == null) _instance = new STTLib();
            return _instance;
        }
        public Intent IntentSTT()
        {
            return _intentSTT;
        }

        public async Task<IList<String>> GetLanguageSupport(Context _context)
        {
            if (_supportLanguage == null)
            {
                _task = new TaskCompletionSource<bool>();
                Intent intent = new Intent(RecognizerIntent.ActionGetLanguageDetails);
                _context.SendOrderedBroadcast(intent, null, new LanguageDetailsChecker(this, _task, _context), null, Result.Ok, null, null);
                await _task.Task;
            }
            return _supportLanguage;
        }

        public void OnGetLanguageSupportReturn(IList<String> supportLanguage){
            this._supportLanguage = supportLanguage;
            _task.SetResult(true);
        }

        public async Task<Dictionary<string, string>> GetLanguageSupportDisplayLanguage(Context _context)
        {            
            if (_supportLanguageAsDisplayLanguage == null)
            {
                _supportLanguageAsDisplayLanguage = new Dictionary<string, string>();
                await GetLanguageSupport(_context);               
                CultureInfo cultureInfo;
                foreach(var item in _supportLanguage){                                        
                    cultureInfo = null;
                    try
                    {
                        cultureInfo = new CultureInfo(item);
                        AddToSupportLanguageList(cultureInfo);
                    }
                    catch{ /*Dont care*/};
                }
            }
            return _supportLanguageAsDisplayLanguage;
        }

        private void AddToSupportLanguageList(CultureInfo cultureInfo)
        {
            string langCode;
            Locale locale;
            langCode = cultureInfo.TwoLetterISOLanguageName;
            if (!_supportLanguageAsDisplayLanguage.ContainsKey(langCode))
            {
                locale = new Locale(langCode);
                _supportLanguageAsDisplayLanguage.Add(langCode,locale.DisplayLanguage);
            }

        }

        public void ClearLangSupport()
        {
            _supportLanguage.Clear();
            _supportLanguageAsDisplayLanguage.Clear();

            _supportLanguage = null;
            _supportLanguageAsDisplayLanguage = null;
        }

    }

   

    public class LanguageDetailsChecker : BroadcastReceiver
    {
        private STTLib _stt;
        private IList<Locale> a = new List<Locale>();
        private TaskCompletionSource<bool> _task;
        private Context _context;
        public LanguageDetailsChecker(STTLib stt, TaskCompletionSource<bool> task, Context context)
        {
            _task = task;
            _context = context;
            _stt = stt;
        }

        private String languagePreference;

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("Get Broadcast ");
            Bundle results = GetResultExtras(true);
            if (results.ContainsKey(RecognizerIntent.ExtraLanguagePreference))
            {
                languagePreference =
                    results.GetString(RecognizerIntent.ExtraLanguagePreference);
            }
            if (results.ContainsKey(RecognizerIntent.ExtraSupportedLanguages))
            {
                IList<String> supportLanguage = results.GetStringArrayList(
                        RecognizerIntent.ExtraSupportedLanguages);     
                _stt.OnGetLanguageSupportReturn(supportLanguage);
            }
        }
    }



}
