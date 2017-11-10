
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Views;
using Android.OS;
using Android.Content;
using Android.Speech;
using Android.Media;
using Android.Util;
using Android.Telephony;
namespace FreeHand
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class PhoneCallBroadcastReceiver : BroadcastReceiver,IRecognitionListener
    {
        private static readonly string TAG = "PhoneCallBroadcastReceiver";
        Context _context;
       
        private Config _config;
        TextToSpeechLib _tts;
        SpeechRecognizer _speech;
        string _telephone,_answer;
        STTLib _stt;
        TaskCompletionSource<Java.Lang.Object> _tcs;
        public PhoneCallBroadcastReceiver() { }
        public PhoneCallBroadcastReceiver(Context context)
        {
            Log.Info(TAG, "Contructor");
            _context = context;
            _config = Config.Instance();
            _tts = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();       
        }

        private async Task PhoneCallHanler()
        {
            await _tts.SpeakMessenger("You Has New Call From "+_telephone);
            await Task.Delay(500);
            Log.Info(TAG,"Start Listen");
            //var status = await listenRequest();
            Intent i = new Intent(Intent.ActionMain);
            i.AddCategory(Intent.CategoryHome);
            Intent btnDown = new Intent(Intent.ActionMediaButton).PutExtra(
                Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Down,
                                                   Keycode.Headsethook));
            Intent btnUp = new Intent(Intent.ActionMediaButton).PutExtra(
                Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up,
                                                   Keycode.Headsethook));
            Intent headSetUnPluggedintent = new Intent(Intent.ActionHeadsetPlug);
            headSetUnPluggedintent.PutExtra("state", 0);
            headSetUnPluggedintent.PutExtra("name", "Headset");
            //var tm = (TelephonyManager)_context.GetSystemService(Context.TelecomService);

            _context.SendOrderedBroadcast(btnDown, null);
            _context.SendOrderedBroadcast(btnUp, null);
        }

        private async Task<int> listenRequest()
        {
            
            _speech = SpeechRecognizer.CreateSpeechRecognizer(_context);
            _speech.SetRecognitionListener(this);
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try
            {
                _speech.StartListening(_stt.IntentSTT());
            }
            catch (Exception e) { }
            return (int)await _tcs.Task;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "OnReceive");
            // ensure there is information
            if (intent.Extras != null)
            {
                // get the incoming call state
                string state = intent.GetStringExtra(TelephonyManager.ExtraState);

                // check the current state
                if (state == TelephonyManager.ExtraStateRinging)
                {

                    Log.Info(TAG, "Phone ExtraStateRinging");
                    var x = _config.AudioManage.RingerMode = RingerMode.Vibrate;
                    // read the incoming call telephone number...
                    _telephone = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    if (string.IsNullOrEmpty(_telephone))
                        _telephone = string.Empty;
                    Log.Info(TAG, "Incoming Numer " + _telephone);
                    if (!_config.RunningCallHanle)
                    {
                        _config.RunningCallHanle = true;
                        PhoneCallHanler();

                    }
                }
                else if (state == TelephonyManager.ExtraStateOffhook)
                {
                    // incoming call answer
                    Log.Info(TAG, "Phone ExtraStateOffhook");
                }
                else if (state == TelephonyManager.ExtraStateIdle)
                {
                    _config.RunningCallHanle = false;
                    Log.Info(TAG, "Phone ExtraStateIdle");
                    // incoming call end
                }
            }
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
            _tcs.SetResult(0);

        }

        public void OnRmsChanged(Single single)
        {
            Log.Info(TAG, "onRmsChanged: " + single);
            //progressBarControl();

        }
    }
}
