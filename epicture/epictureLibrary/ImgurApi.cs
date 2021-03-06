﻿using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;
using Imgur.API.Models.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace epicture
{
    /**
     * ImgurAPI will allow us to interact with Imgur and get pictures and so on...
     */
    public class ImgurApi
    {
        ImageEndpoint endpoint;
        string client_id = "48a61c1ae13d63e";
        string client_secret = "b13698625f471abb649bafe54182f9072017fc45";
        ImgurClient client;
        Imgur.API.Models.IOAuth2Token imgur_token = null;
        string code;
        System.Uri startURI;

        private static readonly HttpClient client_http = new HttpClient();

        public ImgurApi()
        {
            string startURL = "https://api.imgur.com/oauth2/authorize?client_id=" + client_id + "&response_type=code&state=";
            startURI = new System.Uri(startURL);
            client = new ImgurClient(client_id, client_secret);
            endpoint = new ImageEndpoint(client);
        }

        public string getImgurToken()
        {
            return code;
        }

        public async Task setImgurToken(string _code)
        {
            var client_account = new ImgurClient(client_id, client_secret);
            var endpoint = new OAuth2Endpoint(client_account);
            imgur_token = await endpoint.GetTokenByCodeAsync(_code);
        }


        async public void getToken()
        {
            var client_account = new ImgurClient(client_id, client_secret);
            var endpoint = new OAuth2Endpoint(client_account);
            imgur_token = await endpoint.GetTokenByCodeAsync(code);
        }

        /**
        * This method will redirect the user to the imgur login page thanks to the startURI member. This method will also get the response and parse the code in order to get the access token
        */
        async public Task<String> callBackURL()
        {
            string result;

            try
            {
                var webAuthenticationResult =
                    await Windows.Security.Authentication.Web.WebAuthenticationBroker.AuthenticateAsync(
                    Windows.Security.Authentication.Web.WebAuthenticationOptions.None,
                    startURI);

                switch (webAuthenticationResult.ResponseStatus)
                {
                    case Windows.Security.Authentication.Web.WebAuthenticationStatus.Success:
                        result = webAuthenticationResult.ResponseData.ToString();
                        code = result.Split('=').ElementAt(2);
                        getToken();
                        break;
                    case Windows.Security.Authentication.Web.WebAuthenticationStatus.ErrorHttp:
                        result = webAuthenticationResult.ResponseErrorDetail.ToString();
                        break;
                    default:
                        result = webAuthenticationResult.ResponseData.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return "Vous êtes connecté à Imgur !";
        }

        //ajouter une photo à ses favoris
        public async void addFavorites(string id)
        {
            var values = new Dictionary<string, string>();
            var content = new FormUrlEncodedContent(values);

            client_http.DefaultRequestHeaders.Add("Authorization", "Bearer " + imgur_token.AccessToken);
            var response = await client_http.PostAsync("https://api.imgur.com/3/image/" + id + "/favorite", content);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        //connexion à Imgur
        public async Task<String> connection()
        {
            return await callBackURL();
        }

        // vérifie si l'utilisateur est connecté
        public bool isConnected()
        {
            return imgur_token != null;
        }

        //renvoie le nombre d'image associées au tag envoyé
        public async Task<int> GetImageCount(string tag, int nb_photos)
        {
            int count = 0;
            GalleryEndpoint galleryEndpoint = new GalleryEndpoint(client);
            var galleries = await galleryEndpoint.SearchGalleryAsync(tag);

            for (int i = 0; i < nb_photos && i < galleries.Count(); i++)
            {
                if (galleries.ElementAt(i).GetType().ToString() != "Imgur.API.Models.Impl.GalleryImage")
                {
                    GalleryAlbum galleryAlbum = (GalleryAlbum)(galleries.ElementAt(i));
                    if (galleryAlbum.Images.Count() > 0)
                        count++;
                }
                else
                    nb_photos++;
            }
            return count;
        }

        //renvoie les images associées au tag envoyé
        /**
        * This method extract images with a certain tag
        * @param tag the tag's name
        * @param nb_photos the number of pictures.
        */
        public async Task<ImageContainer> createImageContainerFromTag(string tag, int nb_photos)
        {
            ImageContainer imageContainer = new ImageContainer();
            GalleryEndpoint galleryEndpoint = new GalleryEndpoint(client);
            var galleries = await galleryEndpoint.SearchGalleryAsync(tag);

            for (int i = 0; i < nb_photos && i < galleries.Count(); i++)
            {
                if (galleries.ElementAt(i).GetType().ToString() != "Imgur.API.Models.Impl.GalleryImage")
                {
                    GalleryAlbum galleryAlbum = (GalleryAlbum)(galleries.ElementAt(i));

                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
                    if (galleryAlbum.Images.Count() > 0)
                    {
                        imgImgur.Source = new BitmapImage(new Uri(galleryAlbum.Images.ElementAt(0).Link, UriKind.Absolute));
                        imgImgur.Name = galleryAlbum.Images.ElementAt(0).Id;
                        imageContainer.AddImageSource(imgImgur);
                    }
                }
                else
                    nb_photos++;
            }
            return imageContainer;
        }

        //renvoie le nombre de favoris de l'utilsateur
        public async Task<int> GetFavoritesCount()
        {
            int count = 0;
            var client_fav = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client_fav);
            var favourites = await endpoint.GetAccountFavoritesAsync();

            for (int i = 0; i < favourites.Count(); i++)
            {
                if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryImage")
                    count++;
                else if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryAlbum")
                {
                    GalleryAlbum galleryAlbum = (GalleryAlbum)(favourites.ElementAt(i));
                    foreach (var image in galleryAlbum.Images)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        //renvoie les images des favoris de l'utilsateur
        /**
         * Return favorite pictures of the user
         */
        public async Task<ImageContainer> createImageContainerFromFavorites()
        {
            ImageContainer imageContainer = new ImageContainer();
            var client_fav = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client_fav);
            var favourites = await endpoint.GetAccountFavoritesAsync();

            for (int i = 0; i < favourites.Count(); i++)
            {
                if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryImage")
                {
                    GalleryImage galleryImage = (GalleryImage)(favourites.ElementAt(i));
                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
                    imgImgur.Source = new BitmapImage(new Uri(galleryImage.Link, UriKind.Absolute));
                    imgImgur.Name = galleryImage.Id;
                    imageContainer.AddImageSource(imgImgur);
                }
                else if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryAlbum")
                {
                    GalleryAlbum galleryAlbum = (GalleryAlbum)(favourites.ElementAt(i));
                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
                    foreach (var image in galleryAlbum.Images)
                    {
                        imgImgur.Source = new BitmapImage(new Uri(image.Link, UriKind.Absolute));
                        imgImgur.Name = image.Id;
                        imageContainer.AddImageSource(imgImgur);
                    }
                }
            }

            return imageContainer;
        }

        //renvoie le nombre de post par l'utilisateur
        public async Task<int> GetPostCount()
        {
            int count = 0;
            var client_fav = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client_fav);
            var favourites = await endpoint.GetImagesAsync();

            for (int i = 0; i < favourites.Count(); i++)
            {
                count++;
            }
            return count;
        }

        //renvoie les images postées par l'utilisateur
        /**
         * Return pictures posted from the user
         */
        public async Task<ImageContainer> createImageContainerFromPosts()
        {
            ImageContainer imageContainer = new ImageContainer();
            var client_fav = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client_fav);
            var favourites = await endpoint.GetImagesAsync();

            for (int i = 0; i < favourites.Count(); i++)
            {
               Imgur.API.Models.Impl.Image galleryImage = (Imgur.API.Models.Impl.Image)(favourites.ElementAt(i));
               Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
               imgImgur.Source = new BitmapImage(new Uri(galleryImage.Link, UriKind.Absolute));
               imgImgur.Name = galleryImage.Id;
               imageContainer.AddImageSource(imgImgur);
            }
            return imageContainer;
        }

        //poste une image depuis la bibliothèque d'images
        /**
         * Post a picture
         */
        public async void postImage(string filename)
        {
            var client_post = new ImgurClient(client_id, imgur_token);
            var endpoint_post = new ImageEndpoint(client_post);
            IImage image = null;
            await Task.Run(() =>
            {
                Task.Yield();
                using (var fs = new FileStream(@"C:\Users\Léo\Pictures\" + filename, FileMode.Open))
                {
                    image = endpoint_post.UploadImageStreamAsync(fs).Result;
                }
            });
            Windows.UI.Xaml.Controls.Image imgImgur2 = new Windows.UI.Xaml.Controls.Image();
            imgImgur2.Source = new BitmapImage(new Uri(image.Link, UriKind.Absolute));
            imgImgur2.Name = image.Id;
        }

        public async void deletePost(string id)
        {
            var client = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client);
            var deleted = await endpoint.DeleteImageAsync(id, "me");
        }

    }
}
