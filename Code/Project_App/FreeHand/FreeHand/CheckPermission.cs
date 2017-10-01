
using System;

using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Util;
using Android.Support.V4.App;

namespace FreeHand
{
    [Activity(Label = "CheckPermission")]
    public class CheckPermission : Activity, ActivityCompat.IOnRequestPermissionsResultCallback
	{
        private static readonly string TAG = "CheckPermission";
        static readonly string[] PERMISSIONS_CONTACT = {
                Manifest.Permission.ReadContacts               
            };
        Button btn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_Testing);
            //RequestPhoneBookPermission();
            btn = (Button)FindViewById(Resource.Id.btn_getVoices);
            btn.Click += delegate {
                RequestPhoneBookPermission();
            };
            // Create your application here

        }

        void  RequestPhoneBookPermission()
        {
            if (ActivityCompat.CheckSelfPermission(this,
                 Manifest.Permission.ReadContacts)
                != Permission.Granted)
            {
                Log.Info(TAG, "ReadContacts permission NOT has granted");
				if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadContacts))
				{
					// Provide an additional rationale to the user if the permission was not granted
					// and the user would benefit from additional context for the use of the permission.
					// For example if the user has previously denied the permission.
					Log.Info(TAG, "Displaying camera permission rationale to provide additional context.");

					//Snackbar.Make(layout,"Check",
					//Snackbar.LengthIndefinite).SetAction("OK", new Action<View>(delegate (View obj) {
					//  ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadContacts }, 0);
					//})).Show();
				}
                else
                {
					Log.Info(TAG, "CheckPermission permission has NOT been granted");
					// Camera permission has not been granted yet. Request it directly.
					ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadContacts }, 1);
				}
            }
            else Log.Info(TAG, "ReadContacts permission has granted");
        }
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == 1)
			{
                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts) == (int)Permission.Granted)
					{
					    Log.Info(TAG, "ReadContacts permission has granted");
					}
                else 
                {
                    Log.Info(TAG, "ReadContacts permissions were NOT granted.");
                }               								

			}
			else
			{
				base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			}
		}
    }
}