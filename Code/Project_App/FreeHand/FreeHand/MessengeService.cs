
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
        IBinder binder;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here

            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new MessengeServiceBinder(this);
            return binder;
        }
    }

    public class MessengeServiceBinder : Binder
    {
        readonly MessengeService service;

        public MessengeServiceBinder(MessengeService service)
        {
            this.service = service;
        }

        public MessengeService GetMessengeService()
        {
            return service;
        }
    }
}
