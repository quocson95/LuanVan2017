using System;
using System.Collections.Generic;

namespace FreeHand.Model
{
    public interface IMessengeData
    {
        string GetMessengeContent();
        string GetNameSender();
        string GetAddrSender();
        string Reply(string msg);
        void SetNameSender(string name);
        void SetMessengeContent(string content);
        void SetAddrSender(string addr);
    }
}
