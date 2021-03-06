﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace epicture
{
    public class FlickrHelper
    {
        private String API_KEY;
        private String API_SECRET;
        private String API_CALLBACK;
        //private String SecretToken;
        //private String AccessToken;

        public FlickrHelper(String _apiKey, String _apiSecret, String _apiCallback)
        {
            API_KEY = _apiKey;
            API_SECRET = _apiSecret;
            API_CALLBACK = _apiCallback;
        }

        /**
        * Query to get the access token thanks to secretToken and token verifier
        */
        public String QueryGetAccessToken(String secretTokenTmp, String oauthToken, String oauthVerifier)
        {
            // Acquiring a request token
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            Random Rand = new Random();
            String FlickrUrl = "https://secure.flickr.com/services/oauth/access_token";
            Int32 Nonce = Rand.Next(1000000000);

            // Compute base signature string and sign it.
            // This is a common operation that is required for all requests even after the token is obtained.
            // Parameters need to be sorted in alphabetical order
            // Keys and values should be URL Encoded.
            String SigBaseStringParams = "oauth_consumer_key=" + API_KEY;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce.ToString();
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(SinceEpoch.TotalSeconds);
            SigBaseStringParams += "&" + "oauth_token=" + oauthToken;
            SigBaseStringParams += "&" + "oauth_verifier=" + oauthVerifier;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(FlickrUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = SignMeThat(SigBaseString, secretTokenTmp);

            FlickrUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            return FlickrUrl;
        }

        /**
        * Query to get the Request token and the token verifier.
        */
        public String QueryGetOauthUrl(String secretToken)
        {
            // Acquiring a request token
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            Random Rand = new Random();
            String FlickrUrl = "https://secure.flickr.com/services/oauth/request_token";
            Int32 Nonce = Rand.Next(1000000000);

            // Compute base signature string and sign it.
            // This is a common operation that is required for all requests even after the token is obtained.
            // Parameters need to be sorted in alphabetical order
            // Keys and values should be URL Encoded.
            String SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(API_CALLBACK);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + API_KEY;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce.ToString();
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(SinceEpoch.TotalSeconds);
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(FlickrUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = SignMeThat(SigBaseString, secretToken);

            FlickrUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            return FlickrUrl;
        }

        /**
        * Request to test the Authentification.
        */
        public String QueryTestLogin(String accessToken, String secretToken)
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            Random Rand = new Random();
            String FlickrUrl = "https://api.flickr.com/services/rest";
            Int32 Nonce = Rand.Next(1000000000);

            String SigBaseStringParams = "format=json";
            SigBaseStringParams += "&method=flickr.test.login";
            SigBaseStringParams += "&nojsoncallback=1";
            SigBaseStringParams += "&oauth_consumer_key=" + API_KEY;
            SigBaseStringParams += "&" + "oauth_nonce=" + Nonce.ToString();
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(SinceEpoch.TotalSeconds);
            SigBaseStringParams += "&" + "oauth_token=" + accessToken;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            String SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(FlickrUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

            String Signature = SignMeThat(SigBaseString, secretToken);

            FlickrUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            return FlickrUrl;
        }

        /**
        * Web Broker Windows to Authenticate the User with Oauth1 Technology
        */
        public async Task<WebAuthenticationResult> AuthenticateUser(String oauthToken)
        {
            //Get the user authorize
            String FlickrUrl = "https://secure.flickr.com/services/oauth/authorize?oauth_token=" + oauthToken + "&perms=read";
            System.Uri StartUri = new Uri(FlickrUrl);
            System.Uri EndUri = new Uri(API_CALLBACK);

            Debug.WriteLine("Navigating to: " + FlickrUrl);
            return await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    StartUri,
                                                    EndUri);
        }

        /**
        * Signature of Request to be authorized of doing Request
        * @param SigBaseString URL To Sign
        * @param tokenSecret Token Secret to Sign
        */
        public String SignMeThat(String SigBaseString, String tokenSecret)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(API_SECRET + "&" + tokenSecret, BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(SigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            String Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);
            return Signature;
        }
    }
}
