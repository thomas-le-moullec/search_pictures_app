using FlickrNet;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace epicture
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static MainPage Current;
        ImgurApi ImgurApi;
        FlickrApi FlickrApi;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            //string URI = string.Format("ms-appx-web://Microsoft.AAD.BrokerPlugIn/{0}", WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host.ToUpper());
            FlickrApi = new FlickrApi();
            ImgurApi = new ImgurApi();
        }

        public async void OauthFlickrClick(object sender, RoutedEventArgs e)
        {
            FlickrApi.Oauth();
        }

        public async void loginTestFlickrClick(object sender, RoutedEventArgs e)
        {
            FlickrApi.TestLogin();
        }

        public async void SearchTagClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DEBUG : " + TextBoxTag.Text);

            string tag = "";
            int nb_pages = 1;

            if (TextBoxTag.Text != "")
                tag = TextBoxTag.Text;
            if (TextBoxNb.Text != "")
                nb_pages = Int32.Parse(TextBoxNb.Text);


            ImageContainer imgContainerFlickr = await FlickrApi.createImageContainerFromTag(tag, nb_pages);
            ImageContainer imgContainerImgur = await ImgurApi.createImageContainerFromTag(tag);

            ListViewTag1.ItemsSource = imgContainerImgur.GetImages();
            ListViewTag2.ItemsSource = imgContainerFlickr.GetImages();
        }
    }
}
