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
    public class SpeakMessengeBroadcastReceiver : BroadcastReceiver, IRecognitionListener
    {
        private static readonly string TAG = typeof(SpeakMessengeBroadcastReceiver).FullName;
        private Model.MessengeQueue _messengeQueue;
        private TTSLib _ttsLib;
        private STTLib _stt;
        private Config _config;
        private ScriptLang _scripLang;
        private SpeechRecognizer _speech;
        Context _context;
        private TaskCompletionSource<Java.Lang.Object> _tcs;
        private string _answer = "";
        bool stop;
        public SpeakMessengeBroadcastReceiver()
        {
            Log.Info(TAG, "Initializing");
            _messengeQueue = Model.MessengeQueue.GetInstance();
            _config = Config.Instance();
            _ttsLib = TTSLib.Instance();
            _stt = STTLib.Instance();
            _scripLang = ScriptLang.Instance();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG, "OnReceive: " + intent.Action);
            _context = context;
            if (!_config.sms.IsHandleSMSRunnig)
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
            _config.sms.Clean();
        }
        private async void SMSHandleSpeak(Context context)
        {
            _config.sms.IsHandleSMSRunnig = true;
            stop = false;
            Log.Info(TAG, "SMS Handle status " + _config.sms.IsHandleSMSRunnig.ToString());

            Model.IMessengeData messengeData = null;

            int try_listen = 3;
            if (_config.sms.StateSMS != Config.STATE_SMS.IDLE
                && _config.sms.StateSMS != Config.STATE_SMS.DONE)
            {
                Log.Info(TAG, "Continuous Speak Messenger");
                await Task.Delay(2000);
                //await SpeakMsg(Resource.String.tts_continous_speak_prev_mess);
                await SpeakMsg(_scripLang.tts_continous_speak_prev_mess);
            }
            messengeData = GetMessege(_config.sms.StateSMS);

            while (!EmptyMessenge() &&
                   _config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE) && !stop)
            {
                if (_config.IsUpdateCfg)
                {
                    _config.IsUpdateCfg = false;
                    await _ttsLib.ReInitTTS();
                }


                Log.Info(TAG, "Speak Messenge State " + _config.sms.StateSMS);
                switch (_config.sms.StateSMS)
                {
                    case Config.STATE_SMS.IDLE:
                        _config.sms.StateSMS = Config.STATE_SMS.SPEAK_NUMBER;
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
                        await StateReadlyReply(messengeData);
                        break;
                    case Config.STATE_SMS.LISTENT_REQUEST_ANSWER:
                        await StateListenRequest(context, try_listen);
                        try_listen--;
                        break;
                    case Config.STATE_SMS.LISTEN_CONTENT_ANSWER:
                        messengeData.Reply(_answer);
                        Log.Info(TAG, "Reply SMS to " + messengeData.GetAddrSender() + " content: " + _answer);
                        _config.sms.StateSMS = Config.STATE_SMS.DONE;
                        break;
                    case Config.STATE_SMS.DONE:

                        messengeData = GetMessege(Config.STATE_SMS.DONE);
                        if (messengeData != null)
                            _config.sms.StateSMS = Config.STATE_SMS.IDLE;
                        break;
                }

            }
            //End While
            if (_config.sms.StateSMS == Config.STATE_SMS.DONE)
            {
                _config.sms.StateSMS = Config.STATE_SMS.IDLE;
                _config.sms.IsHandleSMSRunnig = false;
            }

        }

        private async Task StateListenRequest(Context context, int try_listen)
        {
            if (_config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE))
            {
                int status;
                status = -1;
                status = await listenRequest(context);
                if (status == 0)
                    _config.sms.StateSMS = Config.STATE_SMS.LISTEN_CONTENT_ANSWER;
                else if (status != 0 && try_listen > 0)
                {
                    //await SpeakMsg(Resource.String.tts_can_not_hear_voice);
                    await SpeakMsg(_scripLang.tts_can_not_hear_voice);
                    await Task.Delay(500);
                    _config.sms.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                }
                else _config.sms.StateSMS = Config.STATE_SMS.DONE;

            }
        }

        private async Task StateReadlyReply(IMessengeData messengeData)
        {
            if (!_config.sms.AllowAutoReply)
            {                
                //await SpeakMsg( Resource.String.tts_do_you_want_rep);
                await SpeakMsg(_scripLang.tts_ask_for_reply);
                _config.sms.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
            }
            else
            {
                AutoReply(messengeData);
                _config.sms.StateSMS = Config.STATE_SMS.DONE;
            }

        }

        private void AutoReply(IMessengeData messengeData)
        {
            TYPE_MESSAGE type = messengeData.Type();
            switch (type)
            {
                case TYPE_MESSAGE.SMS:
                    Log.Info(TAG, "Auto reply mess {0}", _config.sms.CustomContetnReply);
                    messengeData.Reply(_config.sms.CustomContetnReply);
                    break;
                case TYPE_MESSAGE.MAIL:
                    //TODO Add auto reply for mail
                    break;
            }
        }



        //StateSpeakContent
        private async Task StateSpeakContent(IMessengeData messengeData)
        {
            if (_config.sms.AllowSpeakContent)
            {                
                //await SpeakMsg(Resource.String.tts_content_mess);
                await SpeakMsg(_scripLang.tts_content_mess);
                await SpeakMsg(messengeData.GetMessengeContent());
            }
            _config.sms.StateSMS = Config.STATE_SMS.READY_REPLY;
        }

        //StateSpeakName
        private async Task StateSpeakName(IMessengeData messengeData)
        {
            if (_config.sms.AllowSpeakName)
            {
                //await SpeakMsg("Name Sender ");
                //await SpeakMsg(Resource.String.tts_name_sender);
                await SpeakMsg(_scripLang.tts_name_sender);
                if (messengeData.GetNameSender().Equals("Unknow"))
                    await SpeakMsg(_scripLang.tts_name_sender_content);
                else await SpeakMsg(messengeData.GetNameSender());
            }
            _config.sms.StateSMS = Config.STATE_SMS.SPEAK_CONTENT;
        }

        //StateSpeakNumber
        private async Task StateSpeakNumber(IMessengeData messengeData)
        {
            if (_config.sms.AllowSpeakNumber)
            {
                //await SpeakMsg("you get a new message ");
                //await SpeakMsg(Resource.String.tts_you_get_new_mess);
                await SpeakMsg(_scripLang.tts_you_get_new_mess);
                //await SpeakMsg("From ");
                //await SpeakMsg(Resource.String.tts_from);
                await SpeakMsg(_scripLang.tts_from);
                await SpeakMsg(messengeData.GetAddrSender());
            }
            _config.sms.StateSMS = Config.STATE_SMS.SPEAK_NAME_SENDER;
        }

        private bool EmptyMessenge()
        {
            if (_messengeQueue.Empty() && _config.sms.MessengeBackUp == null) return true;
            return false;
        }
        private async Task<int> listenRequest(Context context)
        {
            _speech = SpeechRecognizer.CreateSpeechRecognizer(context);
            _speech.SetRecognitionListener(this);
            _tcs = new TaskCompletionSource<Java.Lang.Object>();
            try
            {
                _speech.StartListening(_stt.IntentSTTCustome(_config.speech.LangSelectBySTT));
            }
            catch { /* Dont care */}
            return (int)await _tcs.Task;
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

                    _config.sms.MessengeBackUp = result;
                    break;
                default:
                    result = _config.sms.MessengeBackUp;
                    break;
            }
            return result;

        }


        private async Task SpeakMsg(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            int status = await _ttsLib.SpeakMessenger(msg);
        }

        async Task SpeakMsg(int resid)
        {
            string s = _context.GetString(resid);
            await _ttsLib.SpeakMessenger(s);
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