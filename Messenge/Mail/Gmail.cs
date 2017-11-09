using System;
using System.Collections.Generic;
namespace FreeHand
{
    public class Gmail : Model.IMessengeData
    {
        private List<string> _nameSender;
        private List<string> _addrSender;
        private string _content;
        public Gmail()
        {
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
            throw new NotImplementedException();
        }    
              

        public string GetNameSender()
        {
            string result = "";
            foreach (var item in _nameSender)
                result = result + item + " ";
            return result;
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
