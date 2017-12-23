
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
        Message.Service.MessageService _messService;
        Phone.PhoneCallService _phoneService;
        Message.Mail.MailSerivce _mailService;
        Config _cfg;
        Handler handler;
        Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");
            isStart = false;
            handler = new Handler();
            _cfg = Config.Instance();
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
            string action = intent.Action;
            if (action != null)
            {
                switch (action)
                {
                    //MainService
                    case Model.Constants.ACTION_START_MAIN_SERVICE:
                        StartMainService();
                        break;
                    case Model.Constants.ACTION_STOP_MAIN_SERVICE:
                        StopMainService();
                        break;

                    //Message Service
                    case Model.Constants.ACTION_START_MESSAGE_SERVICE:
                        if (isStart) StartMessenegeService();
                        break;
                    case Model.Constants.ACTION_STOP_MESSAGE_SERVICE:
                        if (isStart) StopMessengeService();
                        break;

                    //SMS Service
                    case Model.Constants.ACTION_START_SMS_SERVICE:
                        if (isStart) _messService.RegisterSMSReceiver();
                        break;
                    case Model.Constants.ACTION_STOP_SMS_SERVICE:
                        if (isStart) _messService.UnregisterSMSReceiver();
                        break;


                    //Phone Service
                    case Model.Constants.ACTION_START_PHONE_SERVICE:
                        if (isStart) StartPhoneService();
                        break;
                    case Model.Constants.ACTION_STOP_PHONE_SERVICE:
                        if (isStart) StopPhoneService();
                        break;

                    //Smart Alert Phone
                    case Model.Constants.ACTION_START_PHONE_SMART_ALERT:
                        if (isStart && _phoneService != null)
                            if (_phoneService.IsStart()) _phoneService.StartMonitorScreen();
                        break;
                    case Model.Constants.ACTION_STOP_PHONE_SMART_ALERT:
                        if (isStart && _phoneService != null)
                            if (_phoneService.IsStart()) _phoneService.StopMonitorScreen();
                        break;
                    case Model.Constants.ACTION_START_MAIL_SERVICE:
                        if (isStart) StartMailService();
                        break;
                    case Model.Constants.ACTION_STOP_MAIL_SERVICE:
                        if (isStart) StopMailService();
                        break;
                    default:
                        break;
                }
            }
            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.Sticky;
        }

        private void StopMailService()
        {

            if ( _mailService != null)
                _mailService.Stop();
        }

        private void StartMailService()
        {
            if (_mailService == null)
                _mailService = new Message.Mail.MailSerivce(this);
            _mailService.Start();
        }

        private void StopMainService()
        {
            //Stop Service
            isStart = false;
            Log.Info(TAG, "OnStartCommand: The Main service is stopped.");
            handler.RemoveCallbacks(runnable);
            StopMessengeService();
            StopPhoneService();
            StopForeground(true);
            StopSelf();
        }

        private void StartMainService()
        {
            //Start Service 
            if (isStart)
            {
                Log.Info(TAG, "OnStartCommand: The Main service is already running. ");

            }
            else
            {
                isStart = true;
                Log.Info(TAG, "OnStartCommand: The Main service is starting.");
                RegisterForegroundService();
                //handler.PostDelayed(runnable, Model.Constants.DELAY_BETWEEN_LOG_MESSAGES);
                StartMessenegeService();
                if (_cfg.phone.Enable) 
                    StartPhoneService();
               

            }
        }

        void StartMessenegeService()
        {
            if (_messService == null)
                _messService = new Message.Service.MessageService();
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
            //if (isStart){                
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Cancel(Model.Constants.SERVICE_RUNNING_NOTIFICATION_ID);   

            if (_messService != null)
                _messService.Destroy();
            
            if (_phoneService != null)
                _phoneService.Destroy();
            
            if (_mailService != null)
                _mailService.Destroy();
            //}

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
