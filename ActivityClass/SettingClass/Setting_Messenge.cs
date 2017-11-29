
using Android.App;
using Android.OS;
using FreeHand;

namespace FreeHand.ActivityClass.SettingClass
{
    [Activity(Label = "Setting_Messenge")]
    public class Setting_Messenge : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setting_Messenge_Layout);
            // Create your application here
        }
    }
}
