using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using PPEMobile.Model;
using PPEMobile.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PPEMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        bool processing = true;
        FaceDetectResult faceDetectResult = null;
        public ScannerPage(MediaFile file)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            faceImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });

            startLaserAnimation(); // animation du laser démarrée
            startDetection(file);
            // le fait d'appeler startDetection qui est async permet de sortir de la fonction le temps que startDection s'execute
            //  -> on ne sera pas bloquer

        }

        private async Task startLaserAnimation()
        {
            laserImage.Opacity = 0; // le laser ne sera pas visible
            await Task.Delay(500);
            await laserImage.FadeTo(1, 500); // le laser va apparaitre progressivement

            //PlaySound("scan.wav");
            await laserImage.TranslateTo(0, 360, 1800); // definit comment le laser se deplace
            double y = 0;
            while (processing) // si la co est mauvaise et que ça prend du temps à charger
            {
                //PlayCurrentSound();
                await laserImage.TranslateTo(0, Y, 1800);
                y = (y == 0) ? 360 : 0; // si y = 0, On lui donne la valeur 360, sinon on lui donne la valeur 0
                // permet au laser de monter et descendre
            }

            laserImage.IsVisible = false;
            //PlaySound("result.wav");
            await DisplayResults();
            await ResultsSpeech();
        }

        private async Task startDetection(MediaFile file)
        {
            // Appel de la fonction FaceDetect du dossier Services pour l'analyse du visage
            faceDetectResult = await CognitiveService.FaceDetect(file.GetStreamWithImageRotatedForExternalStorage());
            processing = false;
        }

        private async Task DisplayResults()
        {
            statusLabel.Text = "Analyse terminée";

            if (faceDetectResult == null)
            {
                //faceLabel.Text = "Pas de detection";
                await DisplayAlert("Erreur", "L'analyse n'a pas fonctionné", "OK");
                await Navigation.PopAsync(); // reviens a la page d'accueil
            }
            else
            {
                // On a récupéré les infos du visage
                ageLabel.Text = faceDetectResult.faceAttributes.age.ToString();
                genderLabel.Text = faceDetectResult.faceAttributes.gender.Substring(0, 1).ToUpper(); // Affiche uniquement la première lettre
                infoLayout.IsVisible = true;
                continueButton.Opacity = 1;
            }
        }
        public void ContinueButtonClicked(object sender, EventArgs eventArgs) // boutton pour revenir en arrière
        {
            Navigation.PopAsync();
        }

        private void PlaySound(string soundName)
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Load(GetStreamFromFile(soundName));
            player.Play();
        }

        private void PlayCurrentSound() // pour que le son s'execute encore si le laser monte et descend
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            player.Stop();
            player.Play();
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var names = assembly.GetManifestResourceNames();
            Console.WriteLine("RESOURCES NAMES : " + String.Join(", ", names));

            var stream = assembly.GetManifestResourceStream("PPEMobile/" + filename);
            return stream;
        }

        private async Task Speak(string text)
        {
            var locales = await TextToSpeech.GetLocalesAsync();

            var locale = locales.Where(o => o.Language.ToLower() == "fr").FirstOrDefault();

            var settings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 0.1f,
                Locale = locale
            };

            await TextToSpeech.SpeakAsync(text, settings);
        }

        private async Task ResultsSpeech()
        {
            if (faceDetectResult == null)
            {
                await Speak("Humain non detecté");
            }
            else
            {
                await Speak("Un humain à été détecté");
                if(faceDetectResult.faceAttributes.gender.ToLower() == "male")
                {
                    await Speak("Son sexe masculin");
                }
                else
                {
                    await Speak("son Sexe est féminin");
                }

                await Speak("Et son âge est approximativement de" + faceDetectResult.faceAttributes.age.ToString() + " ans");
            }
        }

    }
}