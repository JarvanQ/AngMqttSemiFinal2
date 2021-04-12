using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using SignalRHubs;

namespace AngMqttSemiFinal2.Services.MqttService
{
    /// <summary>
    /// обьект для отправки на клиент.
    /// </summary>
    public class MqttMessage
    {
        public long MessageValue { get; set; }
        public string Topic { get; set; }
    }

    public class MqttClientService : IMqttClientService
    {
        private IMqttClient mqttClient;
        private IMqttClientOptions options;
        IHubContext<MqttHub> hubContext;

        private long startedStored = 0;
        private long startedReceived = 0;
        private long startedSent = 0;

        public MqttMessage mqttMessage ;

        public MqttClientService(IHubContext<MqttHub> hubContext)
        {
            this.hubContext = hubContext;
            CreateMqttClient();
            ConfigureMqttClient();
        }


        /// <summary>
        /// Создание подписчика
        /// </summary>
        /// <param name="options">настройки подписчика</param>
        /// <returns>обьект подписчика</returns>
        private void CreateMqttClient()
        {
            var brokerHostSettings = AppSettingsProvider.BrokerHostSettings;

            options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerHostSettings.Host, brokerHostSettings.Port).Build();

            mqttClient = new MqttFactory().CreateMqttClient();

        }

        /// <summary>
        /// Определение обработчиков событий
        /// </summary>
        /// <param name="mqttClient">Обьект подписчика</param>
        private void ConfigureMqttClient()
        {
            mqttClient.ConnectedHandler = this;
            mqttClient.DisconnectedHandler = this;
            mqttClient.ApplicationMessageReceivedHandler = this;
        }


        /// <summary>
        /// Обработчик получения рассылки
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            await hubContext.Clients.All.SendAsync("transferchartdata", GetMessageValue(eventArgs.ApplicationMessage));
        }
        /// <summary>
        /// Обработка полученого сообщения
        /// </summary>
        /// <param name="message">сообщение от брокера</param>
        /// <returns>Для наглядности будем отображать грифик значений изменений данных,
        /// а не сами данные(слишком большие числа)</returns>
        private MqttMessage GetMessageValue(MqttApplicationMessage message)
        {
            var topic = message.Topic.Split(new char[] { '/' }).Last();
            var payload = Encoding.UTF8.GetString(message.Payload);
            mqttMessage = new MqttMessage();
            mqttMessage.MessageValue = long.Parse(payload);
            mqttMessage.Topic = topic;

            switch (topic)
            {
                case "stored":
                    if (startedStored == 0)
                    {startedStored = mqttMessage.MessageValue;}
                    mqttMessage.MessageValue = mqttMessage.MessageValue - startedStored;
                    break;
                case "received":
                    if (startedReceived == 0)
                    {startedReceived = mqttMessage.MessageValue;}
                    mqttMessage.MessageValue -= startedReceived;
                    break;
                case "sent":
                    if (startedSent == 0)
                    {startedSent = mqttMessage.MessageValue;}
                    mqttMessage.MessageValue -= startedSent;
                    break;
            }
            return mqttMessage;

        }



        /// <summary>
        /// Обработчик подключения
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            await mqttClient.SubscribeAsync("$SYS/broker/messages/#");
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            await mqttClient.ConnectAsync(options);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await mqttClient.ConnectAsync(options);

            if (!mqttClient.IsConnected)
            {
                await mqttClient.ReconnectAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var disconnectOption = new MqttClientDisconnectOptions
                {
                    ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "NormalDiconnection"
                };
                await mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
            }
            await mqttClient.DisconnectAsync();
        }

    }

    /// <summary>
    /// Тестовые данные для более наглядного рисования грифика
    /// </summary>
    public static class DataManager
    {
        public static MqttMessage GetData()
        {
            var r = new Random();
            return new MqttMessage {MessageValue = r.Next(1, 40), Topic = "stored"};
        }
    }


}
