using MastodonClient.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MastodonClient.ViewModels
{
    public class LoginInfoWindowViewModel
    {
        public ReactiveCommand CloseCommand { get; private set; }

        public ReactiveCommand AuthCommand { get; private set; }

        public LoginInfoWindowViewModel()
        {
            AuthCommand = new ReactiveCommand();

            AuthCommand.Subscribe(async obj =>
            {
                await Mastodon.Instance.Initialize();

                System.Diagnostics.Process.Start(Mastodon.Instance.AuthenticationClient.OAuthUrl());
            });

            CloseCommand = new ReactiveCommand();

            CloseCommand.Subscribe(async obj =>
            {
                await Mastodon.Instance.Initialize();

                Properties.Settings.Default.Save();
                var window = obj as Window;
                window?.Close();
            });
        }
    }
}
