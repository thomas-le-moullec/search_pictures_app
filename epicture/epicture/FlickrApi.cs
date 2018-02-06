using FlickrNet;
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
    class FlickrApi
    {
        const string API_KEY = "ed97463b02210fa18ccfe6a1358267b3";
        Flickr flickr;

        public FlickrApi()
        {
            flickr = new Flickr(API_KEY);
        }

        public async Task<ImageContainer> createImageContainerFromTag(string tag, int nb_pages)
        {

            var options = new PhotoSearchOptions { Tags = tag, PerPage = nb_pages, Page = 1 };
            PhotoCollection photos = await flickr.PhotosSearchAsync(options);
            ImageContainer imageContainer = new ImageContainer();

            foreach (Photo photo in photos)
            {
                //Debug.WriteLine("Photo {0} has title {1}", photo.PhotoId, photo.Title);
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(photo.LargeUrl));
                imageContainer.AddImageSource(img);
            }

            return imageContainer;

        }

    }
}
