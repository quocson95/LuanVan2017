using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Util;
using FreeHand.Model;

namespace FreeHand.Messenge.Service
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class SpeakMessengeBroadcastReceiver : BroadcastReceiver,IRecognitionListener
    {
        private static readonly string TAG = typeof(SpeakMessengeBroadcastReceiver).FullName;
        private Model.MessengeQueue _messengeQueue;
        private TextToSpeechLib _ttsLib;
        private STTLib _stt;
        private Config _config;
        private SpeechRecognizer _speech;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private string _answer = "";
        bool stop;
        public SpeakMessengeBroadcastReceiver(){         
            Log.Info(TAG, "Initializing");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
            _ttsLib = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "OnReceive: "+intent.Action);

            if (!_config.smsConfig.IsHandleSMSRunnig)
                SMSHandleSpeak(context);            
        }

        public void Stop()
        {
            Log.Info(TAG, "Stop: ");
            stop = true;
            if (_speech != null)
            {               
                _speech.Cancel();
                _speech.UnregisterFromRuntime();

            }
            _ttsLib.Stop();
            _config.smsConfig.Clean();
        }
        private async void SMSHandleSpeak(Context context)
        {           
            _config.smsConfig.IsHandleSMSRunnig = true;
            stop = false;
            Log.Info(TAG,"SMS Handle status "+_config.smsConfig.IsHandleSMSRunnig.ToString());

            Model.IMessengeData messengeData = null;
           
            int try_listen = 3;
            if (_config.smsConfig.StateSMS != Config.STATE_SMS.IDLE 
                && _config.smsConfig.StateSMS != Config.STATE_SMS.DONE) {
                Log.Info(TAG,"   Continuous Speak Messenger");
                await SpeakMsg(" ");
                await Task.Delay(2000);
                await SpeakMsg("Continuos Speak Previous Messenger");

            }
            messengeData = GetMessege(_config.smsConfig.StateSMS);

            while (!EmptyMessenge() &&
                   _config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE) && !stop)                  
            {
                if (_config.UpdateConfig)
                {
                    _config.UpdateConfig = false;
                    await _ttsLib.ReInitTTS();
                }


                Log.Info(TAG, "Speak Messenge State " + _config.smsConfig.StateSMS);
                switch (_config.smsConfig.StateSMS)
                {
                    case Config.STATE_SMS.IDLE:
                        _config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_NUMBER;
                        try_listen = 3;
                        break;

                    case Config.STATE_SMS.SPEAK_NUMBER:
                        await StateSpeakNumber(messengeData);
                        break;
                    case Config.STATE_SMS.SPEAK_NAME_SENDER:
                        await StateSpeakName(messengeData);
                        break;
                    case Config.STATE_SMS.SPEAK_CONTENT:
                        await StateSpeakContent(messengeData);
                        break;
                    case Config.STATE_SMS.READY_REPLY:
                        await StateReadlyReply();
                        break;
                    case Config.STATE_SMS.LISTENT_REQUEST_ANSWER:
                        await StateListenRequest(context,try_listen);
                        try_listen--;
                        break;
                    case Config.STATE_SMS.LISTEN_CONTENT_ANSWER:
                        _config.smsConfig.StateSMS = Config.STATE_SMS.DONE;
                        break;                    
                    case Config.STATE_SMS.DONE:
                        
                        messengeData = GetMessege(Config.STATE_SMS.DONE);
                        if (messengeData != null) 
                            _config.smsConfig.StateSMS = Config.STATE_SMS.IDLE;                        
                        break;
                }

            }
            //End While
            if (_config.smsConfig.StateSMS == Config.STATE_SMS.DONE)
            {
                _config.smsConfig.StateSMS = Config.STATE_SMS.IDLE;
                _config.smsConfig.IsHandleSMSRunnig = false;
            }                

        }

        private async Task StateListenRequest(Context context,int try_listen)
        {
            if (_config.smsConfig.PrevAllowAutoReply)
            {
                if (_config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE))
                {
                    int status;
                    status = -1;
                    status = await listenRequest(context);
                    if (status == 0)
                        _config.smsConfig.StateSMS = Config.STATE_SMS.LISTEN_CONTENT_ANSWER;
                    else if (status != 0 && try_listen > 0)
                    {                        
                        await SpeakMsg("I can't hear you, please try again");
                        await Task.Delay(500);
                        _config.smsConfig.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                    }
                    else _config.smsConfig.StateSMS = Config.STATE_SMS.DONE;

                }
            }
            else _config.smsConfig.StateSMS = Config.STATE_SMS.DONE;
        }

        private async Task StateReadlyReply()
        {
            if (_config.smsConfig.AllowAutoReply)
            {
                await SpeakMsg("Do you want reply");
            }
            _config.smsConfig.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
        }

        //StateSpeakContent
        private async Task StateSpeakContent(IMessengeData messengeData)
        {
            if (_config.smsConfig.AllowSpeakContent)
            {
                await SpeakMsg("Content of Messenge");
                await SpeakMsg(messengeData.GetMessengeContent());
            }
            _config.smsConfig.StateSMS = Config.STATE_SMS.READY_REPLY;
        }

        //StateSpeakName
        private async Task StateSpeakName(IMessengeData messengeData)
        {
            if (_config.smsConfig.AllowSpeakName)
            {
                await SpeakMsg("Name Sender ");
                await SpeakMsg(messengeData.GetNameSender());
            }
            _config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_CONTENT;
        }

        //StateSpeakNumber
        private async Task StateSpeakNumber(IMessengeData messengeData)
        {
            if (_config.smsConfig.AllowSpeakNumber)
            {
                await SpeakMsg("you get a new messege ");
                await SpeakMsg("From ");
                await SpeakMsg(messengeData.GetAddrSender());
            }
            _config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_NAME_SENDER;
        }

        private bool EmptyMessenge(){
            if (_messengeQueue.Empty() && _config.smsConfig.MessengeBackUp == null) return true;
            return false;
        }
        private async Task<int> listenRequest(Context context)
        {
            _speech = SpeechRecognizer.CreateSpeechRecognizer(context);
            _speech.SetRecognitionListener(this);
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try
            {
                _speech.StartListening(_stt.IntentSTT());
            }
            catch { /* Dont care */}
            return (int)await _tcs.Task;
        }


        private async Task SpeakContentSMS(Model.IMessengeData messengeData, Config.STATE_SMS state)
        {
            //messengeData.Reply("aaa");
            if (state == Config.STATE_SMS.SPEAK_NUMBER)
            {
                Log.Info(TAG, messengeData.GetAddrSender());
                await SpeakMsg("Get new SMS messeger ");
                await SpeakMsg("From ");
                await SpeakMsg(messengeData.GetAddrSender());
                state = Config.STATE_SMS.SPEAK_NAME_SENDER;
            }
            if (state == Config.STATE_SMS.SPEAK_NAME_SENDER)
            {
                await SpeakMsg("Name Sender ");
                await SpeakMsg(messengeData.GetNameSender());
                state = Config.STATE_SMS.SPEAK_CONTENT;
            }
            if (state == Config.STATE_SMS.SPEAK_CONTENT)
            {
                await SpeakMsg("Content of Messenger");
                //Log.Info(TAG, messengeData.GetMessengeContent());
                await SpeakMsg(messengeData.GetMessengeContent());
                state = Config.STATE_SMS.READY_REPLY;
            }
            await Task.Delay(500);
            if (state == Config.STATE_SMS.READY_REPLY)
            {
                await SpeakMsg("Do you want reply");
            }

        }

        private Model.IMessengeData GetMessege(Config.STATE_SMS state)
        {
            Model.IMessengeData result;
            result = null;
            switch (state)
            {
                case Config.STATE_SMS.IDLE:
                case Config.STATE_SMS.DONE:
                    if (!_messengeQueue.Empty())
                        result = _messengeQueue.DequeueMessengeQueue();
                    else result = null;

                    _config.smsConfig.MessengeBackUp = result;
                    break;
                default:
                    result = _config.smsConfig.MessengeBackUp;
                    break;
            }
            return result;

        }


        private async Task SpeakMsg(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            int status = await _ttsLib.SpeakMessenger(msg);
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
