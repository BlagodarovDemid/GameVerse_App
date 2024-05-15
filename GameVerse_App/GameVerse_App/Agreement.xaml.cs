using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameVerse_App
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Agreement : ContentPage
	{
		public Agreement ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void AgreementButton_Pressed(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }
    }
}