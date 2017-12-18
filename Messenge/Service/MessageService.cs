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
        private Config _cfg;
        //private MailManager mailMng;
        bool isStart;
        bool registerSMS;
        public MessageService()
        {
            Log.Info(TAG,"Initializing");
            isStart = false;
            registerSMS = false;
            _cfg = Config.Instance();
            _smsReceiver = new SMSBroadcastReceiver();
            _speakReceicer = new SpeakMessengeBroadcastReceiver();
        }

        public void Start()
        {
            if (isStart)
                Log.Info(TAG, "Start : MessengeManage already start");
            else
            {                
                Log.Info(TAG, "Start : Register MessengeManage");
                if (_cfg.smsConfig.Enable) RegisterSMSReceiver();
                Application.Context.RegisterReceiver(this._speakReceicer, new IntentFilter("FreeHand.QueueMessenge.Invoke"));
                isStart = true;               
            }
                       
        }

        public void RegisterSMSReceiver()
        {
            if (!registerSMS)
            {
                Log.Info(TAG, "Register SMS Receiver");
                registerSMS = true;
                Application.Context.RegisterReceiver(this._smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
            }
            else
                Log.Info(TAG, "SMS Receiver Already Register");
        }

        public void UnregisterSMSReceiver()
        {            
            if (registerSMS)
            {
                Log.Info(TAG, "Unregister SMS Receiver");
                Application.Context.UnregisterReceiver(_smsReceiver);
                registerSMS = false;
            }
        }

        public void Stop(){
            if (isStart)
            {
                Log.Info(TAG, "Stop : MessengeManage ");
                _speakReceicer.Stop();           
                UnregisterSMSReceiver();
                Application.Context.UnregisterReceiver(_speakReceicer);              
            }
            else
                Log.Info(TAG, "Stop : MessengeManage is not running");
            isStart = false;
        }

        public void Destroy()
        {
            Log.Info(TAG, "Destroy ");
            _smsReceiver.Dispose();
            _speakReceicer.Dispose();
        }
              
    }

}
