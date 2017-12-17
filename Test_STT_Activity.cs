
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Speech;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using System.Threading.Tasks;
using Android.Widget;

namespace FreeHand
{
    [Activity(Label = "Test_STT_Activity")]
    public class Test_STT_Activity : Activity, IRecognitionListener
    {
        private readonly static string TAG = "Test_STT_Activity";
        private STTLib _stt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.STT_Layout);
            Button btn_ins = (Button)FindViewById(Resource.Id.btn_ins);
            Button btn_listen = (Button)FindViewById(Resource.Id.btn_lis);
            TextView txt_view = (TextView)FindViewById(Resource.Id.txt_view);
            // Create your application here
            btn_ins.Click += delegate
            {
                _stt = STTLib.Instance();
            };

            btn_listen.Click += delegate
            {
                //StartActivityForResult(_stt.IntentSTT(),0);
                SpeechRecognizer speech = SpeechRecognizer.CreateSpeechRecognizer(this);
                speech.SetRecognitionListener(this);
                speech.StartListening(_stt.IntentSTT());
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == 0)
            {
                if (resultVal == Result.Ok)
                {
                    Log.Info(TAG, "rec");
                }
                base.OnActivityResult(requestCode, resultVal, data);
            }
        }
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
            Log.Info(TAG, "onResults");
        }

        public void OnRmsChanged(Single single)
        {
            Log.Info(TAG, "onRmsChanged: " + single);
            //progressBarControl();

        }
    }
}
