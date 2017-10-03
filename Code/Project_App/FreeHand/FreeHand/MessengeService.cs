
using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace FreeHand
{
    [Service(Label = "MessengeService")]
    [IntentFilter(new String[] { "com.yourname.MessengeService" })]
    public class MessengeService : Service
    {       
        private SMSBroadcastReceiver smsReceiver;
        private Config config; 
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here
            config = Config.Instance();
			smsReceiver = new SMSBroadcastReceiver(this);
			this.RegisterReceiver(this.smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }

}
