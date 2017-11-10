
using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace FreeHand
{
    [Service(Label = "PhoneCallService")]
    [IntentFilter(new String[] { "com.yourname.PhoneCallService" })]
    public class PhoneCallService : Service
    {
        private PhoneCallBroadcastReceiver phoneCallReceiver;
        private Config config; 
       

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here
            config = Config.Instance();
            phoneCallReceiver = new PhoneCallBroadcastReceiver(this);
            this.RegisterReceiver(this.phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));
            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }

  
}
