﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace epicture
{
    class ImageContainer
    {

        List<Image> _images = new List<Image>();

        public ImageContainer()
        {
        }

        public void AddImageSource(Image img)
        {
            img.Width = 300;
            _images.Add(img);
        }

        public List<Image> GetImages()
        {
            return _images;
        }
    }
}
