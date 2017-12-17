
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
    public class PhoneCallService
    {
        private static readonly string TAG = typeof(PhoneCallService).FullName;
        private PhoneCallBroadcastReceiver phoneCallReceiver;
        private ScreenStateBroadcastReceiver mScreenStateReceiver;
        private Config config;       
        public PhoneCallService()
        {
            config = Config.Instance();
            phoneCallReceiver = new PhoneCallBroadcastReceiver();
            mScreenStateReceiver = new ScreenStateBroadcastReceiver();

        }
        public void Start()
        {
            // start your service logic here
            Application.Context.RegisterReceiver(this.phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));
            MonitorScreen();
            // Return the correct StartCommandResult for the type of service you are building           
        }

        private void MonitorScreen()
        {
            IntentFilter screenStateFilter = new IntentFilter();
            screenStateFilter.AddAction(Intent.ActionScreenOff);
            screenStateFilter.AddAction(Intent.ActionScreenOn);
            Application.Context.RegisterReceiver(mScreenStateReceiver, screenStateFilter);
        }
      
        public void Stop()
        {
            Log.Info(TAG, "Stop");           
            Application.Context.UnregisterReceiver(this.phoneCallReceiver);
            Application.Context.UnregisterReceiver(this.mScreenStateReceiver);
            phoneCallReceiver.Dispose();
            mScreenStateReceiver.Dispose();

            
        }
              
    }

  
}
