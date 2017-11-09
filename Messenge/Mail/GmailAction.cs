using System;
using System.Collections.Generic;
using MimeKit;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit.Search;
using Android.Util; 
namespace FreeHand
{    
    public class GmailAction : IMailAction
    {
        private static readonly string TAG = "GmailAction";
        private ImapClient client;
        private string usr, pwd;
        public GmailAction(string usr,string pwd)
        {
            this.usr = usr;
            this.pwd = pwd;
        }

        public void Login()
        {
            try{
                if (client == null) client = new ImapClient();
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                    client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

                    // disable OAuth2 authentication unless you are actually using an access_token
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(usr, pwd);
                                    
                                    

            }
            catch (Exception e){
                Log.Error(TAG, "Login err "+e.ToString());
            }
        }

        public List<Model.IMessengeData> SyncInbox()
        {
            List<Model.IMessengeData> lstInbox = new List<Model.IMessengeData>();
            if (client.IsConnected)
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);
                //for (int i = 0; i < inbox.Count; i++)
                //{
                //    var message = inbox.GetMessage(i);
                //    Console.WriteLine("Subject: {0}", message.Subject);
                //}
                //Get unread mess
                foreach (var uid in inbox.Search(SearchQuery.NotSeen))
                {
                    Model.IMessengeData mailData = new Gmail();
                    var message = inbox.GetMessage(uid);
                    Console.WriteLine("Subject: {0}", message.Subject);
                    foreach (var mailbox in message.From.Mailboxes)
                    {
                        mailData.SetNameSender(mailbox.Name);
                        mailData.SetAddrSender(mailbox.Address);
                    }                    
                    //mailData.SetNameSender(message.Sender.Name);
                    mailData.SetMessengeContent(message.Subject);
                    lstInbox.Add(mailData);
                }
            }
            else {
                Log.Info(TAG,"Sync mail not run, not connect to server");
            }
            return lstInbox;
        }

        public void Logout(){
            client.Disconnect(true);
        }

        public bool isLogin()
        {
            if (client == null) return false;
            return client.IsConnected;
        }
    }
}
