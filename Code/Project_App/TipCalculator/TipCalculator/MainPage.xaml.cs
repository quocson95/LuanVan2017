using System;
using Xamarin.Forms;
namespace TipCalculator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //Remove Title App ; 
            NavigationPage.SetHasNavigationBar(this, false);

        }
        public void OpenSMS_SettingPage(object sender, EventArgs arg){
            Navigation.PushAsync(new SMS_Settings());
            
        }

		public void OpenCall_SettingPage(object sender, EventArgs arg)
		{			
            Navigation.PushAsync(new Call_Setting());

		}
    }
}