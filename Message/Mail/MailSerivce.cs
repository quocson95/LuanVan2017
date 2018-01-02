using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Util;
using FreeHand.Model;

namespace FreeHand.Message.Mail
{
    public class MailSerivce
    {
        private static readonly string TAG = typeof(MailSerivce).FullName;
        Config _cfg;
        bool isStart;
        Handler handler;
        Action runnable;
        Context _context;
        MessengeQueue _messengeQueue;
        IList<IMailAction> lstMailAction = new List<IMailAction>();

        public MailSerivce(Context context)
        {
            _cfg = Config.Instance();
            _context = context;
            //_lstMail = _cfg.mail.GetListMailAction();
            isStart = false;
            _messengeQueue = MessengeQueue.GetInstance();
            handler = new Handler();

            runnable = new Action(() =>
            {
                Log.Debug(TAG, "runnable sync mail");
                Task.Run(() =>
                {
                    if (_cfg.mail.Enable && isStart)
                    {
                        SyncMail();
                        handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_SYNC_MAIL);
                    }
                    else 
                    {
                        DisconnectedMail();
                        if (_cfg.mail.AccountEmail.Count() > 0)
                        {
                            DisconnectedMail();
                            _cfg.mail.AccountEmail.Clear();
                        }
                    }
                });
            });
                      
        }

        public void AddMailAccount(string email,string token)
        {            
            if (isStart)
            {                                
                Log.Info(TAG, "Add Account {0} Mail Action", email);
                IMailAction newAccount = new GmailAction(email, token);
                lock (this)
                {
                    lstMailAction.Add(newAccount);
                }
            }
            else 
            {
                Log.Info(TAG, "Mail Service is not Start, it will be auto add when mail service enable");
            }
        }

        public void DelMailAccount(string email)
        {
            if (isStart)
            {
                lock (this)
                {
                    IMailAction itemDelete = lstMailAction.FirstOrDefault(item => item.GetNameLogin().Equals(email));
                    if (itemDelete != null)
                    {
                        Log.Info(TAG, "Delete Account {0}  Mail Action",itemDelete.GetNameLogin());
                        itemDelete.Logout();
                        lstMailAction.Remove(itemDelete);                       
                    }
                    else 
                    {
                        Log.Info(TAG, "email {0} not available",email);
                    }
                }
            }
            else
            {
                Log.Info(TAG, "Mail Service is not Start");
            }
            
        }

        public void Start()
        {
            if (!isStart)
            {
                Log.Info(TAG,"Start Mail Service");
                isStart = true;
                foreach (var item in _cfg.mail.AccountEmail)
                {
                    IMailAction newMail = new GmailAction(item.Item1.Email, item.Item2.Properties["access_token"]);
                    lstMailAction.Add(newMail);
                }
                handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_SYNC_MAIL);
            }
            else 
                Log.Info(TAG, "Mail Service already start");
        }

        public void Stop()
        {
            if (isStart){
                Log.Info(TAG, "Stop Mail Service");
                isStart = false;
                handler.RemoveCallbacks(runnable);  
                _messengeQueue.CleanMail();
            }
            else 
                Log.Info(TAG, "Mail Service is not start, can't stop");
        }

        private void DisconnectedMail()
        {
            foreach (var item in lstMailAction)
            {
                item.Logout();
            }
            lstMailAction.Clear();
        }

        public void Destroy()
        {
            isStart = false;
            handler.Dispose();
        }

        public async void SyncMail()
        {
            Log.Debug(TAG, "Start Sync Mail, number account {0}",lstMailAction.Count());
            foreach (var item in lstMailAction )
            {
                if (!item.isLogin())
                {
                    item.Login();
                }

                IList<IMessengeData> inbox = await item.SyncInbox();
                if (inbox.Count > 0)
                {
                    if (_cfg.mail.Enable)
                    {
                        _messengeQueue.EnMessengeListQueue(inbox);
                        var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                        _context.SendBroadcast(speakSMSIntent);
                    }
                }
            }
        }

    }
}
