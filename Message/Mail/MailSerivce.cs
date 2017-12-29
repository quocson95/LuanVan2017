using System;
using System.Collections.Generic;
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
        MessengeQueue _messengeQueue
; 
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
                    SyncMail();
                    if (_cfg.mail.Enable && isStart)
                        handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_SYNC_MAIL);
                });
            });
        }

        public void Start()
        {
            if (!isStart)
            {
                Log.Info(TAG,"Start Mail Service");
                isStart = true;
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
                _cfg.mail.Clean();
            }
            else 
                Log.Info(TAG, "Mail Service is not start, can't stop");
        }


        public void Destroy()
        {
            isStart = false;
            handler.Dispose();
        }

        public void SyncMail()
        {
            Log.Debug(TAG, "Start Sync Mail");
            IList<IMailAction> _lstMail =  _cfg.account.LstMail;
            foreach (var item in _lstMail )
            {
                if (!item.isLogin())
                {
                    item.Login();
                }
                IList<IMessengeData> inbox = item.SyncInbox();
                if (inbox.Count > 0)
                {
                    _messengeQueue.EnMessengeListQueue(inbox);
                    var speakSMSIntent = new Intent("FreeHand.QueueMessenge.Invoke");
                    _context.SendBroadcast(speakSMSIntent);
                }
            }
        }

    }
}
