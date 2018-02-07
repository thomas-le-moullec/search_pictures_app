using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;
using Imgur.API.Models.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace epicture
{

    class ImgurApi
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

        async public void getToken()
        {
            var client_account = new ImgurClient(client_id, client_secret);
            var endpoint = new OAuth2Endpoint(client_account);
            imgur_token = await endpoint.GetTokenByCodeAsync(code);
        }

        async public void callBackURL()
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
        }

        public async void addFavorites(string id)
        {
            var values = new Dictionary<string, string>();
            var content = new FormUrlEncodedContent(values);

            client_http.DefaultRequestHeaders.Add("Authorization", "Bearer " + imgur_token.AccessToken);
            var response = await client_http.PostAsync("https://api.imgur.com/3/image/" + id + "/favorite", content);
            var responseString = await response.Content.ReadAsStringAsync();
        }


        public async void connection()
        {
            callBackURL();
        }

        public bool isConnected()
        {
            return imgur_token != null;
        }

        public async Task<ImageContainer> createImageContainerFromTag(string tag, int nb_photos)
        {
            ImageContainer imageContainer = new ImageContainer();
            GalleryEndpoint galleryEndpoint = new GalleryEndpoint(client);
            var galleries = await galleryEndpoint.SearchGalleryAsync(tag);

            ImageEndpoint imageEndpoint = new ImageEndpoint(client);

            for (int i = 0; i < nb_photos && i < galleries.Count(); i++)
            {
                Debug.WriteLine(galleries.ElementAt(i).GetType());
                if (galleries.ElementAt(i).GetType().ToString() != "Imgur.API.Models.Impl.GalleryImage")
                {
                    GalleryAlbum galleryAlbum = (GalleryAlbum)(galleries.ElementAt(i));

                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
                    if (galleryAlbum.Images.Count() > 0)
                    {
                        imgImgur.Source = new BitmapImage(new Uri(galleryAlbum.Images.ElementAt(0).Link, UriKind.Absolute));
                        Debug.WriteLine(galleryAlbum.Images.ElementAt(0).Link + " - - - " + galleryAlbum.Images.ElementAt(0).Id);
                        imgImgur.Name = galleryAlbum.Images.ElementAt(0).Id;
                        imageContainer.AddImageSource(imgImgur);
                    }
                }
                else
                    nb_photos++;
            }

            return imageContainer;
        }

        public async Task<ImageContainer> createImageContainerFromFavorites()
        {
            ImageContainer imageContainer = new ImageContainer();
            var client_fav = new ImgurClient(client_id, imgur_token);
            var endpoint = new AccountEndpoint(client_fav);
            var favourites = await endpoint.GetAccountFavoritesAsync();

            Debug.WriteLine(favourites.Count());

            for (int i = 0; i < favourites.Count(); i++)
            {
                Debug.WriteLine("I VAUT : " + i);

                if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryImage")
                {
                    Debug.WriteLine("Cast image");
                    GalleryImage galleryImage = (GalleryImage)(favourites.ElementAt(i));

                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();

                    imgImgur.Source = new BitmapImage(new Uri(galleryImage.Link, UriKind.Absolute));
                    Debug.WriteLine(galleryImage.Link + " - - - " + galleryImage.Id);
                    imgImgur.Name = galleryImage.Id;
                    imageContainer.AddImageSource(imgImgur);
                }
                else if (favourites.ElementAt(i).GetType().ToString() == "Imgur.API.Models.Impl.GalleryAlbum")
                {
                    Debug.WriteLine("Cast album");

                    GalleryAlbum galleryAlbum = (GalleryAlbum)(favourites.ElementAt(i));

                    Windows.UI.Xaml.Controls.Image imgImgur = new Windows.UI.Xaml.Controls.Image();
                    foreach (var image in galleryAlbum.Images)
                    {
                        imgImgur.Source = new BitmapImage(new Uri(image.Link, UriKind.Absolute));
                        Debug.WriteLine(image.Link + " - - - " + image.Id);
                        imgImgur.Name = image.Id;
                        imageContainer.AddImageSource(imgImgur);
                    }
                }
            }

            Debug.WriteLine("ON A FINI");

            return imageContainer;
        }

    }
}
