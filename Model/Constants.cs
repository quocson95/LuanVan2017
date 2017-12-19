using System;
namespace FreeHand.Model
{
    public static class Constants
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds

        public const string ACTION_START_MAIN_SERVICE = "FreeHand.action.ACTION_START_MAIN_SERVICE";
        public const string ACTION_STOP_MAINSERVICE = "FreeHand.action.ACTION_STOP_MAINSERVICE";

        public const string ACTION_START_MESSAGE_SERVICE = "FreeHand.action.ACTION_START_MESSAGE_SERVICE";
        public const string ACTION_STOP_MESSAGE_SERVICE = "FreeHand.action.ACTION_STOP_MESSAGE_SERVICE";

        public const string ACTION_START_SMS_SERVICE = "FreeHand.action.ACTION_START_SMS_SERVICE";
        public const string ACTION_STOP_SMS_SERVICE = "FreeHand.action.ACTION_STOP_SMS_SERVICE";

        public const string ACTION_START_MAIL_SERVICE = "FreeHand.action.ACTION_START_MAIL_SERVICE";
        public const string ACTION_STOP_MAIL_SERVICE = "FreeHand.action.ACTION_STOP_MAIL_SERVICE";

        public const string ACTION_START_PHONE_SERVICE = "FreeHand.action.ACTION_START_PHONE_SERVICE";
        public const string ACTION_STOP_PHONE_SERVICE = "FreeHand.action.ACTION_STOP_PHONE_SERVICE";

        public const string ACTION_START_PHONE_SMART_ALERT = "FreeHand.action.ACTION_START_PHONE_SMART_ALERT";
        public const string ACTION_STOP_PHONE_SMART_ALERT = "FreeHand.action.ACTION_STOP_PHONE_SMART_ALERT";

        //Request Code
        public const int CODE_SETTING_CONTENT_REPLY = 20000;
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
