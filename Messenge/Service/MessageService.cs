using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace FreeHand.Messenge.Service
{    
    public class MessageService
    {
        private static readonly string TAG = typeof(MessageService).FullName;
        private SMSBroadcastReceiver _smsReceiver;
        private SpeakMessengeBroadcastReceiver _speakReceicer;
        //private Config _cfg;
        //private MailManager mailMng;
        bool isStart;

        public MessageService()
        {
            Log.Info(TAG,"Initializing");
            isStart = false;
            //_cfg = Config.Instance();
            _smsReceiver = new SMSBroadcastReceiver();
            _speakReceicer = new SpeakMessengeBroadcastReceiver();
        }
        public void Start()
        {
            if (isStart)
                Log.Info(TAG, "Start : MessengeManage already start");
            else
            {
                // start your service logic here

                Log.Info(TAG, "Start : Register MessengeManage");
                Application.Context.RegisterReceiver(this._smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
                Application.Context.RegisterReceiver(this._speakReceicer, new IntentFilter("FreeHand.QueueMessenge.Invoke"));
                isStart = true;
                //Start Email Manage;
                //Log.Info(TAG,"Start MailManager");
                //mailMng = MailManager.Instance();
                //mailMng.Context = Application.Context;
                //mailMng.StartAutoCheckMail();
            }
                       
        }

        public void Stop(){
            if (isStart)
            {
                Log.Info(TAG, "Stop : MessengeManage ");
                _speakReceicer.Stop();           
                Application.Context.UnregisterReceiver(_smsReceiver);
                Application.Context.UnregisterReceiver(_speakReceicer);
                //mailMng.EnableAutoCheck = false;
            }
            else
                Log.Info(TAG, "Stop : MessengeManage is not running");
            isStart = false;
        }

        public void Destroy()
        {
            _smsReceiver.Dispose();
            _speakReceicer.Dispose();
        }
              
    }

}
