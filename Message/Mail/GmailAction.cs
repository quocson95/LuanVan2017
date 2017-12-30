using System;

using Android.Util;
using MailKit.Net.Imap;
using FreeHand.Model;
using System.Collections.Generic;

using MailKit;
using MailKit.Search;
using MailKit.Security;
using Xamarin.Auth;

namespace FreeHand.Message.Mail
{
    public class GmailAction : Mail.IMailAction
    {
        private static readonly string TAG = typeof(GmailAction).FullName;
        private ImapClient client;
        string _type = "Gmail";
        bool isActive;
        public delegate void MarkSeenAction(MailKit.UniqueId uid);
        private MarkSeenAction markSeenAction;

        Account _account;
        string _email;

        public GmailAction(string email, Account account )
        {
            _email = email;
            _account = account;
            client = new ImapClient();
        }
        private void MarkSeen(MailKit.UniqueId uid)
        {
            var inbox = client.Inbox;
            inbox.AddFlags(uid, MessageFlags.Seen, true);
        }

        public string GetType()
        {
            return _type;
        }
        public void Login()
        {
            Log.Info(TAG, "Login");
            try
            {
                client.Connect("imap.gmail.com", 993, true);

                // use the access token as the password string
                client.Authenticate(_email, _account.Properties["access_token"]);
                client.Inbox.Check();
                var folder = client.Inbox;
                folder.Status(StatusItems.Count | StatusItems.Unread);
                int total = folder.Count;
                int unread = folder.Unread;
                Console.WriteLine("total  {0} \nunread {1}", total, unread);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Login err " + e);
            }

        }



        public List<IMessengeData> SyncInbox()
        {
            Log.Info(TAG, "SyncInbox");

            List<IMessengeData> lstInbox = new List<IMessengeData>();
            if (isLogin())
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                foreach (var uid in inbox.Search(SearchQuery.NotSeen))
                {
                    IMessengeData mailData = new Gmail(uid, markSeenAction);
                    var message = inbox.GetMessage(uid);
                    Console.WriteLine("Subject: {0}", message.Subject);
                    foreach (var mailbox in message.From.Mailboxes)
                    {
                        mailData.SetNameSender(mailbox.Name);
                        mailData.SetAddrSender(mailbox.Address);
                    }

                    mailData.SetMessengeContent(message.Subject);
                    lstInbox.Add(mailData);
                }
            }
            else
            {
                Log.Info(TAG, "Sync mail not run, not connect to server try login");
            }

            return lstInbox;
        }

        public void Logout()
        {
            if (isLogin())
            {
                Log.Info(TAG, "Logout");
                client.Disconnect(true);
            }
            else
                Log.Info(TAG, "Client is not login, can't disconnect");
        }

        public bool isLogin()
        {
            bool result = false;
            try
            {
                if (client == null) result = false;
                else if (!client.IsAuthenticated) result = false;
                else result = client.IsConnected;
            }
            catch (Exception e)
            {
                Log.Error(TAG, e.ToString());
            }
            return result;
        }

        public string GetNameLogin()
        {
            return _email;
        }

        public bool GetActive()
        {
            return isActive;
        }

        public bool SetActive(bool active) => this.isActive = active;
               
        public void Reply()
        {
            throw new NotImplementedException();
        }

        public void Login_v2(string email,string token)
        {
            
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

                // use the access token as the password string
                client.Authenticate(email, token);
                var folder = client.Inbox;
                folder.Status(StatusItems.Count | StatusItems.Unread);
                int total = folder.Count;
                int unread = folder.Unread;
                Console.WriteLine("total  {0} \nunread {1}", total, unread);
            }
        }
    }
}
