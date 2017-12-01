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
        public const int Code_Setting_Messenge_SMS = 20000;
        public const int Code_Setting_Messenge_MAIL = 20001;

    }
}
