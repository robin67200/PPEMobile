using PPEMobile.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PPEMobile.Services
{
    public static class CognitiveService
    {
        // Clef d'API
        // URL (endPoint)
        private static readonly string API_KEY = "8cd336c616c04831a634b75a1508a503";
        private static readonly string ENDPOINT_URL = "https://ppemobile.cognitiveservices.azure.com/face/v1.0/";
        public static async Task<FaceDetectResult> FaceDetect(Stream imageStream)
        {
            if (imageStream == null)
            {
                return null;
            }

            var url = ENDPOINT_URL + "detect" + "?returnFaceAttributes=age,gender";

            // Headers où l'on passe les données de la clef d'api
            // Body où l'on passe les données de l'image

            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
                    webClient.Headers.Add("Ocp-Apim-Subscription-Key", API_KEY);

                    var data = ReadStream(imageStream); // conversion du imageStream en byte[]
                    //var result = webClient.UploadData(url, data);

                    var result = await Task.Run(() => webClient.UploadData(url, data) );
                    // sur la ligne ci-dessus on va attendre que la tache s'execute grace au await pour ne pas avoir une méthode bloquante

                    if (result == null)
                    {
                        return null;
                    }

                    string json = Encoding.UTF8.GetString(result, 0, result.Length); // convertit notre result en string

                    var faceResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FaceDetectResult[]>(json); // deserialize les json de notre model

                    if (faceResult.Length >= 1) // si plusieurs visages sur la photo -> renvoi uniquement le premier detecté
                    {
                        return faceResult[0];
                    }

                    Console.WriteLine("Réponse Ok" + json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception webclient : " + ex.Message);
                }

                return null;
                
            }
        }

        private static byte[] ReadStream(Stream input) // pour convertir notre stream du scannerPage en byte pour le FaceDetect
        {
            BinaryReader b = new BinaryReader(input);
            byte[] data = b.ReadBytes((int)input.Length);
            return data;
        }
    }
}
