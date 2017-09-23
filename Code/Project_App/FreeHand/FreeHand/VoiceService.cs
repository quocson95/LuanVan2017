
using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace FreeHand
{
    [Service(Label = "VoiceService")]
    [IntentFilter(new String[] { "com.yourname.VoiceService" })]
    public class VoiceService : IntentService
    {
        IBinder binder;

        protected override void OnHandleIntent(Intent intent)
        {
            // Perform your service logic here
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new VoiceServiceBinder(this);
            return binder;
        }
    }

    public class VoiceServiceBinder : Binder
    {
        readonly VoiceService service;

        public VoiceServiceBinder(VoiceService service)
        {
            this.service = service;
        }

        public VoiceService GetVoiceService()
        {
            return service;
        }
    }
}
