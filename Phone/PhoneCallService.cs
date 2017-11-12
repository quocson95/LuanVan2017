
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Plugin.Vibrate;
using Android.Util;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;

namespace FreeHand.Phone
{
    [Service(Label = "PhoneCallService")]
    [IntentFilter(new String[] { "com.yourname.PhoneCallService" })]
    public class PhoneCallService : Service
    {
        private static readonly string TAG = "PhoneCallService";
        private PhoneCallBroadcastReceiver phoneCallReceiver;
        private ScreenStateBroadcastReceiver mScreenStateReceiver;
        private Config config;
        DeviceMotionImplementation sensor;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here
            config = Config.Instance();
            phoneCallReceiver = new PhoneCallBroadcastReceiver(this);
            this.RegisterReceiver(this.phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));

            MonitorScreen();
            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.NotSticky;
        }

        private void MonitorScreen()
        {
            mScreenStateReceiver = new ScreenStateBroadcastReceiver(this);
            IntentFilter screenStateFilter = new IntentFilter();
            screenStateFilter.AddAction(Intent.ActionScreenOff);
            screenStateFilter.AddAction(Intent.ActionScreenOn);
            this.RegisterReceiver(mScreenStateReceiver, screenStateFilter);
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override void OnDestroy()
        {
            Log.Info(TAG, "OnDestroy");
            base.OnDestroy();
            this.UnregisterReceiver(this.phoneCallReceiver);
            this.UnregisterReceiver(this.mScreenStateReceiver);
            this.phoneCallReceiver.Dispose();
            this.mScreenStateReceiver.Dispose();

            
        }
              
    }

  
}
