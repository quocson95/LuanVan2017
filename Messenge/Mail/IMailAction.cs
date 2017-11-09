using System;
using System.Collections.Generic;
using FreeHand.Model;
namespace FreeHand
{
    public interface IMailAction
    {
        void Login();
        List<IMessengeData> SyncInbox();
        void Logout();
        bool isLogin();
    }
}
