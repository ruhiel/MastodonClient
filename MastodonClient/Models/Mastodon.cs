using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonClient.Models
{
    public class Mastodon
    {
        public AuthenticationClient AuthenticationClient { get; private set; }

        public Mastonet.MastodonClient MastodonClient { get; private set; }

        private AppRegistration _AppRegistration;

        public static Mastodon Instance { get; } = new Mastodon();

        public ObservableCollection<Status> PublicStatusList { get; private set; } = new ObservableCollection<Status>();

        public ObservableCollection<Status> UserStatusList { get; private set; } = new ObservableCollection<Status>();

        private TimelineStreaming _PublicStream;

        private TimelineStreaming _UserStream;

        private Mastodon()
        {
        }

        public async Task Initialize()
        {
            if(String.IsNullOrEmpty(Properties.Settings.Default.InstanceUrl))
            {
                return;
            }

            if(String.IsNullOrEmpty(Properties.Settings.Default.Auth_ClientId))
            {
                _AppRegistration = await AppRegistrationSetting();
                Properties.Settings.Default.Auth_Id = _AppRegistration.Id;
                Properties.Settings.Default.Auth_ClientId = _AppRegistration.ClientId;
                Properties.Settings.Default.Auth_ClientSecret = _AppRegistration.ClientSecret;
                Properties.Settings.Default.Auth_RedirectUri = _AppRegistration.RedirectUri;
                Properties.Settings.Default.InstanceUrl = _AppRegistration.Instance;
                Properties.Settings.Default.Save();
            }
            else
            {
                _AppRegistration = new AppRegistration();
                _AppRegistration.Id = Properties.Settings.Default.Auth_Id;
                _AppRegistration.ClientId = Properties.Settings.Default.Auth_ClientId;
                _AppRegistration.ClientSecret = Properties.Settings.Default.Auth_ClientSecret;
                _AppRegistration.RedirectUri = Properties.Settings.Default.Auth_RedirectUri;
                _AppRegistration.Instance = Properties.Settings.Default.InstanceUrl;
                _AppRegistration.Scope = Scope.Read | Scope.Write | Scope.Follow;
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.Token_AccessToken))
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.AuthCode))
                {
                    var auth = await AuthSetting(Properties.Settings.Default.AuthCode);
                    Properties.Settings.Default.Token_AccessToken = auth.AccessToken;
                    Properties.Settings.Default.Token_TokenType = auth.TokenType;
                    Properties.Settings.Default.Token_Scope = auth.Scope;
                    Properties.Settings.Default.Token_CreatedAt = auth.CreatedAt;
                    Properties.Settings.Default.Save();

                    MastodonClient = new Mastonet.MastodonClient(_AppRegistration, auth);
                }
            }
            else
            {
                var auth = new Auth();
                auth.AccessToken = Properties.Settings.Default.Token_AccessToken;
                auth.TokenType = Properties.Settings.Default.Token_TokenType;
                auth.Scope = Properties.Settings.Default.Token_Scope;
                auth.CreatedAt = Properties.Settings.Default.Token_CreatedAt;

                MastodonClient = new Mastonet.MastodonClient(_AppRegistration, auth);
            }
        }

        public async void PostStatus(string str, Visibility visibility)
        {
            await MastodonClient.PostStatus(str, visibility);
        }

        private Task<AppRegistration> AppRegistrationSetting()
        {
            AuthenticationClient = new AuthenticationClient(Properties.Settings.Default.InstanceUrl);
            return AuthenticationClient.CreateApp("MastodonClient", Scope.Read | Scope.Write | Scope.Follow);
        }

        private Task<Auth> AuthSetting(string authCode)
        {
            return AuthenticationClient.ConnectWithCode(authCode);
        }

        public void Start()
        {
            if(MastodonClient == null)
            {
                return;
            }

            _PublicStream = MastodonClient.GetPublicStreaming();

            _PublicStream.Start();

            _PublicStream.OnUpdate += (_, e) =>
            {
                PublicStatusList.Insert(0, e.Status);
            };

            _UserStream = MastodonClient.GetUserStreaming();

            _UserStream.Start();

            _UserStream.OnUpdate += (_, e) =>
            {
                UserStatusList.Insert(0, e.Status);
            };
        }
        public void Stop()
        {
            _PublicStream?.Stop();

            _UserStream?.Stop();
        }
    }
}
