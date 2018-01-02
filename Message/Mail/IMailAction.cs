using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeHand.Model;
using MimeKit;

namespace FreeHand.Message.Mail
{
    public interface IMailAction
    {
        void Login();
        Task<List<IMessengeData>> SyncInbox();
        void Logout();
        bool isLogin();
        string GetNameLogin();
        string GetPwd();
        bool GetActive();
        bool SetActive(bool active);
        void Reply(string msg, IList<string> _addrDes, MimeMessage message);
    }
}
