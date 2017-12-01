
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Speech;
using Android.Media;
using Android.Util;
using Android.Telephony;
using DeviceMotion.Plugin;

namespace FreeHand.Phone
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class PhoneCallBroadcastReceiver : BroadcastReceiver,IRecognitionListener
    {
        private static readonly string TAG = "PhoneCallBroadcastReceiver";
       
        private Config _config;
        TextToSpeechLib _tts;
        SpeechRecognizer _speech;
        string _telephone,_answer;
        STTLib _stt;
        TaskCompletionSource<Java.Lang.Object> _tcs;    
        public PhoneCallBroadcastReceiver()        
        {
            Log.Info(TAG, "Initializing");           
            _config = Config.Instance();
            _tts = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();       
        }

        private async Task PhoneCallHanler()
        {
            string nameCaller;
            nameCaller = Model.Commom.GetNameFromPhoneNumber(_telephone);

            await _tts.SpeakMessenger("You Has New Call From ");
            await Task.Delay(500);
            await _tts.SpeakMessenger(_telephone);
            await Task.Delay(500);
            await _tts.SpeakMessenger("Name Caller ");
            await Task.Delay(500);
            await _tts.SpeakMessenger(nameCaller);

            Intent buttonDown = new Intent(Intent.ActionMediaButton);
            buttonDown.PutExtra(Intent.ExtraKeyEvent, new KeyEvent(KeyEventActions.Up, Keycode.Headsethook));
            Application.Context.SendOrderedBroadcast(buttonDown, Android.Manifest.Permission.CallPrivileged);
        }

        private async Task<int> listenRequest()
        {
            
            _speech = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
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
            bool acceptCall;
            acceptCall = false;
            Log.Info(TAG, "OnReceive");
            // ensure there is information
            if (intent.Extras != null)
            {
                // get the incoming call state
                string state = intent.GetStringExtra(TelephonyManager.ExtraState);

                // check the current state
                if (state == TelephonyManager.ExtraStateRinging)
                {                    
                    Log.Info(TAG, "Phone ExtraStateRinging");                    ;
                    _config.AudioManage.RingerMode = RingerMode.Vibrate;

                    // read the incoming call telephone number...
                    _telephone = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    if (string.IsNullOrEmpty(_telephone))
                        _telephone = string.Empty;
                    Log.Info(TAG, "Incoming Numer " + _telephone);
                    if (_config.GetPermissionRun(Config.PERMISSION_RUN.PHONE))
                    {             
                          _config.phoneConfig.IsHandlePhoneRunnig = true;
                          PhoneCallHanler();
                    }
                }
                else if (state == TelephonyManager.ExtraStateOffhook)
                {
                    // incoming call answer
                    acceptCall = true;
                    Log.Info(TAG, "Phone ExtraStateOffhook");
                }
                else if (state == TelephonyManager.ExtraStateIdle)
                {
                    _config.phoneConfig.IsHandlePhoneRunnig = false;
                    _config.phoneConfig.MissedCall++;
                    Log.Info(TAG, "Phone ExtraStateIdle,SMS running "+_config.smsConfig.IsHandleSMSRunnig.ToString()  ); 
                    if (_config.smsConfig.IsHandleSMSRunnig)
                    {
                        Log.Info(TAG, "Send Speak Broadcast"); 
                        _config.smsConfig.IsHandleSMSRunnig = false;
                        var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                        context.SendBroadcast(speakSMSIntent);
                    }                  

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
