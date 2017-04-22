using MastodonClient.Models;
using MastodonClient.Views;
using Mastonet.Entities;
using Reactive.Bindings;
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

        public ObservableCollection<Status> PublicStatusList => Mastodon.Instance.PublicStatusList;

        public ObservableCollection<Status> UserStatusList => Mastodon.Instance.UserStatusList;

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
        }
    }
}
