using Machina;
using Machina.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PPEMobile.Page
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            ApiService apiService = new ApiService();
            bool response = await apiService.LoginUser(EntEmail.Text, EnterPassword.Text);
            if (!response)
            {
                await DisplayAlert("Erreur", "Connection impossible" + response, "ok");
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }

        private void SignUp(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignUpPage());
        }
        
    }
}