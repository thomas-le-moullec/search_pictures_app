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
using Windows.UI;
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
        private static readonly HttpClient client_http = new HttpClient();

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            FlickrApi = new FlickrApi();
            ImgurApi = new ImgurApi();
        }

        public void Flickr_ConnectionClick(object sender, RoutedEventArgs e)
        {
            FlickrApi.Oauth();
        }

        public void loginTestFlickrClick(object sender, RoutedEventArgs e)
        {
            FlickrApi.TestLogin();
        }

        //boutton pour rechercher sur les 2 sites en fonction d'un tag
        public async void SearchTagClick(object sender, RoutedEventArgs e)
        {
            string tag = "";
            int nb_photos = 1;

            if (TextBoxTag.Text != "")
                tag = TextBoxTag.Text;
            if (TextBoxNb.Text != "")
                nb_photos = Int32.Parse(TextBoxNb.Text);
            ImageContainer imgContainerFlickr = await FlickrApi.createImageContainerFromTag(tag, nb_photos);
            ImageContainer imgContainerImgur = await ImgurApi.createImageContainerFromTag(tag, nb_photos);
            ListViewTag_Imgur.ItemsSource = imgContainerImgur.GetImages();
            ListViewTag_Flickr.ItemsSource = imgContainerFlickr.GetImages();
        }

        //IMGUR - voir ses favoris
        public async void Imgur_GetFavoritesClick(object sender, RoutedEventArgs e)
        {
            if (ImgurApi.isConnected())
            {
                ImageContainer imgContainerImgur = await ImgurApi.createImageContainerFromFavorites();
                ListViewTag_Imgur.ItemsSource = imgContainerImgur.GetImages();
                setTextBoxWarning("");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");
        }

        //IMGUR - voir ses posts
        public async void Imgur_GetPostsClick(object sender, RoutedEventArgs e)
        {
            if (ImgurApi.isConnected())
            {
                ImageContainer imgContainerImgur = await ImgurApi.createImageContainerFromPosts();
                ListViewTag_Imgur.ItemsSource = imgContainerImgur.GetImages();
                setTextBoxWarning("");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");
        }

        //IMGUR - ajouter une photo aux favoris
        public void Imgur_AddFavorisClick(object sender, RoutedEventArgs e)
        {
            if (ImgurApi.isConnected())
            {
                for (int idx = 0; idx < ListViewTag_Imgur.SelectedItems.Count; idx++)
                {
                    Image img = (Image)ListViewTag_Imgur.SelectedItems.ElementAt(0);
                    ImgurApi.addFavorites(img.Name);
                }
                setTextBoxWarning("Photo ajoutée aux favoris !");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");

        }

        //IMGUR - supprimer l'un de ses posts
        public void Imgur_DeleteClick(object sender, RoutedEventArgs e)
        {
            if (ImgurApi.isConnected())
            {
                for (int idx = 0; idx < ListViewTag_Imgur.SelectedItems.Count; idx++)
                {
                    Image img = (Image)ListViewTag_Imgur.SelectedItems.ElementAt(0);
                    ImgurApi.deletePost(img.Name);
                }
                setTextBoxWarning("Photo supprimée !");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");

        }

        //IMGUR - poster une image
        public  void Imgur_PostClick(object sender, RoutedEventArgs e)
        {
            if (ImgurApi.isConnected())
            {
                ImgurApi.postImage(TextBoxPostFile.Text);
                setTextBoxWarning("Photo postée !");
            }
            else
                setTextBoxWarning("Vous n'êtes pas connecté à Imgur !");
        }


        //IMGUR - se connecter au service Imgur , modification des boutons
        public async void Imgur_ConnectionClick(object sender, RoutedEventArgs e)
        {
            setTextBoxWarning(await ImgurApi.connection());

            ButtonConnectImgur.Visibility = Visibility.Collapsed;

            TextBoxPostFile.Visibility = Visibility.Visible;
            ButtonImgurAddFavourites.Visibility = Visibility.Visible;
            ButtonImgurGetPost.Visibility = Visibility.Visible;
            ButtonImgurGetFavourites.Visibility = Visibility.Visible;
            ButtonImgurPost.Visibility = Visibility.Visible;
            ButtonImgurDeletePost.Visibility = Visibility.Visible;
        }

        //IMGUR - modification du message d'informations
        public void setTextBoxWarning(string text)
        {
            TextBlockWarning.Text = text;
        }
    }
}
