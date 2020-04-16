using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PPEMobile
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public ICommand AnimationClickedCommand { get; set; }

        public MainPage()
        {
            AnimationClickedCommand = new Command(() =>
            {
                StartButtonClickedAsync();
            });
            BindingContext = this; // pour l'animation

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void StartButtonClicked(object sender, EventArgs e)
        {
            StartButtonClickedAsync();
        }

        private async Task StartButtonClickedAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Erreur", "La caméra n'est pas disponible", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium // Permet de mettre la photo en plus petite taille -> améliore la performance
            });

            if (file == null)
            {
                await DisplayAlert("Erreur", "Pas de fichier", "OK");
                return;
            }

            await Navigation.PushAsync(new ScannerPage(file), false); // Si c réussi on navigue sur la scannerpage
                
            //await DisplayAlert("Reussi", file.Path, "OK");

            //image.Source = ImageSource.FromStream(() =>
            //{
            //    var stream = file.GetStream();
            //    return stream;
            //});
        }
    }
}
