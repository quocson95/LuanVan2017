using System;

using Android.Util;
using MailKit.Net.Imap;
using FreeHand.Model;
using System.Collections.Generic;

using MailKit;
using MailKit.Search;
using MailKit.Security;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using System.Linq;
using MailKit.Net.Smtp;

namespace FreeHand.Message.Mail
{
    public class GmailAction : Mail.IMailAction
    {
        private static readonly string TAG = typeof(GmailAction).FullName;
        private ImapClient client;
        private string email, token;
        string _type = "Gmail";
        bool isActive;
        public delegate void MarkSeenAction(MailKit.UniqueId uid);
        private MarkSeenAction markSeenAction;

        public delegate void ReplyAction(string content, IList<string> desAddr,MimeMessage message);
        private ReplyAction replyAction;

        public GmailAction(string usr, string token)
        {
            this.email = usr;
            this.token = token;
            markSeenAction = MarkSeen;
            replyAction = Reply;
            isActive = false;
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
            try
            {
                if (!client.IsConnected)
                {
                    client.Connect("imap.gmail.com", 993, true);
                }

                // use the access token as the password string
                client.Authenticate(email, token);
                var folder = client.Inbox;
                folder.Status(StatusItems.Count | StatusItems.Unread);
                int total = folder.Count;
                int unread = folder.Unread;
                Console.WriteLine("total  {0} \nunread {1}", total, unread);               
            }
            catch(Exception e)
            {
                Log.Debug(TAG,e.Message);
            }

            //Log.Info(TAG, "Login");
            //try
            //{
            //    client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
            //    client.Connect("imap.gmail.com", 993, SecureSocketOptions.Auto);

            //    // disable OAuth2 authentication unless you are actually using an access_token
            //    //client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    client.Authenticate(usr, token);

            //    var folder = client.Inbox;

            //    folder.Status(StatusItems.Count | StatusItems.Unread);
            //    int total = folder.Count;
            //    int unread = folder.Unread;
            //    // do stuff...
            //    Console.WriteLine("total  {0} \nunread {1}", total, unread);
            //}
            //catch (Exception e)
            //{
            //    Log.Error(TAG, "Login err " + e);
            //}

        }



        public async Task<List<IMessengeData>> SyncInbox()
        {
            Log.Info(TAG, "SyncInbox {0}",email);

            List<IMessengeData> lstInbox = new List<IMessengeData>();
            if (isLogin())
            {
                try
                {
                    client.Timeout = 15000;
                    await client.NoOpAsync();
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite);
                    //inbox.Opened += (sender, e) => 
                    //{
                    //    Console.WriteLine("Total messages 2: {0}", inbox.Count);
                    //    Console.WriteLine("Recent messages 2: {0}", inbox.Recent);
                    //};
                    Console.WriteLine("Total messages: {0}", inbox.Count);
                    Console.WriteLine("Recent messages: {0}", inbox.Recent);

                    foreach (var uid in inbox.Search(SearchQuery.NotSeen))
                    {
                        
                        var message = inbox.GetMessage(uid);

                        IMessengeData mailData = new Gmail(message, replyAction);
                        Console.WriteLine("Subject: {0}", message.Subject);
                        foreach (var mailbox in message.From.Mailboxes)
                        {
                            mailData.SetNameSender(mailbox.Name);
                            mailData.SetAddrSender(mailbox.Address);
                        }

                        mailData.SetMessengeContent(message.Subject);
                        mailData.SetDesAddress(this.email);
                        inbox.AddFlags(uid, MessageFlags.Seen, true);
                        lstInbox.Add(mailData);
                    }
                    inbox.Close(false);
                }
                catch(ImapProtocolException ex)
                {
                    Log.Error(TAG,ex.Message);
                }
                catch(Exception e)
                {
                    Log.Error(TAG,e.Message);
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
                client.DisconnectAsync(true);
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
            return email;
        }

        public bool GetActive()
        {
            return isActive;
        }

        public bool SetActive(bool active) => this.isActive = active;

        public string GetPwd()
        {
            return token;
        }

        public void Reply(string msg, IList<string> _addrDes,MimeMessage message)
        {
            var reply = new MimeMessage();
            reply.From.Add(new MailboxAddress(email));

            // reply to the sender of the message
            if (message.ReplyTo.Count > 0)
            {
                reply.To.AddRange(message.ReplyTo);
            }
            else if (message.From.Count > 0)
            {
                reply.To.AddRange(message.From);
            }
            else if (message.Sender != null)
            {
                reply.To.Add(message.Sender);
            }

            // include all of the other original recipients - TODO: remove ourselves from these lists
            //reply.To.AddRange(message.To);
            //reply.Cc.AddRange(message.Cc);

            // set the reply subject
            if (!message.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase))
                reply.Subject = "Re: " + message.Subject;
            else
                reply.Subject = message.Subject;

            // construct the In-Reply-To and References headers
            if (!string.IsNullOrEmpty(message.MessageId))
            {
                reply.InReplyTo = message.MessageId;
                foreach (var id in message.References)
                    reply.References.Add(id);
                reply.References.Add(message.MessageId);
            }

            // quote the original message text
            using (var quoted = new StringWriter())
            {
                var sender = message.Sender ?? message.From.Mailboxes.FirstOrDefault();

                quoted.WriteLine("On {0}, {1} wrote:", message.Date.ToString("f"), !string.IsNullOrEmpty(sender.Name) ? sender.Name : sender.Address);
                using (var reader = new StringReader(message.TextBody))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        quoted.Write("> ");
                        quoted.WriteLine(line);
                    }
                }

                quoted.Write("\n");
                quoted.WriteLine(msg);

                reply.Body = new TextPart("plain")
                {
                    Text = quoted.ToString(),
               };

            }
            using (var client2 = new SmtpClient())
            {
                client2.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

                client2.Authenticate(email, token);

                var options = FormatOptions.Default.Clone();

                if (client2.Capabilities.HasFlag(SmtpCapabilities.UTF8))
                    options.International = true;

                client2.Send(options, reply);

                client.Disconnect(true);
            }

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
                client.Disconnect(true);
            }
        }

       
    }
}
