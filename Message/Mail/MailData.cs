using System;
namespace FreeHand.Message.Mail
{
    public class MailData
    {
        string nameLogin;
        string nameDisplay;
        bool isActive;
        public MailData()
        {
            nameLogin = "";
            nameDisplay = "";
            isActive = true;
        }

        public MailData(string nameLogin, string nameDisplay)
        {
            this.nameLogin = nameLogin;
            this.nameDisplay = nameDisplay;
            isActive = true;
        }
    }
}
