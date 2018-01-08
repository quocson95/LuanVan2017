using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace FreeHand.Message.Mail
{
    public class Google
    {
        public static readonly string TAG = typeof(Google).FullName;

        public event EventHandler<AuthenticatorCompletedEventArgs> Completed;

        public Google()
        {
        }


        public void Authenticate()
        {
            string ClientId = Model.Constants.ClientID;
            string RedirectUri = Model.Constants.RedirectUri;
            Console.WriteLine("V3-V3");
            OAuth2Authenticator Auth2 = null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Model.Constants.ScopeMail);
            stringBuilder.Append(" ");
            stringBuilder.Append(Model.Constants.Scopeuserinfo);
            Auth2 = new OAuth2Authenticator
                                    (
                    clientId: ClientId,
                    clientSecret: null,
                    scope: stringBuilder.ToString(),
                    authorizeUrl: new Uri(Model.Constants.AuthorizeUrl),
                    redirectUrl: new Uri(RedirectUri),
                    accessTokenUrl: new Uri(Model.Constants.AccessTokenUrl),
                    getUsernameAsync: null,
                    // Native UI API switch
                    //      true    - NEW native UI support 
                    //      false   - OLD embedded browser API [DEFAULT]
                    // DEFAULT will be switched to true in the near future 2017-04
                    isUsingNativeUI: true

                                    )
            {

            };
            Auth2.Completed += Completed;
            Auth2.Error += OnAuthError;
            if (Auth2.IsUsingNativeUI == true)
            {
                // Step 2.2 Customizing the UI - Native UI [OPTIONAL]
                // In order to access CustomTabs API 

                CustomTabsConfiguration.AreAnimationsUsed = true;
                CustomTabsConfiguration.IsShowTitleUsed = false;
                CustomTabsConfiguration.IsUrlBarHidingUsed = false;
                CustomTabsConfiguration.IsCloseButtonIconUsed = false;
                CustomTabsConfiguration.IsActionButtonUsed = false;
                CustomTabsConfiguration.IsActionBarToolbarIconUsed = false;
                CustomTabsConfiguration.IsDefaultShareMenuItemUsed = false;
                CustomTabsConfiguration.MenuItemTitle = null;
                //CustomTabsConfiguration.CustomTabsClosingMessage = null;
                CustomTabsConfiguration.ToolbarColor = new Android.Graphics.Color(Resource.Color.zaloColor);
            }

            // Step 3 Present/Launch the Login UI
            //StartActivity(ui_object);
            AuthenticationState.Authenticator = Auth2;
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(Auth2);
        }

        private void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Message  = ").Append(e.Message)
                                    .Append(System.Environment.NewLine);
            string msg = sb.ToString();
            Console.WriteLine("Auth_Error: " + msg);
        }

        public static async Task<Account> RefreshToken(Account account)
        {
            RefreshAccount refreshAccount;
            Account newAccount = null;
            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "refresh_token", account.Properties["refresh_token"] }, { "client_id", Model.Constants.ClientID }, { "grant_type", "refresh_token" } };
            var request = new OAuth2Request("POST", new Uri(Model.Constants.AccessTokenUrl), dictionary, account);
            var response = await request.GetResponseAsync();
            if (response != null && response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                string accountJson = response.GetResponseText();
                refreshAccount = JsonConvert.DeserializeObject<RefreshAccount>(accountJson);
                var properties = account.Properties;
                properties["access_token"] = refreshAccount.AccessToken;
                newAccount = new Account("", properties, account.Cookies);

            }
            Log.Info(TAG,"Refresh token {0}",newAccount.ToString());
            return newAccount;
        }
    }

}
