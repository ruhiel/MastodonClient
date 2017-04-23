using Mastonet.Entities;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MastodonClient.Models
{
    public class StatusModel : BindableBase
    {
        private Status _Status;

        private string _Content;

        private BitmapImage _AvatarIcon;

        public string Content
        {
            get { return _Content; }
            set { this.SetProperty(ref this._Content, value); }
        }

        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { this.SetProperty(ref this._UserName, value); }
        }

        public BitmapImage AvatarIcon
        {
            get { return _AvatarIcon; }
            set { this.SetProperty(ref this._AvatarIcon, value); }
        }

        public void Initialize()
        {
            Content = _Status.Content;

            UserName = _Status.Account.UserName;

            try
            {
                var byteArray = AsyncHelper.RunSync(() => DownLoadImageBytesAsync(_Status.Account.AvatarUrl));

                AvatarIcon = CreateBitmap(byteArray);
            }
            catch (Exception)
            {
            }
        }

        public StatusModel(Status status)
        {
            _Status = status;

            Initialize();
        }

        public static async Task<byte[]> DownLoadImageBytesAsync(string url)
        {
            using (var web = new HttpClient())
            {
                return await web.GetByteArrayAsync(url);
            }
        }

        public static BitmapImage CreateBitmap(byte[] bytes, bool freezing = true)
        {
            using (var stream = new WrappingStream(new MemoryStream(bytes)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                if (freezing && bitmap.CanFreeze)
                { bitmap.Freeze(); }

                return bitmap;
            }
        }

        ~StatusModel()
        {
            this.AvatarIcon = null;
        }
    }
}
