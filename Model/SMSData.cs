﻿using System;
using Android.Telephony.Gsm;
using Android.Content;
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
        public void SetNameSender(string name){
            _smsNameSender = name;
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
            SmsManager.Default.SendTextMessage(_smsNumber, null,msg, null, null);			
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
    }
}
