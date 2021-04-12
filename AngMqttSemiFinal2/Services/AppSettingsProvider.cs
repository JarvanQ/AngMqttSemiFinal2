using Microsoft.Extensions.Configuration;

namespace AngMqttSemiFinal2.Services
{
    public class AppSettingsProvider
    {
        private IConfiguration configuration;
        public static BrokerHostSettings BrokerHostSettings;
        public static ClientSettings ClientSettings;

        public AppSettingsProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void MapConfiguration()
        {
            MapBrokerHostSettings();
            MapClientSettings();
        }

        /// <summary>
        /// Считывание настроек брокера
        /// </summary>
        private void MapBrokerHostSettings()
        {
            BrokerHostSettings brokerHostSettings = new BrokerHostSettings();
            configuration.GetSection(nameof(BrokerHostSettings)).Bind(brokerHostSettings);
            AppSettingsProvider.BrokerHostSettings = brokerHostSettings;
        }
        /// <summary>
        /// Считывание настроек подписчика
        /// </summary>
        private void MapClientSettings()
        {
            ClientSettings clientSettings = new ClientSettings();
            configuration.GetSection(nameof(ClientSettings)).Bind(clientSettings);
            AppSettingsProvider.ClientSettings = clientSettings;
        }

    }
    /// <summary>
    /// Настройки брокера
    /// </summary>
    public class BrokerHostSettings
    {
        public string Host { set; get; }
        public int Port { set; get; }
    }
    /// <summary>
    /// Настройки подписчика
    /// </summary>
    public class ClientSettings
    {
        public string Id { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
    }
}
