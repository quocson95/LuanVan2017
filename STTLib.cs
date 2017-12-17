
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Speech;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace FreeHand
{
    public class STTLib

    {
        private readonly static string TAG = "STTLib";
        private static STTLib _instance;
        private Intent _intentSTT;
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
            Log.Info(TAG,"lang intent "+lang);
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
    }
    //	public class IRecognitionLs : IRecognitionListener
    //	{
    //		private static readonly string TAG = "IRecognitionLs";

    //		public IntPtr Handle => throw new NotImplementedException();

    //		public void Dispose()
    //		{
    //			throw new NotImplementedException();
    //		}

    //		public void OnBeginningOfSpeech()
    //		{
    //			Log.Info(TAG, "onBeginningOfSpeech");
    //		}

    //		public void OnBufferReceived(Byte[] buffer)
    //		{
    //			Log.Info(TAG, "onBufferReceived: " + buffer);
    //		}

    //		public void OnEndOfSpeech()
    //		{
    //			Log.Info(TAG, "onEndOfSpeech");
    //		}

    //		public void OnError(SpeechRecognizerError err)
    //		{
    //			Log.Debug(TAG, "FAILED " + err.ToString());
    //		}

    //		public void OnEvent(Int32 flag, Bundle bundle)
    //		{
    //			Log.Info(TAG, "onEvent");
    //		}

    //		public void OnPartialResults(Bundle bundle)
    //		{
    //			Log.Info(TAG, "onPartialResults");
    //		}

    //		public void OnReadyForSpeech(Bundle bundel)
    //		{
    //			Log.Info(TAG, "onReadyForSpeech");
    //		}

    //		public void OnResults(Bundle bundle)
    //		{			
    //            IList<string> result = bundle.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
    //            Log.Info(TAG, "onResults" + result[0]);
    //		}

    //		public void OnRmsChanged(Single single)
    //		{
    //			Log.Info(TAG, "onRmsChanged: " + single);
    //			//progressBarControl();

    //		}
}
