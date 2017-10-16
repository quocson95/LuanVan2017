using System;
namespace FreeHand.Model
{
    public interface IMessengeData
    {
        string GetMessengeContent();
        string GetNameSender();
        string GetAddrSender();
        string Reply(string msg);
        void SetNameSender(string name);
    }
}
