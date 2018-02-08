
using System;
using System.Threading.Tasks;
using epicture;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace epictureTest
{
    [TestClass]
    public class ImgurApiTest
    {
        [TestMethod]
        public async Task getImageCountTest()
        {
            ImgurApi imgur_api = new ImgurApi();
            int count = await imgur_api.GetImageCount("hello", 10);

            Assert.AreEqual(10, count);
        }

        [TestMethod]
        public async Task getImagesFavoritesCountTest()
        {
            ImgurApi imgur_api = new ImgurApi();
            await imgur_api.setImgurToken("eb084daf1fbbd4c6087c07644f7df017ef930ed1");
            int count = await imgur_api.GetFavoritesCount();

            Assert.AreNotEqual(0, count);  
        }

        [TestMethod]
        public async Task getImagesPostCountTest()
        {
            ImgurApi imgur_api = new ImgurApi();
            await imgur_api.setImgurToken("eb084daf1fbbd4c6087c07644f7df017ef930ed1");
            int count = await imgur_api.GetPostCount();

            Assert.AreNotEqual(0, count);
        }

        [TestMethod]
        public async Task PostTest()
        {
            ImgurApi imgur_api = new ImgurApi();
            await imgur_api.setImgurToken("eb084daf1fbbd4c6087c07644f7df017ef930ed1");
            int count = await imgur_api.GetPostCount();
            //await imgur_api.postImage("Capture.png");
            int count_final = await imgur_api.GetPostCount();
            Assert.AreEqual(count, count_final);
        }
    }
}
