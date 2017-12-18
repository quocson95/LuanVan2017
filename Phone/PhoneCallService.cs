
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
        IntentFilter screenStateFilter;
        private Config _cfg;
        bool monitorScreen;
        bool isStart;
        public PhoneCallService()
        {
            Log.Info(TAG, "Initializing");
            monitorScreen = false;
            isStart = false;
            screenStateFilter = new IntentFilter();
            screenStateFilter.AddAction(Intent.ActionScreenOff);
            screenStateFilter.AddAction(Intent.ActionScreenOn);

            _cfg = Config.Instance();
            phoneCallReceiver = new PhoneCallBroadcastReceiver();
            mScreenStateReceiver = new ScreenStateBroadcastReceiver();

        }
        public void Start()
        {
            isStart = true;
            // start your service logic here
            Log.Info(TAG, "Start: Register PhoneCallBroadcastReceiver ");

            Application.Context.RegisterReceiver(this.phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));
            if (_cfg.phoneConfig.SmartAlert) StartMonitorScreen();
            // Return the correct StartCommandResult for the type of service you are building           
        }

        public void StartMonitorScreen()
        {
            if (!monitorScreen)
            {
                Log.Info(TAG, "Start Monitor Screen ");
                monitorScreen = true;
                Application.Context.RegisterReceiver(mScreenStateReceiver, screenStateFilter);
            }
            else
                Log.Info(TAG, "Monitor Screen Already Start");
        }

        public void StopMonitorScreen()
        {
            
            if (monitorScreen)
            {
                Log.Info(TAG, "Stop Monitor Screen ");
                Application.Context.UnregisterReceiver(this.mScreenStateReceiver);
                monitorScreen = false;
            }
        }

        public void Stop()
        {
            isStart = false;
            Log.Info(TAG, "Stop"); 
            Log.Info(TAG, "UnRegister PhoneCallBroadcastReceiver ");
            Application.Context.UnregisterReceiver(this.phoneCallReceiver);
            StopMonitorScreen();
        }

        public bool IsStart(){
            return isStart;
        }

        public void Destroy()
        {
            Log.Info(TAG, "Destroy ");
            phoneCallReceiver.Dispose();
            mScreenStateReceiver.Dispose();
        }
              
    }

  
}
