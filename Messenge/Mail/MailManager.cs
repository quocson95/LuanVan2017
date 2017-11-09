using System;
using Android.Content;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
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
                _messQueue.EnqueueMessengeListQueue(item.SyncInbox());
            }
            if (!_messQueue.Empty())
            {
                var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                _context.SendBroadcast(speakSMSIntent);
            }

        }
        public void StartAutoCheckMail()
        {
            Console.WriteLine("Timer start");
            if (!_enableAutoCheck) _aTimer.Enabled = false;
            else _aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _aTimer.Enabled = false;
            CheckMail();
            if (!_enableAutoCheck) _aTimer.Enabled = false;
            else _aTimer.Enabled = true;
        }

    }
}
