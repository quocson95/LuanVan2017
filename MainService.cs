﻿
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace FreeHand
{
    [Service(Label = "MainService")]
    [IntentFilter(new String[] { "com.yourname.MainService" })]
    public class MainService : Service
    {
        private bool isStart;
        static readonly string TAG = typeof(MainService).FullName;
        Messenge.Service.MessageService _messService;
        Phone.PhoneCallService _phoneService;
        Handler handler;
        Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");
            isStart = false;
            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {                           
                    Log.Debug(TAG, "Test");
                    handler.PostDelayed(runnable, Model.Constants.DELAY_BETWEEN_LOG_MESSAGES);
            });
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here
            if (intent.Action.Equals(Model.Constants.ACTION_START))
            {
                //Start Service 
                if (isStart)
                {
                    Log.Info(TAG, "OnStartCommand: The Main service is already running. ");

                }
                else 
                {
                    Log.Info(TAG, "OnStartCommand: The Main service is starting.");
                    RegisterForegroundService();
                    handler.PostDelayed(runnable, Model.Constants.DELAY_BETWEEN_LOG_MESSAGES);
                    StartMessenegeService();
                    StartPhoneService();
                    isStart = true;

                }
            }
            else if (intent.Action.Equals(Model.Constants.ACTION_STOP))
            {
                //Stop Service
                Log.Info(TAG, "OnStartCommand: The Main service is stopped.");
                handler.RemoveCallbacks(runnable);
                StopMessengeService();
                StopPhoneService();
                StopForeground(true);
                StopSelf();

                isStart = false;
            }
                

            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.Sticky;
        }


        void StartMessenegeService()
        {
            if (_messService == null)
                _messService = new Messenge.Service.MessageService();
            _messService.Start();
        }

        void StopMessengeService()
        {
            if (_messService != null)
            {
                _messService.Stop();
            }    
        }

        void StartPhoneService()
        {
            if (_phoneService == null)
                _phoneService = new Phone.PhoneCallService();
            _phoneService.Start();
        }

        void StopPhoneService()
        {
            if (_phoneService != null)
            {
                _phoneService.Stop();
            }
        }

        public override void OnDestroy()
        {
            // We need to shut things down.           
            Log.Info(TAG, "OnDestroy: The Main started service is shutting down.");
            // Stop the handler.           
            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(Model.Constants.SERVICE_RUNNING_NOTIFICATION_ID);                     
            isStart = false;
            _messService.Destroy();
            base.OnDestroy();
        }


        void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                //.AddAction(BuildRestartTimerAction())
                //.AddAction(BuildStopServiceAction())
                .Build();


            // Enlist this instance of the service as a foreground service
            StartForeground(Model.Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));           
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Model.Constants.SERVICE_STARTED_KEY, true);
            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }


        public override IBinder OnBind(Intent intent)
        {
            
            return null;
        }
    }

   
}
