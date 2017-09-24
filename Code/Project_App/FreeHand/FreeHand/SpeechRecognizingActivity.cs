
using System;
using System.Threading;
using System.Threading.Tasks;
//using Java.Lang;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Speech;

namespace FreeHand
{
    [Activity(Label = "SpeechRecognizingActivity")]
    public class SpeechRecognizingActivity : Activity, IRecognitionListener
    {
        private static readonly string LOG_TAG = "SpeechRecognizingActivity";
        private SpeechRecognizer speech;
        bool isRun = false;
        private TextView txt_view;
        private ProgressBar processbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_SpeechRecognizingActivity);
            Button btn_speech = (Button)FindViewById(Resource.Id.btn_start);
            txt_view = (TextView)FindViewById(Resource.Id.txtSpeech);
            processbar = (ProgressBar)FindViewById(Resource.Id.progress);
			
            // Create your application here
            speech = SpeechRecognizer.CreateSpeechRecognizer(this);
            speech.SetRecognitionListener(this);
            Intent recognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraLanguagePreference,
                                      "en-US");
			recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 50000);
			recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
			recognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 50000);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            recognizerIntent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
            btn_speech.Click += delegate
            {
                isRun = !isRun;
                speech.StartListening(recognizerIntent);
            };
        }
        async void progressBarControl(){
            await Task.Run(() => {
                processbar.IncrementProgressBy(10);
            });
        }
        public void OnBeginningOfSpeech()
        {
            Log.Info(LOG_TAG, "onBeginningOfSpeech");
        }

        public void OnBufferReceived(Byte[] buffer)
        {
            Log.Info(LOG_TAG, "onBufferReceived: " + buffer);
        }

        public void OnEndOfSpeech()
        {
            Log.Info(LOG_TAG, "onEndOfSpeech");
        }

        public void OnError(SpeechRecognizerError err)
        {
            Log.Debug(LOG_TAG, "FAILED " + err.ToString());
        }

        public void OnEvent(Int32 flag, Bundle bundle)
        {
            Log.Info(LOG_TAG, "onEvent");
        }

        public void OnPartialResults(Bundle bundle)
        {
            Log.Info(LOG_TAG, "onPartialResults");
            var matches = bundle.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
			string text = " ";
            text = matches[0];
            //txt_view.Append(text.ToCharArray(),0,text.Length);
            txt_view.Text = "";
            txt_view.Append(text);

        }

        public void OnReadyForSpeech(Bundle bundel)
        {
            Log.Info(LOG_TAG, "onReadyForSpeech");
        }

        public void OnResults(Bundle bundle)
        {
            Log.Info(LOG_TAG, "onResults");
        }

        public void OnRmsChanged(Single single)
        {
            Log.Info(LOG_TAG, "onRmsChanged: " + single);
            progressBarControl();

        }

    }
}
