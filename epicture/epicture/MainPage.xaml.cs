using FlickrNet;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        ImgurApi imgurApi = new ImgurApi();
        FlickrApi flickrApi = new FlickrApi();

        private static readonly HttpClient client_http = new HttpClient();

        public MainPage()
        {
            this.InitializeComponent();
        }


        public async void SearchTagClick(object sender, RoutedEventArgs e)
        {
            string tag = "";
            int nb_photos = 1;

            if (TextBoxTag.Text != "")
                tag = TextBoxTag.Text;
            if (TextBoxNb.Text != "")
                nb_photos = Int32.Parse(TextBoxNb.Text);

            ImageContainer imgContainerFlickr = await flickrApi.createImageContainerFromTag(tag, nb_photos);
            ImageContainer imgContainerImgur = await imgurApi.createImageContainerFromTag(tag, nb_photos);
            ListViewTag1.ItemsSource = imgContainerImgur.GetImages();
            ListViewTag2.ItemsSource = imgContainerFlickr.GetImages();
        }


        public async void GetFavoritesClick(object sender, RoutedEventArgs e)
        {
            if (imgurApi.isConnected())
            {
                ImageContainer imgContainerImgur = await imgurApi.createImageContainerFromFavorites();
                ListViewTag1.ItemsSource = imgContainerImgur.GetImages();
            }
        }


        public async void AddFavorisClick(object sender, RoutedEventArgs e)
        {
            if (imgurApi.isConnected())
            {
                for (int idx = 0; idx < ListViewTag1.SelectedItems.Count; idx++)
                {
                    Image img = (Image)ListViewTag1.SelectedItems.ElementAt(0);
                    imgurApi.addFavorites(img.Name);
                }
                setTextBoxWarning("Photo ajoutée !");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");

        }

        public async void ImgurConnectionClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("On a clique sur le bouton");
            imgurApi.connection();
        }

        public void ListViewTag1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = ListViewTag1.SelectedItems.ToString();
            Debug.WriteLine("AAA : " + s);
        }

        public void setTextBoxWarning(string text)
        {
            TextBlockWarning.Text = text;
        }
    }
}
