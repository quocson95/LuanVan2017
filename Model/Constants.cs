using System;
namespace FreeHand.Model
{
    public static class Constants
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds

        public const string ACTION_START = "FreeHand.action.ACTION_START";
        public const string ACTION_STOP = "FreeHand.action.ACTION_STOP";

        //Request Code
        public const int CODE_SETTING_CONTENT_REPLY_SMS = 20000;
        public const int CODE_BLOCK_SMS_NUMBER = 20001;

        public const int CODE_SETTING_CONTENT_REPLY_MAIL = 30000;

        public const int CODE_PICK_CONTACT = 4000;

        public enum TYPE {
            SMS,
            PHONE,
            MAIL
        }


    }
}
