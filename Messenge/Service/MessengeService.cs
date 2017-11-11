using System;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
namespace FreeHand
{
    [Service(Label = "MessengeService")]
    [IntentFilter(new String[] { "com.yourname.MessengeService" })]
    public class MessengeService : Service
    {
        private static readonly string TAG = "MessengeService";
        private SMSBroadcastReceiver smsReceiver;
        private Messenge.SpeakMessengeBroadcastReceiver _speakReceicer;
        private Config config;
        private MailManager mailMng;
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here
            Log.Info(TAG, "Start SMSBroadcastReceiver");
            config = Config.Instance();
			smsReceiver = new SMSBroadcastReceiver(this);
            _speakReceicer = new Messenge.SpeakMessengeBroadcastReceiver(this);
			this.RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
            this.RegisterReceiver(this._speakReceicer, new IntentFilter("FreeHand.QueueMessenge.Invoke"));
            //Start Email Manage;
            Log.Info(TAG,"Start MailManager");
            mailMng = MailManager.Instance();
            mailMng.Context = this;
            mailMng.EnableAutoCheck = true;
            mailMng.StartAutoCheckMail();
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy(){
            Log.Info(TAG,"OnDestroy");
            base.OnDestroy();
            this.UnregisterReceiver(this.smsReceiver);
            this.UnregisterReceiver(this._speakReceicer);
            mailMng.EnableAutoCheck = false;
        }
    }

}
