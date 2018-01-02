using System;
using System.Collections.Generic;
using FreeHand.Model;
using MimeKit;

namespace FreeHand.Message.Mail
{
    public class Gmail : IMessengeData
    {
        private List<string> _nameSender;
        private List<string> _addrSender;
        private string _desAddress;
        private Model.TYPE_MESSAGE _type;
        private string _content;
        MimeMessage _message;
        private GmailAction.MarkSeenAction markSeenAction;
        private GmailAction.ReplyAction _replyAction;
        public Gmail(MimeMessage message, GmailAction.ReplyAction m1)
        {
            _type = Model.TYPE_MESSAGE.MAIL;
            _replyAction = m1;
            this._message = message;
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

        public string GetDesAddress()
        {
            return _desAddress;
        }

        public void MarkSeen()
        {
            
        }

        public void Reply(string msg)
        {
            _replyAction(msg, _addrSender,_message);
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

        public void SetDesAddress(string des)
        {
            _desAddress = des;
        }
    }
}
