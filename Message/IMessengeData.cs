namespace FreeHand.Model
{
    public enum TYPE_MESSAGE{
        SMS,
        MAIL
    }
    public interface IMessengeData
    {
        TYPE_MESSAGE Type();
        string GetMessengeContent();
        string GetNameSender();
        string GetAddrSender();
        void Reply(string msg);
        string GetDesAddress();
        void SetDesAddress(string des);
        void SetNameSender(string name);
        void SetMessengeContent(string content);
        void SetAddrSender(string addr);
        void MarkSeen();
    }
}
