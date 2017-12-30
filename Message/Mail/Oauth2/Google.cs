using System;
using System.Text;
using Android.Content;
using Xamarin.Auth;

namespace FreeHand.Message.Mail.Oauth2
{
    public class Google
    {
        Context _context;

        public event EventHandler<AuthenticatorCompletedEventArgs> Completed;

        public Google(Context context)
        {
            this._context = context;
        }

        public void authenticate()
        {
            string ClientId = _context.GetString(Resource.String.client_id_google_service);
            string RedirectUri = _context.GetString(Resource.String.redirect_url_google_service);
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
    }
}
