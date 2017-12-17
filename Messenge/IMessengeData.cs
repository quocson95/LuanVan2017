namespace FreeHand.Model
{
    public interface IMessengeData
    {
        string Type();
        string GetMessengeContent();
        string GetNameSender();
        string GetAddrSender();
        string Reply(string msg);
        void SetNameSender(string name);
        void SetMessengeContent(string content);
        void SetAddrSender(string addr);
        void MarkSeen();
    }
}
