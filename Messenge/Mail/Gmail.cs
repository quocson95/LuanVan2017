using System;
using System.Collections.Generic;
namespace FreeHand
{
    public class Gmail : Model.IMessengeData
    {
        private List<string> _nameSender;
        private List<string> _addrSender;
        private string _type;
        private string _content;
        private MailKit.UniqueId _uids;       
        private FreeHand.GmailAction.MarkSeenAction markSeenAction;
        public Gmail(MailKit.UniqueId uids,FreeHand.GmailAction.MarkSeenAction m1)
        {
            _type = "EMA";
            _uids = uids;
            markSeenAction = m1;
            _nameSender = new List<string>();
            _addrSender = new List<string>();
        }


        public string GetAddrSender()
        {
            string result = "";
            foreach (var item in _addrSender)
                result = result + item + " ";
            return result;
        }

        public string GetMessengeContent()
        {
            return _content;
        }


        public string GetNameSender()
        {
            string result = "";
            foreach (var item in _nameSender)
                result = result + item + " ";
            return result;
        }

        public void MarkSeen()
        {
            markSeenAction(_uids);
        }

        public string Reply(string msg)
        {
            throw new NotImplementedException();
        }

        public void SetAddrSender(string addr)
        {
            _addrSender.Add(addr);
        }

        public void SetMessengeContent(string content)
        {
            _content = content;
        }

        public void SetNameSender(string name)
        {
            _nameSender.Add(name);
        }

        public string Type()
        {
            return _type;
        }
    }
}
