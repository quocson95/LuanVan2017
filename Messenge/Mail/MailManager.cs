using System;
using Android.Content;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
namespace FreeHand
{
    public class MailManager
    {
        private static MailManager instance;
        private List<IMailAction> listMailAction;
        System.Timers.Timer aTimer;
        private TaskCompletionSource<Java.Lang.Object> _tcs;

        private MailManager()
        {
            listMailAction = new List<IMailAction>();
            aTimer = new System.Timers.Timer();
            aTimer.Enabled = false;
        }

        public static MailManager Instance()
        {
            if (instance == null) instance = new MailManager();
            return instance;
        }

        public void AddMailAccount(IMailAction item)
        {
            listMailAction.Add(item);
        }

        public void CheckMail()
        {
            foreach(var item in listMailAction ){
                item.SyncInbox();
            }

        }
        public void autoCheckMail()
        {
            Console.WriteLine("Timer start");
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 30000;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
            CheckMail();
            aTimer.Enabled = true;
        }
            
    }
}
