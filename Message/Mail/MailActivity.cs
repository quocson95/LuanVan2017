using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using OpenId.AppAuth;
using Xamarin.Auth;
namespace FreeHand.Message.Mail
{
    [Activity(Label = "MailActivity")]
   
    public class MailActivity :Activity
    {
        private static string DiscoveryEndpoint = "https://accounts.google.com/.well-known/openid-configuration";

        private static string AuthEndpoint = null; // auth endpoint is discovered
        private static string TokenEndpoint = null; // token endpoint is discovered
        private static string RegistrationEndpoint = null; // dynamic registration not supported
        AccountStore store;
        Account account;
        readonly string TAG = typeof(MailActivity).FullName;
        const int RC_SIGN_IN = 9001;
        Button login,a;
        Oauth2.Google google;
        MailSerivce mailService;
        private AuthorizationService authService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mail_Setting_Layout);
                       
            login = FindViewById<Button>(Resource.Id.btn_login);
            a = FindViewById<Button>(Resource.Id.btn_sync_mail);

            login.Click += delegate {
                google = new Oauth2.Google(this);
                google.Completed += (sender, e) => {
                    Console.WriteLine("this is custom");
                };
                google.authenticate();

            };
            a.Click += delegate {              
                v3();

            };

            //authService = new AuthorizationService(this);
            store = AccountStore.Create();
                     
        }

        protected override void OnStart()
        {            
            base.OnStart();
            //custom_tab_activity_helper.BindCustomTabsService(this);
        }

        protected override void OnStop()
        {
            base.OnStop();

            // Step 2.2 Customizing the UI - Native UI [OPTIONAL]
            // [Chrome] Custom Tabs WarmUp and prefetch
            //custom_tab_activity_helper.UnbindCustomTabsService(this);

            return;
        }
        private void v3()
        {
            string ClientId = this.GetString(Resource.String.client_id_google_service);
            string RedirectUri = this.GetString(Resource.String.redirect_url_google_service);
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
                    accessTokenUrl : new Uri(Model.Constants.AccessTokenUrl),
                    getUsernameAsync: null,
                                        // Native UI API switch
                                        //      true    - NEW native UI support 
                                        //      false   - OLD embedded browser API [DEFAULT]
                                        // DEFAULT will be switched to true in the near future 2017-04
                    isUsingNativeUI: true

                                    )
            {
                
            };
            Auth2.Completed += OnAuthCompleted;
            Auth2.Error += OnAuthError;
            Auth2.BrowsingCompleted += Auth_BrowsingCompleted;
            if (Auth2.IsUsingNativeUI == true)
            {
                // Step 2.2 Customizing the UI - Native UI [OPTIONAL]
                // In order to access CustomTabs API 

                Xamarin.Auth.CustomTabsConfiguration.AreAnimationsUsed = true;
                Xamarin.Auth.CustomTabsConfiguration.IsShowTitleUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.IsUrlBarHidingUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.IsCloseButtonIconUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.IsActionButtonUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.IsActionBarToolbarIconUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.IsDefaultShareMenuItemUsed = false;
                Xamarin.Auth.CustomTabsConfiguration.MenuItemTitle = null;
                //CustomTabsConfiguration.CustomTabsClosingMessage = null;
                Xamarin.Auth.CustomTabsConfiguration.ToolbarColor = new Android.Graphics.Color(Resource.Color.zaloColor); 
            }

            // Step 3 Present/Launch the Login UI
            //StartActivity(ui_object);
            AuthenticationState.Authenticator = Auth2;
            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(Auth2);
            return;
        }


        private void Auth_BrowsingCompleted(object sender, EventArgs e)
        {
            string title = "OAuth Browsing Completed";
            string msg = "";

            StringBuilder sb = new StringBuilder();
            msg = sb.ToString();
            Console.WriteLine("Auth_BrowsingCompleted "+msg);
        }

        private void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Message  = ").Append(e.Message)
                                    .Append(System.Environment.NewLine);
            string msg = sb.ToString();
            Console.WriteLine("Auth_Error: "+msg);
        }

        private async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            Console.WriteLine("Auth_Completed");
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            if (!e.IsAuthenticated)
            {
                Console.WriteLine("Auth_Completed:  not Authenticated ");
            }
            else
            {                
                string token = default(string);
                if (null != e.Account)
                {
                    string token_name = default(string);
                    Type t = sender.GetType();
                    if (t == typeof(Xamarin.Auth.OAuth2Authenticator))
                    {
                        token_name = "access_token";
                        token = e.Account.Properties[token_name];
                    }
                    else if (t == typeof(Xamarin.Auth.OAuth1Authenticator))
                    {
                        token_name = "oauth_token";
                        token = e.Account.Properties[token_name];
                    }
                    Console.WriteLine(token_name + ": " + token);
                }

                User user = null;
                var request = new OAuth2Request("GET", new Uri(Model.Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }

                if (account != null)
                {
                    store.Delete(account, this.GetString(Resource.String.app_name));
                }

                account = e.Account;
                await store.SaveAsync(account, "aa");
                Console.WriteLine(account.ToString());
                //gmail.Login_v2(user.Email,account.Properties["access_token"]);

                //Dictionary<string, string> dictionary = new Dictionary<string, string> { { "refresh_token", e.Account.Properties["refresh_token"] }, { "client_id", ClientId}, { "grant_type", "refresh_token" } };
                //var request2 = new OAuth2Request("POST", new Uri(Model.Constants.RefreshToken), dictionary, e.Account);
                //var response2 = await request2.GetResponseAsync();
                //string RefreshToken = await response2.GetResponseTextAsync();
                //Console.WriteLine(RefreshToken);

            }
        }                     
       
    }
}
