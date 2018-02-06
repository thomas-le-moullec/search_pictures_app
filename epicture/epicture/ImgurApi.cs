using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace epicture
{

    class ImgurApi
    {
        static string client_id = "61e03335a5b2a97";
        static string client_secret = "dac6ae0e73d9758b1495113127fce6e8276c3352";
        ImageEndpoint endpoint;

        public ImgurApi()
        {
            var client = new ImgurClient(client_id, client_secret);
            endpoint = new ImageEndpoint(client);
            Debug.WriteLine("CONSTRUCTEUR IMGURAPI");
        }

        public async Task<ImageContainer> createImageContainerFromTag(string tag)
        {
            var image = await endpoint.GetImageAsync(tag);
            ImageContainer imageContainer = new ImageContainer();
            Image imgImgur = new Image();

            imgImgur.Source = new BitmapImage(new Uri(image.Link, UriKind.Absolute));
            imageContainer.AddImageSource(imgImgur);
            return imageContainer;
        }

    }
}
