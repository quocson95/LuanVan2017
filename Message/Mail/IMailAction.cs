using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using FreeHand.Model;
namespace FreeHand.Message.Mail
{
    public interface IMailAction
    {
        void Login();
        List<IMessengeData> SyncInbox();
        void Logout();
        bool isLogin();
        string GetNameLogin();      
        bool GetActive();
        bool SetActive(bool active);
        void Reply();
    }
}
