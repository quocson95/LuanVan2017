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
            //Contract.Ensures(Contract.Result<Task>() != null);
            //         STTLib _stt = STTLib.Instance();
            //SpeechRecognizer speech = SpeechRecognizer.CreateSpeechRecognizer(_context);
            //speech.SetRecognitionListener(this);
            //speech.StartListening(_stt.IntentSTT());

            Model.IMessengeData messengeData = null;
            _config.RunningSMSHandle = true;
            int status;
            status = -1;
            while (!_messengeQueue.Empty())
            {
                if (_config.UpdateConfig)
                {
                    _config.UpdateConfig = false;
                    await _ttsLib.ReInitTTS();
                }
                messengeData = _messengeQueue.DequeueMessengeQueue();
                //Speak content SMS comming
                await SpeakContentSMS(messengeData);
                int tryNumber = 3;
                if (messengeData.Type().Equals("EMA")) messengeData.MarkSeen();
                //Listen request of user : Yes or No
                //while (status != 0 && tryNumber > 0)
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
                    //Log.Info(TAG, "status " + status.ToString());
                    //Log.Info(TAG, "Listen Answer " + _answer);
                    ////await Task.Delay(1000);
                    ////Listen msg reply
                    //await SpeakMsg("Please speak messenge after beep ");
                    //await Task.Delay(500);
                    //status = -1;
                    //tryNumber = 3;
                    //while (status != 0 && tryNumber > 0)
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
                //    Log.Info(TAG, "Listen reply " + _answer);
                //    //messengeData.Reply(_answer);
                //}
            }
            _config.RunningSMSHandle = false;

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
        private async Task SpeakContentSMS(Model.IMessengeData messengeData)
        {
            //messengeData.Reply("aaa");  
            await SpeakMsg("Get new SMS messeger ");
            await SpeakMsg("From ");
            await SpeakMsg(messengeData.GetAddrSender());
            Log.Info(TAG, messengeData.GetAddrSender());
            await SpeakMsg("Name Sender ");
            await SpeakMsg(messengeData.GetNameSender());
            Log.Info(TAG, messengeData.GetNameSender());
            await SpeakMsg("Content of Messenger");
            Log.Info(TAG, messengeData.GetMessengeContent());
            await SpeakMsg(messengeData.GetMessengeContent());

            await SpeakMsg("Do you want reply");
        }
        private async Task SpeakMsg(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            int status = await _ttsLib.SpeakMessenger(msg);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(TAG,"FreeHand.QueueMessenge.Invoke");
            if (!_config.RunningSMSHandle)
            {
                //Task task = new Task( async delegate { await SMSHandleSpeak(); });
                //task.Start();
                SMSHandleSpeak();
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
