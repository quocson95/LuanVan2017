using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Speech;
using Android.Util;
using FreeHand.Model;

namespace FreeHand.Message.Service
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
            if (!_config.sms.IsHandleSMSRunnig && !_config.phone.IsHandlePhoneRunnig)
                SMSHandleSpeak(context);
        }

        public void Stop()
        {
            Log.Info(TAG, "Stop: ");
            stop = true;
            if (_speech != null)
            {
                _speech.StopListening();
                _speech.Cancel();
                _speech.UnregisterFromRuntime();


            }
            _ttsLib.Stop();
            //_config.Clean();
        }
        private async void SMSHandleSpeak(Context context)
        {
            _config.sms.IsHandleSMSRunnig = true;
            stop = false;
            Log.Info(TAG, "SMS Handle status {0}  state: {1} " , _config.sms.IsHandleSMSRunnig.ToString(),_config.sms.StateSMS);
            Log.Debug(TAG, "Queue {0}", _messengeQueue.Count());
            Model.IMessengeData messengeData = null;

            int try_listen = 3;

            messengeData = GetMessege(_config.sms.StateSMS);

            if (_config.sms.StateSMS != Config.STATE_SMS.IDLE
                && _config.sms.StateSMS != Config.STATE_SMS.DONE)
            {
                Log.Info(TAG, "Continuous Speak Messenger");
                await Task.Delay(2000);
                //await SpeakMsg(Resource.String.tts_continous_speak_prev_mess);
                await SpeakMsg(_scripLang.tts_continous_speak_prev_mess);
            }
            if (_config.sms.StateSMS == Config.STATE_SMS.IDLE)
            {
                _config.sms.StateSMS = Config.STATE_SMS.SPEAK_NUMBER;
            }

            while (messengeData != null &&
                   _config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE) && !stop)
            {
                //if (_config.IsUpdateCfg)
                //{
                //    _config.IsUpdateCfg = false;
                //    await _ttsLib.ReInitTTS();
                //}
                await _ttsLib.GetTTS();

                Log.Info(TAG, "Speak Messenge State {0} type mess : {1} " , _config.sms.StateSMS,messengeData.Type().ToString());
                switch (_config.sms.StateSMS)
                {
                    case Config.STATE_SMS.IDLE:
                        switch (messengeData.Type())
                        {
                            case TYPE_MESSAGE.SMS:
                                if (_config.sms.Enable)
                                    await SpeakMsg(_scripLang.tts_you_get_new_mess);
                                break;
                            case TYPE_MESSAGE.MAIL:
                                if (_config.mail.Enable)
                                    await SpeakMsg(_scripLang.tts_you_get_new_mail);
                                break;
                        }
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
                        await StateListenRequest(context, try_listen,messengeData.Type());
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
            _config.sms.IsHandleSMSRunnig = false;
            //End While
            if (_config.sms.StateSMS == Config.STATE_SMS.DONE)
            {
                _config.sms.StateSMS = Config.STATE_SMS.IDLE;
                _config.sms.IsHandleSMSRunnig = false;
            }

        }

        private async Task StateListenRequest(Context context, int try_listen,Model.TYPE_MESSAGE type)
        {
            if (_config.GetPermissionRun(Config.PERMISSION_RUN.MESSENGE))
            {
                int status;
                status = -1;
                status = await listenRequest(context,type);
                if (status == 0)
                    _config.sms.StateSMS = Config.STATE_SMS.LISTEN_CONTENT_ANSWER;
                else if (status == -2 ) _config.sms.StateSMS = Config.STATE_SMS.DONE;
                else if (status != 0 && try_listen > 0)
                {
                    //await SpeakMsg(Resource.String.tts_can_not_hear_voice);
                    await SpeakMsg(_scripLang.tts_can_not_hear_voice);
                    await Task.Delay(500);
                    _config.sms.StateSMS = Config.STATE_SMS.LISTENT_REQUEST_ANSWER;
                }
                else _config.sms.StateSMS = Config.STATE_SMS.DONE;

            }
            else 
                _config.sms.StateSMS = Config.STATE_SMS.DONE;
        }

        private async Task StateReadlyReply(IMessengeData messengeData)
        {
            switch (messengeData.Type())
            {
                case TYPE_MESSAGE.SMS:
                    if (!_config.sms.AllowAutoReply && _config.sms.Enable)
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
                    break;
                case TYPE_MESSAGE.MAIL:
                    if (!_config.mail.AutoReply && _config.mail.Enable)
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
                    break;
            }


        }

        private void AutoReply(IMessengeData messengeData)
        {
            TYPE_MESSAGE type = messengeData.Type();
            switch (type)
            {
                case TYPE_MESSAGE.SMS:
                    Log.Info(TAG, "Auto reply mess SMS {0}", _config.sms.CustomContetnReply);
                    messengeData.Reply(_config.sms.CustomContetnReply);
                    break;
                case TYPE_MESSAGE.MAIL:                    
                    Log.Info(TAG, "Auto reply mess mail {0}", _config.mail.ContentReply);
                    messengeData.Reply(_config.mail.ContentReply);
                    break;
            }
        }



        //StateSpeakContent
        private async Task StateSpeakContent(IMessengeData messengeData)
        {            
            switch (messengeData.Type())
            {
                case TYPE_MESSAGE.SMS:
                    if (_config.sms.AllowSpeakContent && _config.sms.Enable)
                    {
                        await SpeakMsg(_scripLang.tts_content_mess);
                        await SpeakMsg(messengeData.GetMessengeContent());
                    }
                    break;
                case TYPE_MESSAGE.MAIL:
                    if (_config.mail.AllowSpeakContent && _config.mail.Enable)
                    {
                        await SpeakMsg(_scripLang.tts_subject_mail);
                        await SpeakMsg(messengeData.GetMessengeContent());
                    }
                    break;
            }
            _config.sms.StateSMS = Config.STATE_SMS.READY_REPLY;
        }

        //StateSpeakName
        private async Task StateSpeakName(IMessengeData messengeData)
        {
            switch (messengeData.Type())
            {
                case TYPE_MESSAGE.SMS:
                    if (_config.sms.AllowSpeakName && _config.sms.Enable)
                    {
                        //await SpeakMsg("Name Sender ");
                        //await SpeakMsg(Resource.String.tts_name_sender);
                        await SpeakMsg(_scripLang.tts_name_sender);
                        if (messengeData.GetNameSender().Equals("Unknow"))
                            await SpeakMsg(_scripLang.tts_name_sender_content);
                        else await SpeakMsg(messengeData.GetNameSender());
                    }
                    break;
                case TYPE_MESSAGE.MAIL:
                    if (_config.mail.AllowSpeakName && _config.mail.Enable)
                    {
                        //await SpeakMsg("Name Sender ");
                        //await SpeakMsg(Resource.String.tts_name_sender);
                        await SpeakMsg(_scripLang.tts_name_sender);
                        if (messengeData.GetNameSender().Equals("Unknow"))
                            await SpeakMsg(_scripLang.tts_name_sender_content);
                        else await SpeakMsg(messengeData.GetNameSender());

                        await SpeakMsg(_scripLang.tts_to);
                        await SpeakMsg(messengeData.GetDesAddress());
                    }
                    break;
            }

            _config.sms.StateSMS = Config.STATE_SMS.SPEAK_CONTENT;
        }

        //StateSpeakNumber
        private async Task StateSpeakNumber(IMessengeData messengeData)
        {
            switch (messengeData.Type())
            {
                case TYPE_MESSAGE.SMS:
                    if (_config.sms.AllowSpeakNumber && _config.sms.Enable)
                    {
                        //await SpeakMsg("From ");
                        //await SpeakMsg(Resource.String.tts_from);
                        await SpeakMsg(_scripLang.tts_from);
                        await SpeakMsg(messengeData.GetAddrSender());
                    }
                    break;
                case TYPE_MESSAGE.MAIL:
                    if (_config.mail.AllowSpeakAddr && _config.mail.Enable)
                    {
                        //await SpeakMsg("From ");
                        //await SpeakMsg(Resource.String.tts_from);
                        await SpeakMsg(_scripLang.tts_from);
                        await SpeakMsg(messengeData.GetAddrSender());
                    }
                    break;
            }

            _config.sms.StateSMS = Config.STATE_SMS.SPEAK_NAME_SENDER;
        }

        private bool EmptyMessenge()
        {
            if (_messengeQueue.Empty() && _config.sms.MessengeBackUp == null) 
            {
                Log.Debug(TAG, "Empty message");
                return true;
            }

            return false;
        }
        private async Task<int> listenRequest(Context context,Model.TYPE_MESSAGE type)
        {
            bool allow;
            allow = false;
            switch (type)
            {
                case TYPE_MESSAGE.SMS:
                    allow = _config.sms.Enable;
                    break;
                case TYPE_MESSAGE.MAIL:
                    allow = _config.mail.Enable;
                    break;
            }
            if (allow)
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
            else return -2;

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
                    if (_config.sms.MessengeBackUp != null)
                        result = (IMessengeData)_config.sms.MessengeBackUp;
                    else
                        result = null;
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