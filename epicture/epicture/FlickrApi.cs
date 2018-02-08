using FlickrNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace epicture
{
    class FlickrApi
    {
        private const string API_KEY = "ed97463b02210fa18ccfe6a1358267b3";
        private const string API_SECRET = "d6826b8008652f89";
        private const string API_CALLBACK = "ms-appx-web://Microsoft.AAD.BrokerPlugIn/S-1-15-2-660250367-210907714-264770912-2147469295-3070322386-383610947-1518229695";
        private String SecretToken;
        private String AccessToken;
        private Flickr flickr;
        private ApiHelper.Helper Helper;

        public FlickrApi()
        {
            flickr = new Flickr(API_KEY);
            SecretToken = "";
            AccessToken = "";
            Helper = new ApiHelper.Helper(API_KEY, API_SECRET, API_CALLBACK);
        }

        public async Task<ImageContainer> createImageContainerFromTag(string tag, int nb_photos)
        {

            var options = new PhotoSearchOptions { Tags = tag, PerPage = nb_photos, Page = 1 };
            PhotoCollection photos = await flickr.PhotosSearchAsync(options);
            ImageContainer imageContainer = new ImageContainer();

            foreach (Photo photo in photos)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(photo.LargeUrl));
                img.Name = photo.PhotoId;
                imageContainer.AddImageSource(img);
            }

            return imageContainer;

        }

        private async Task<string> SendDataAsync(String Url)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                return await httpClient.GetStringAsync(new Uri(Url));
            }
            catch (Exception Err)
            {
                Debug.WriteLine(Err.Message);
            }

            return null;
        }

        /*public async void GetPhotos()
        {
        }*/

        public async void TestLogin()
        {
            String FlickrUrl = Helper.QueryTestLogin(AccessToken, SecretToken);
            string GetResponse = await SendDataAsync(FlickrUrl);
            Debug.WriteLine("Login Test Response :"+GetResponse);
        }

        private String ParseToken(String toFind, String response)
        {
            String token = null;
            String[] keyValPairs = response.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                if (splits[0] == toFind)
                {
                    token = splits[1];
                    return token;
                }
            }
            return "";
        }

        private async void ExchangeRequestTokenWithOauthToken(String TokenUri, String TokenSecretTmp)
        {
            //remove call back uri
            TokenUri = TokenUri.Remove(0, API_CALLBACK.Length + 1);
            Debug.WriteLine("Output Token After Auth:"+TokenUri);
            String oauth_token = ParseToken("oauth_token", TokenUri);
            String oauth_verifier = ParseToken("oauth_verifier", TokenUri);
            Debug.WriteLine("OauthToken:" + oauth_token + " and oauthVerifier:" + oauth_verifier);

            try
            {
                String FlickrUrl = Helper.QueryGetAccessToken(TokenSecretTmp, oauth_token, oauth_verifier);
                //Get the access token
                string GetResponse = await SendDataAsync(FlickrUrl);
                AccessToken = ParseToken("oauth_token", GetResponse);
                SecretToken = ParseToken("oauth_token_secret", GetResponse);

                Debug.WriteLine("Received Data: " + GetResponse);
                if (AccessToken == "" || SecretToken == "")
                {
                    Debug.WriteLine("Connection Failed ! :(");
                }
                Debug.WriteLine("Class Variables {AccessToken:" + AccessToken + " && SecretToken:" + SecretToken + "}");
            }
            catch (Exception Error)
            {
                Debug.WriteLine("Error : "+Error.Message);
            }
        }

        public static Flickr GetInstance()
        {
            return new Flickr(API_KEY, API_SECRET);
        }

        public async void Oauth()
        {
            try
            {
                //Get the request token.
                String FlickrUrl = Helper.QueryGetOauthUrl(SecretToken);
                string GetResponse = await SendDataAsync(FlickrUrl);

                Debug.WriteLine("Received Data: " + GetResponse);

                if (GetResponse != null)
                {
                    String oauth_token = ParseToken("oauth_token", GetResponse);
                    String oauth_token_secret = ParseToken("oauth_token_secret", GetResponse);

                    if (oauth_token != null)
                    {
                        //Get the user authorize
                        FlickrUrl = "https://secure.flickr.com/services/oauth/authorize?oauth_token=" + oauth_token + "&perms=read";
                        System.Uri StartUri = new Uri(FlickrUrl);
                        System.Uri EndUri = new Uri(API_CALLBACK);

                        Debug.WriteLine("Navigating to: " + FlickrUrl);
                        WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                                WebAuthenticationOptions.None,
                                                                StartUri,
                                                                EndUri);
                        //WebAuthenticationResult WebAuthenticationResult = Helper.AuthenticateUser(oauth_token).Result;

                        //WebAuthenticationResult.ResponseData.ToString() is the token request for User and the Token Verifier
                        if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                        {
                            ExchangeRequestTokenWithOauthToken(WebAuthenticationResult.ResponseData.ToString(), oauth_token_secret);
                        }
                        else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                        {
                            Debug.WriteLine("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                        }
                        else
                        {
                            Debug.WriteLine("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                        }
                    }
                }
            }
            catch (Exception Error)
            {
                Debug.WriteLine(Error.Message);
            }
        }

    }
}
