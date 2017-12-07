using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Plus;
using MailKit.Net.Imap;
using FreeHand.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit;
using Google.Apis.Gmail.v1;
using MailKit.Search;
using MailKit.Security;

namespace FreeHand
{    
    public class GmailAction : IMailAction
    {
        private static readonly string TAG = "GmailAction";
        private ImapClient client;
        private string usr, pwd;
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";

        public delegate void MarkSeenAction(MailKit.UniqueId uid);
        private MarkSeenAction markSeenAction;
        public GmailAction(string usr,string pwd)
        {
            this.usr = usr;
            this.pwd = pwd;
            markSeenAction = MarkSeen;
        }

        private void MarkSeen(MailKit.UniqueId uid){
            var inbox = client.Inbox;
            inbox.AddFlags(uid, MessageFlags.Seen, true);
        }

        public async Task Login(Context ca)
        {
           
            try
            {
                using (var client = new ImapClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                    client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

                    // disable OAuth2 authentication unless you are actually using an access_token
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate("user@gmail.com", "password");

                    // do stuff...

                    client.Disconnect(true);
                }
               





            }
            catch (Exception e)
            {
                Log.Error(TAG, "Login err " + e.ToString());
            }
           
        }

       

        public List<Model.IMessengeData> SyncInbox()
        {
            Log.Info(TAG, "Sync Inbox GMAIL");
            List<Model.IMessengeData> lstInbox = new List<Model.IMessengeData>();
            if (client.IsConnected)
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
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
                    Model.IMessengeData mailData = new Gmail(uid,markSeenAction);
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
                Log.Info(TAG,"Sync mail not run, not connect to server try login");

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
