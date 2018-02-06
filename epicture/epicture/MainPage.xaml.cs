using FlickrNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        const string API_KEY = "ed97463b02210fa18ccfe6a1358267b3";

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void SearchTagClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DEBUG : " + TextBoxTag.Text);

            string tag = TextBoxTag.Text;
            int nb_pages = Int32.Parse(TextBoxNb.Text);

            Flickr flickr = new Flickr(API_KEY);
            var options = new PhotoSearchOptions { Tags = tag, PerPage = nb_pages, Page = 1 };
            PhotoCollection photos = await flickr.PhotosSearchAsync(options);

            ImageContainer imageContainer = new ImageContainer();
            ImageContainer imageContainer2 = new ImageContainer();

            foreach (Photo photo in photos)
            {
                Debug.WriteLine("Photo {0} has title {1}", photo.PhotoId, photo.Title);
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(photo.LargeUrl));
                Image img_2 = new Image();
                img_2.Source = new BitmapImage(new Uri(photo.LargeUrl));
                imageContainer.AddImageSource(img);
                imageContainer2.AddImageSource(img_2);
            }

            ListViewTag1.ItemsSource = imageContainer.GetImages();
            ListViewTag2.ItemsSource = imageContainer2.GetImages();

            //ListViewTag1.SelectionChanged = Listview1_SelectionChanged;

        }
    }
}
