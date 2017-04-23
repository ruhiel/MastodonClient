using MastodonClient.Models;
using MastodonClient.Views;
using Mastonet.Entities;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MastodonClient.ViewModels
{
    public class MainWindowViewModel
    {
        public ReactiveCommand<RoutedEventArgs> LoadCommand { get; private set; }

        public ReactiveCommand AccountCommand { get; private set; }

        public ReactiveCommand PostStatusCommand { get; private set; }

        public ReactiveProperty<Mastonet.Visibility> SelectedVisibility { get; private set; }

        public LimitObservableCollection<StatusViewModel> PublicStatusList { get; private set; }

        public LimitObservableCollection<StatusViewModel> UserStatusList { get; private set; }

        public MainWindowViewModel()
        {
            LoadCommand = new ReactiveCommand<RoutedEventArgs>();

            LoadCommand.Subscribe(async _ => 
            {
                await Mastodon.Instance.Initialize();

                Mastodon.Instance.Start();
            });

            PostStatusCommand = new ReactiveCommand();

            PostStatusCommand.Subscribe(obj =>
            {
                Mastodon.Instance.PostStatus((string)obj, SelectedVisibility.Value);
            });

            SelectedVisibility = new ReactiveProperty<Mastonet.Visibility>(Mastonet.Visibility.Public);

            AccountCommand = new ReactiveCommand();

            AccountCommand.Subscribe(async _ =>
            {
                var window = new LoginInfoWindow();
                window.ShowDialog();

                await Mastodon.Instance.Initialize();

                Mastodon.Instance.Start();
            });

            PublicStatusList = new LimitObservableCollection<StatusViewModel>(100);

            UserStatusList = new LimitObservableCollection<StatusViewModel>(100);

            Mastodon.Instance.PublicStatusList.CollectionChangedAsObservable().Subscribe(x =>
            {
                if(x.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in x.NewItems)
                    {
                        var model = new StatusModel((Status)item);

                        PublicStatusList.Add(new StatusViewModel(model));
                    }
                }
            });

            Mastodon.Instance.UserStatusList.CollectionChangedAsObservable().Subscribe(x =>
            {
                if (x.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in x.NewItems)
                    {
                        var model = new StatusModel((Status)item);

                        PublicStatusList.Add(new StatusViewModel(model));
                    }
                }
            });
        }
    }
}
