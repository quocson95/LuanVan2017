using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Util;

namespace FreeHand.Messenge
{
    [BroadcastReceiver]
    public class SpeakMessengeBroadcastReceiver : BroadcastReceiver,IRecognitionListener
    {
        private static readonly string TAG = "SpeakMessengeBroadcastReceiver";
        Context _context;
        private Model.MessengeQueue _messengeQueue;
        private TextToSpeechLib _ttsLib;
        private STTLib _stt;
        private Config _config;
        private SpeechRecognizer _speech;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private string _answer = "";
        private static readonly string IntentAction = "FreeHand.QueueMessenge.Invoke";
        public SpeakMessengeBroadcastReceiver(){}
        public SpeakMessengeBroadcastReceiver(Context context)
        {
            _context = context;
            Log.Info(TAG, "Contructor");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
            _ttsLib = TextToSpeechLib.Instance();
            _stt = STTLib.Instance();
        }
        private async Task SMSHandleSpeak()
        {
            _config.smsConfig.IsHandleSMSRunnig = true;
            Log.Info(TAG,"SMS Handle status "+_config.smsConfig.IsHandleSMSRunnig.ToString());

            Model.IMessengeData messengeData = null;
            int status;
            status = -1;
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
                   _config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE))                  
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
                        await SpeakMsg("you get a new messege ");
                        await SpeakMsg("From ");
                        await SpeakMsg(messengeData.GetAddrSender());
                        _config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_NAME_SENDER;
                        break;
                    case Config.STATE_SMS.SPEAK_NAME_SENDER:
                        await SpeakMsg("Name Sender ");
                        await SpeakMsg(messengeData.GetNameSender());
                        _config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_CONTENT;
                        break;
                    case Config.STATE_SMS.SPEAK_CONTENT:
                        await SpeakMsg("Content of Messenge");
                        await SpeakMsg(messengeData.GetMessengeContent());
                        _config.smsConfig.StateSMS = Config.STATE_SMS.READY_REPLY;
                        break;
                    case Config.STATE_SMS.READY_REPLY:
                        await SpeakMsg("Do you want reply");
                        _config.smsConfig.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                        break;
                    case Config.STATE_SMS.LISTENT_REQUEST_ANSWER:
                        if (_config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE))
                        {
                            status = await listenRequest();
                            if (status == 0)
                                _config.smsConfig.StateSMS = Config.STATE_SMS.LISTEN_CONTENT_ANSWER;
                            else if (status != 0 && try_listen > 0)
                            {
                                try_listen--;
                                await SpeakMsg("I can't hear you, please try again");
                                await Task.Delay(500);
                                _config.smsConfig.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                            }
                            else _config.smsConfig.StateSMS = Config.STATE_SMS.DONE;
                                                  
                        }
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
                //Speak content SMS comming
                //_config.smsConfig.StateSMS = Config.STATE_SMS.SPEAK_NUMBER;
                //await SpeakContentSMS(messengeData, _config.smsConfig.StateSMS);
                //int tryNumber = 3;
                ////Listen request of user : Yes or No
                //_config.smsConfig.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                //while (status != 0 && tryNumber > 0 && !_config.phoneConfig.IsHandlePhoneRunnig)
                //{
                //    status = await listenRequest();
                //    await Task.Delay(500);
                //    if (status != 0)
                //    {
                //        tryNumber--;
                //        await SpeakMsg("I can't hear you, please try again");
                //        await Task.Delay(500);
                //    }
                //}
                //if (status == 0)
                //{
                //    _config.smsConfig.StateSMS = Config.STATE_SMS.LISTEN_CONTENT_ANSWER;
                //    Log.Info(TAG, "status " + status.ToString());
                //    Log.Info(TAG, "Listen Answer " + _answer);
                //    //await Task.Delay(1000);
                //    //Listen msg reply
                //    await SpeakMsg("Please speak messenge after beep ");
                //    await Task.Delay(500);
                //    status = -1;
                //    tryNumber = 3;
                //    while (status != 0 && tryNumber >= 0 && !_config.phoneConfig.IsHandlePhoneRunnig)
                //    {
                //        status = await listenRequest();
                //        await Task.Delay(500);
                //        if (status != 0 )
                //        {
                //            tryNumber--;
                //            await SpeakMsg("I can't hear you, please try again");
                //            await Task.Delay(500);
                //        }
                //    }
                //    Log.Info(TAG, "Listen reply " + _answer);
                //    //messengeData.Reply(_answer);
                //}
                //else
                //{
                //    //Can not hear request reply
                //}
            }
            if (_config.smsConfig.StateSMS == Config.STATE_SMS.DONE)
                _config.smsConfig.IsHandleSMSRunnig = false;

        }

        private bool EmptyMessenge(){
            if (_messengeQueue.Empty() && _config.smsConfig.MessengeBackUp == null) return true;
            return false;
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

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG,"FreeHand.QueueMessenge.Invoke");
            if (!_config.smsConfig.IsHandleSMSRunnig)
                SMSHandleSpeak();
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
