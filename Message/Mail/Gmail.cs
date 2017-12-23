using System;
using System.Collections.Generic;
using FreeHand.Model;

namespace FreeHand.Message.Mail
{
    public class Gmail : IMessengeData
    {
        private List<string> _nameSender;
        private List<string> _addrSender;
        private Model.TYPE_MESSAGE _type;
        private string _content;
        private MailKit.UniqueId _uids;
        private GmailAction.MarkSeenAction markSeenAction;
        public Gmail(MailKit.UniqueId uids, GmailAction.MarkSeenAction m1)
        {
            _type = Model.TYPE_MESSAGE.MAIL;
            _uids = uids;
            markSeenAction = m1;
            _nameSender = new List<string>();
            _addrSender = new List<string>();
        }

        TYPE_MESSAGE IMessengeData.Type() => _type;

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

    }
}
