using System;
namespace FreeHand.Model
{
    public class SMSData : IMessengeData
    {
        private string _smsContent { get; set;}
        private string _smsNumber { get; set; }
        private string _smsNameSender { get; set;}
        public SMSData(string number,string content)
        {
            _smsContent = content;
            _smsNumber = number;
            //_smsNameSender = name; 
            //TODO
            //need function get name from contact
        }
        public string GetMessengeContent()
        {
            return _smsContent;
        }
        public string GetNameSender()
        {
            return _smsNameSender;
        }
        public string GetAddrSender()
        {
            return _smsNumber;
        }
        public string Reply(string msg)
        {
            return "";
        }
    }
}
