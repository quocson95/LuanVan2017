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
using Android.Gms.Auth;
using System.Threading.Tasks;
using Auth0.SDK;
namespace FreeHand.Messenge.Mail
{
    [Activity(Label = "MailSetting")]
    public class MailSetting : Activity, View.IOnClickListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        const string TAG = "MainActivity";

        const int RC_SIGN_IN = 9001;

        const string KEY_IS_RESOLVING = "is_resolving";
        const string KEY_SHOULD_RESOLVE = "should_resolve";
        Auth0Client auth0;
        GoogleApiClient mGoogleApiClient;

        TextView mStatus;

        bool mIsResolving = false;

        bool mShouldResolve = false;

        string b;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Mail_Setting_Layout);

            if (savedInstanceState != null)
            {
                mIsResolving = savedInstanceState.GetBoolean(KEY_IS_RESOLVING);
                mShouldResolve = savedInstanceState.GetBoolean(KEY_SHOULD_RESOLVE);
            }

            FindViewById(Resource.Id.btn_login).SetOnClickListener(this);
           
                       

            mStatus = FindViewById<TextView>(Resource.Id.status);

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(PlusClass.API)
                                                  .AddScope(new Scope("https://mail.google.com/"))
                .Build();
        }

        void UpdateUI(bool isSignedIn)
        {
            if (isSignedIn)
            {
                var person = PlusClass.PeopleApi.GetCurrentPerson(mGoogleApiClient);
                var name = string.Empty;
                if (person != null)
                    name = person.DisplayName;
                
                var a = PlusClass.AccountApi.GetAccountName(mGoogleApiClient);
                Task configWork = new Task(() =>
                {
                    b = GoogleAuthUtil.GetToken(this, PlusClass.AccountApi.GetAccountName(mGoogleApiClient), "oauth2:https://www.googleapis.com/auth/gmail.compose");
                });
                configWork.Start();

            }

        }

        protected override void OnStart()
        {
            base.OnStart();
            mGoogleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();
            mGoogleApiClient.Disconnect();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(KEY_IS_RESOLVING, mIsResolving);
            outState.PutBoolean(KEY_SHOULD_RESOLVE, mIsResolving);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

            if (requestCode == RC_SIGN_IN)
            {
                if (resultCode != Result.Ok)
                {
                    mShouldResolve = false;
                }

                mIsResolving = false;
                mGoogleApiClient.Connect();
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log.Debug(TAG, "onConnected:" + connectionHint);

            UpdateUI(true);
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Warn(TAG, "onConnectionSuspended:" + cause);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Debug(TAG, "onConnectionFailed:" + result);

            if (!mIsResolving && mShouldResolve)
            {
                if (result.HasResolution)
                {
                    try
                    {
                        result.StartResolutionForResult(this, RC_SIGN_IN);
                        mIsResolving = true;
                    }
                    catch (IntentSender.SendIntentException e)
                    {
                        Log.Error(TAG, "Could not resolve ConnectionResult.", e);
                        mIsResolving = false;
                        mGoogleApiClient.Connect();
                    }
                }
                else
                {
                    ShowErrorDialog(result);
                }
            }
            else
            {
                UpdateUI(false);
            }
        }

        class DialogInterfaceOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
        {
            public Action<IDialogInterface> OnCancelImpl { get; set; }

            public void OnCancel(IDialogInterface dialog)
            {
                OnCancelImpl(dialog);
            }
        }

        void ShowErrorDialog(ConnectionResult connectionResult)
        {
            int errorCode = connectionResult.ErrorCode;

            if (GooglePlayServicesUtil.IsUserRecoverableError(errorCode))
            {
                var listener = new DialogInterfaceOnCancelListener();
                listener.OnCancelImpl = (dialog) => {
                    mShouldResolve = false;
                    UpdateUI(false);
                };
                GooglePlayServicesUtil.GetErrorDialog(errorCode, this, RC_SIGN_IN, listener).Show();
            }
            else
            {                               
                mShouldResolve = false;
                UpdateUI(false);
            }
        }

        async void View.IOnClickListener.OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btn_login:
                    mShouldResolve = true;
                    //mGoogleApiClient.Connect();

                    auth0 = new Auth0Client(
                        "sondq.auth0.com",
                        "OL6_d7S_Cgx04ep-47WsV7C29ZkDnZ2W");
                    var user = await auth0.LoginAsync(this, "google-oauth2");
                    break;
            }
            //    case Resource.Id.sign_out_button:
            //        if (mGoogleApiClient.IsConnected)
            //        {
            //            PlusClass.AccountApi.ClearDefaultAccount(mGoogleApiClient);
            //            mGoogleApiClient.Disconnect();
            //        }
            //        UpdateUI(false);
            //        break;
            //    case Resource.Id.disconnect_button:
            //        if (mGoogleApiClient.IsConnected)
            //        {
            //            PlusClass.AccountApi.ClearDefaultAccount(mGoogleApiClient);
            //            await PlusClass.AccountApi.RevokeAccessAndDisconnect(mGoogleApiClient);
            //            mGoogleApiClient.Disconnect();
            //        }
            //        UpdateUI(false);
            //        break;
            //}
        }
    }

}

