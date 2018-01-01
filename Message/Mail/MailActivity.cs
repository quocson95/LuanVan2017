using System;
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
        private static string ClientId = "484778695759-bvqmvmdoj6uit61iho1j9j2je8hn5sov.apps.googleusercontent.com";
        //private static string ClientSecrect = "mdnMeEsStaM3e1j-s2eeCzSp";
        private static string RedirectUri = "com.googleusercontent.apps.484778695759-bvqmvmdoj6uit61iho1j9j2je8hn5sov:/oauth2redirect";

        //private static string RedirectUri = "http://localhost";

        private static string AuthEndpoint = null; // auth endpoint is discovered
        private static string TokenEndpoint = null; // token endpoint is discovered
        private static string RegistrationEndpoint = null; // dynamic registration not supported
        AccountStore store;
        Account account;
        readonly string TAG = typeof(MailActivity).FullName;
        const int RC_SIGN_IN = 9001;
        Button login,a;
        //GmailAction gmail = new GmailAction("","");
        MailSerivce mailService;
        private AuthorizationService authService;
        //Android.Support.CustomTabs.Chromium.SharedUtilities.CustomTabActivityHelper custom_tab_activity_helper = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mail_Setting_Layout);
                       
            login = FindViewById<Button>(Resource.Id.btn_login);
            a = FindViewById<Button>(Resource.Id.btn_sync_mail);

            a.Click += delegate {
                //gmail.Login_v2();
                //v3();
                v4();

            };

            authService = new AuthorizationService(this);
            store = AccountStore.Create();

            login.Click += Login_Click;
            //custom_tab_activity_helper = new global::Android.Support.CustomTabs.Chromium.SharedUtilities.CustomTabActivityHelper();
        }

        private void v4()
        {
            Google google = new Google();
            google.Completed += Google_Completed;
            google.Authenticate();
        }

        void Google_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            GmailAction gmail = new GmailAction("", "");
            gmail.Login_v2("dangquocson1995@gmail.com",e.Account.Properties["access_token"]);
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
                    authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                    redirectUrl: new Uri(RedirectUri),
                    accessTokenUrl : new Uri("https://accounts.google.com/o/oauth2/token"),
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
            Intent ui_object = Auth2.GetUI(this);
            //if (Auth2.IsUsingNativeUI == true)
            //{
            //    // Step 2.2 Customizing the UI - Native UI [OPTIONAL]
            //    // In order to access CustomTabs API 

            //    Xamarin.Auth.CustomTabsConfiguration.AreAnimationsUsed = true;
            //    Xamarin.Auth.CustomTabsConfiguration.IsShowTitleUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.IsUrlBarHidingUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.IsCloseButtonIconUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.IsActionButtonUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.IsActionBarToolbarIconUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.IsDefaultShareMenuItemUsed = false;
            //    Xamarin.Auth.CustomTabsConfiguration.MenuItemTitle = null;
            //    Xamarin.Auth.CustomTabsConfiguration.ToolbarColor = new Android.Graphics.Color(Resource.Color.zaloColor); 
            //}

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
                await store.SaveAsync(account, this.GetString(Resource.String.app_name));
                Console.WriteLine(account.ToString());
                GmailAction gmail = new GmailAction("", "");
                gmail.Login_v2(user.Email,account.Properties["access_token"]);
            }
        }

        async void Login_Click(object sender, EventArgs e)
        {         
            Console.WriteLine("initiating auth...");

            try
            {
                AuthorizationServiceConfiguration serviceConfiguration;
                if (DiscoveryEndpoint != null)
                {
                    serviceConfiguration = await AuthorizationServiceConfiguration.FetchFromUrlAsync(
                        Android.Net.Uri.Parse(DiscoveryEndpoint));
                }
                else
                {
                    serviceConfiguration = new AuthorizationServiceConfiguration(
                        Android.Net.Uri.Parse(AuthEndpoint),
                        Android.Net.Uri.Parse(TokenEndpoint),
                        Android.Net.Uri.Parse(RegistrationEndpoint));
                }

                Console.WriteLine("configuration retrieved, proceeding");
                if (ClientId == null)
                {
                    // Do dynamic client registration if no client_id
                    MakeRegistrationRequest(serviceConfiguration);
                }
                else
                {
                    MakeAuthRequest(serviceConfiguration, new AuthState());
                }
            }
            catch (AuthorizationException ex)
            {
                Console.WriteLine("Failed to retrieve configuration:" + ex);
            }

        }

        private async void MakeRegistrationRequest(AuthorizationServiceConfiguration serviceConfig)
        {
            var registrationRequest = new RegistrationRequest.Builder(serviceConfig, new[] { Android.Net.Uri.Parse(RedirectUri) })
                .SetTokenEndpointAuthenticationMethod(ClientSecretBasic.Name)
                .Build();

            Console.WriteLine("Making registration request to " + serviceConfig.RegistrationEndpoint);

            try
            {
                var registrationResponse = await authService.PerformRegistrationRequestAsync(registrationRequest);
                Console.WriteLine("Registration request complete");

                if (registrationResponse != null)
                {
                    ClientId = registrationResponse.ClientId;
                    Console.WriteLine("Registration request complete successfully");
                    // Continue with the authentication
                    MakeAuthRequest(registrationResponse.Request.Configuration, new AuthState((registrationResponse)));
                }
            }
            catch (AuthorizationException ex)
            {
                Console.WriteLine("Registration request had an error: " + ex);
            }
        }
        private void MakeAuthRequest(AuthorizationServiceConfiguration serviceConfig, AuthState authState)
        {
            var authRequest = new AuthorizationRequest.Builder(serviceConfig, ClientId, ResponseTypeValues.Code, Android.Net.Uri.Parse(RedirectUri))
                                                      .SetScope("https://mail.google.com/")
                .Build();            
            Console.WriteLine("Making auth request to " + serviceConfig.AuthorizationEndpoint);
            authService.PerformAuthorizationRequest(
                authRequest,
                TokenActivity.CreatePostAuthorizationIntent(this, authRequest, serviceConfig.DiscoveryDoc, authState),
                authService.CreateCustomTabsIntentBuilder().Build());            
        }


       
    }
}
