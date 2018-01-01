using System;
namespace FreeHand.Model
{
    public static class Constants
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        public const int DELAY_BETWEEN_SYNC_MAIL = 10000; //minilseconds

        public const string ACTION_START_MAIN_SERVICE = "FreeHand.action.ACTION_START_MAIN_SERVICE";
        public const string ACTION_STOP_MAIN_SERVICE = "FreeHand.action.ACTION_STOP_MAIN_SERVICE";

        public const string ACTION_START_MESSAGE_SERVICE = "FreeHand.action.ACTION_START_MESSAGE_SERVICE";
        public const string ACTION_STOP_MESSAGE_SERVICE = "FreeHand.action.ACTION_STOP_MESSAGE_SERVICE";

        public const string ACTION_START_SMS_SERVICE = "FreeHand.action.ACTION_START_SMS_SERVICE";
        public const string ACTION_STOP_SMS_SERVICE = "FreeHand.action.ACTION_STOP_SMS_SERVICE";

        public const string ACTION_START_MAIL_SERVICE = "FreeHand.action.ACTION_START_MAIL_SERVICE";
        public const string ACTION_STOP_MAIL_SERVICE = "FreeHand.action.ACTION_STOP_MAIL_SERVICE";
        public const string ACTION_ADD_ACCOUNT_MAIL_SERVICE = "FreeHand.action.ACTION_ADD_ACCOUNT_MAIL_SERVICE";
        public const string ACTION_DEL_ACCOUNT_MAIL_SERVICE = "FreeHand.action.ACTION_DEL_ACCOUNT_MAIL_SERVICE";


        public const string ACTION_START_PHONE_SERVICE = "FreeHand.action.ACTION_START_PHONE_SERVICE";
        public const string ACTION_STOP_PHONE_SERVICE = "FreeHand.action.ACTION_STOP_PHONE_SERVICE";

        public const string ACTION_START_PHONE_SMART_ALERT = "FreeHand.action.ACTION_START_PHONE_SMART_ALERT";
        public const string ACTION_STOP_PHONE_SMART_ALERT = "FreeHand.action.ACTION_STOP_PHONE_SMART_ALERT";



        //Request Code
        public const int CODE_SETTING_CONTENT_REPLY = 20000;
        public const int CODE_BLOCK_SMS_NUMBER = 20001;

        public const int CODE_SETTING_CONTENT_REPLY_MAIL = 30000;

        public const int CODE_PICK_CONTACT = 4000;

        public const int CODE_ADD_ACCOUNT_GOOGLE = 5000;
        public enum TYPE {
            SMS,
            PHONE,
            MAIL
        }


        //Google Service
        private const string Id = "484778695759-bvqmvmdoj6uit61iho1j9j2je8hn5sov";
        public const string ClientID = Id + ".apps.googleusercontent.com";
        public const string CallbackUri = "com.googleusercontent.apps." + Id;
        public const string RedirectUri = CallbackUri + ":/oauth2redirect";



        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v3/userinfo";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://accounts.google.com/o/oauth2/token";
        public static string ScopeMail = "https://mail.google.com/";
        public static string Scopeuserinfo = "email";

        public static string RefreshToken = "https://www.googleapis.com/oauth2/v4/token";
    }
}
