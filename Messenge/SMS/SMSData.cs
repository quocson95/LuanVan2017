using System;
using Android.Telephony.Gsm;

namespace FreeHand.Model
{
    public class SMSData : IMessengeData
    {
        private string _content { get; set;}
        private string _number { get; set; }
        private string _nameSender { get; set;}
        private TYPE_MESSAGE _type;

        public SMSData(string number,string content)
        {
            _content = content;
            _number = number;
            _type = TYPE_MESSAGE.SMS;
            //_smsNameSender = name; 
            //TODO
            //need function get name from contact
        }
        public void SetNameSender(string name){
            _nameSender = name;
        }

        public string GetMessengeContent()
        {
            return _content;
        }
        public string GetNameSender()
        {
            return _nameSender;
        }
        public string GetAddrSender()
        {
            return _number;
        }
        public string Reply(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
                SmsManager.Default.SendTextMessage(_number, null, msg, null, null);
            else
                Console.WriteLine("message reply is null or empty");
            return "";
        }

        public void SetMessengeContent(string name)
        {
            throw new NotImplementedException();
        }

        public void SetAddrSender(string name)
        {
            throw new NotImplementedException();
        }

        public void MarkSeen()
        {
            throw new NotImplementedException();
        }

        public TYPE_MESSAGE Type()
        {
            return _type;
        }
               
    }
}
