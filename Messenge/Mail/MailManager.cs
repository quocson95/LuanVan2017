using System;
using Android.Content;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;

using System.Threading;

namespace FreeHand
{
    public class MailManager
    {
        private static MailManager _instance;
        private List<IMailAction> _listMailAction;
        System.Timers.Timer _aTimer;
        private bool _enableAutoCheck;
        private Model.MessengeQueue _messQueue;
        private Context _context;
        private TaskCompletionSource<Java.Lang.Object> _tcs;

        public bool EnableAutoCheck { get => _enableAutoCheck; set => _enableAutoCheck = value; }
        public Context Context { get => _context; set => _context = value; }

        private MailManager()
        {

            _listMailAction = new List<IMailAction>();
            _aTimer = new System.Timers.Timer();
            _aTimer.Enabled = false;
            EnableAutoCheck = false;
            _aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _aTimer.Interval = 30000;
            _messQueue = Model.MessengeQueue.GetInstance();
        }


        public static MailManager Instance()
        {
            if (_instance == null) _instance = new MailManager();
            return _instance;
        }

        public void AddMailAccount(IMailAction item)
        {
            _listMailAction.Add(item);
        }

        public void CheckMail()
        {
            foreach (var item in _listMailAction)
            {
                _messQueue.EnMessengeListQueue(item.SyncInbox());
            }
            if (!_messQueue.Empty())
            {
                var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                _context.SendBroadcast(speakSMSIntent);
            }

        }
        public async Task StartAutoCheckMail()
        {
            //CheckMail();
            //Console.WriteLine("Timer start");
            //if (!_enableAutoCheck) _aTimer.Enabled = false;
            //else _aTimer.Enabled = true;
            var certificate = new X509Certificate2(@"C:\path\to\certificate.p12", "kakula@7#9", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential
                                                          .Initializer("699738745588-ulrlq8d7tk37fct854i0hauk34ie5hh9.apps.googleusercontent.com @developer.gserviceaccount.com")
            {
                // Note: other scopes can be found here: https://developers.google.com/gmail/api/auth/scopes
                Scopes = new[] { "https://mail.google.com/" },
                User = "dangquocson1995@gmail.com@gmail.com"
            }.FromCertificate(certificate));

            bool result = await credential.RequestAccessTokenAsync(CancellationToken.None);

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //_aTimer.Enabled = false;
            //CheckMail();
            //if (!_enableAutoCheck) _aTimer.Enabled = false;
            //else _aTimer.Enabled = true;=
        }

    }
}
