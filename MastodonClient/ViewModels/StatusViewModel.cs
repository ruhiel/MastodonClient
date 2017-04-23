using MastodonClient.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MastodonClient.ViewModels
{
    public class StatusViewModel
    {
        public ReactiveProperty<BitmapImage> AvatarIcon { get; private set; }

        public ReactiveProperty<string> Content { get; private set; }

        public ReactiveProperty<string> UserName { get; private set; }

        private StatusModel _StatusModel;

        public StatusViewModel(StatusModel model)
        {
            _StatusModel = model;

            this.Content = _StatusModel
                .ObserveProperty(x => x.Content)
                .ToReactiveProperty();

            this.UserName = _StatusModel
                .ObserveProperty(x => x.UserName)
                .ToReactiveProperty();

            this.AvatarIcon = _StatusModel
                .ObserveProperty(x => x.AvatarIcon)
                .ToReactiveProperty();
        }
    }
}
