using System;
using System.Collections.Generic;
using Android.Util;

namespace FreeHand.Messenge.Mail
{
    public class MailSerivce
    {
        private static readonly string TAG = typeof(MailSerivce).FullName;
        Config _cfg;
        IList<IMailAction> _lstMail;
        public MailSerivce()
        {
            _cfg = Config.Instance();
            _lstMail = _cfg.mail.GetListMailAction();
        }


        public  void AddAccount(IMailAction item){
            item.Login();
            if (item.isLogin())
                _lstMail.Add(item);
            else
                Log.Info(TAG,"Can't add ImailAction, It's not connect");
        }

        public void SyncMail()
        {
            foreach (var item in _lstMail )
            {
                if (!item.isLogin())
                {
                    item.Login();
                }
                item.SyncInbox();
            }
        }
    }
}
