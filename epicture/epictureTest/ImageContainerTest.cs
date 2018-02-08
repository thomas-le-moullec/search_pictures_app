using System;
using System.Collections.Generic;
using epicture;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml.Controls;

namespace epictureTest
{
    [TestClass]
    public class ImageContainerTest
    {
        [TestMethod]
        public void emptyListTest()
        {
            ImageContainer image_container = new ImageContainer();
            List<Image> list_img = image_container.GetImages();

            Assert.AreEqual(0, list_img.Count);
        }

        [UITestMethod]
        public void fiveItemsListTest()
        {
            ImageContainer image_container = new ImageContainer();
            Image img = new Image();
            List<Image> list_img = null;

            for (int i = 0; i < 5; i++)
                image_container.AddImageSource(img);
            list_img = image_container.GetImages();
            Assert.AreEqual(5, list_img.Count);
        }



    }
}
