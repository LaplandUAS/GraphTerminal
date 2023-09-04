using System.Globalization;
using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;

namespace GraphTerminal.TerminalContent
{

    public class Packet
    {
        public string DeviceID { get; set; } = default!;
        public List<Sensor> Sensors { get; set; } = default!;
    }

    public class Sensor
    {
        public string SensorID { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    internal class Client
    {
        public event EventHandler<float[]>? MessageHandler;
        public event EventHandler? ClientHandler;
        public string TopicHandle;
        public int EntryCount;

        private readonly string _mqttURI;
        private readonly string _mqttUser;
        private readonly string _mqttPassword;
        private readonly string _mqttTopic;
        private Packet? _pkg;
        private float[]? _data;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _mqttOpitons;
        public Client(string uri, string usr, string pwd, string top)
        {
            TopicHandle = string.Empty;
            EntryCount = 0;
            _pkg = new Packet();
            _mqttURI = uri;
            _mqttUser = usr;
            _mqttPassword = pwd;
            _mqttTopic = top;
            _mqttFactory = new MqttFactory();
            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttOpitons = new MqttClientOptionsBuilder().WithTcpServer(_mqttURI, 1883).WithCredentials(_mqttUser, _mqttPassword);
        }
        public async void Connect()
        {
            await _mqttClient.ConnectAsync(_mqttOpitons.Build(), CancellationToken.None);
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_mqttTopic).Build());
            _mqttClient.ApplicationMessageReceivedAsync += (sender) => ClientReceived(sender.ApplicationMessage);
        }
        protected virtual Task ClientReceived(MqttApplicationMessage e)
        {
            try
            {
                _pkg = JsonSerializer.Deserialize<Packet>(Encoding.UTF8.GetString(e.PayloadSegment));
                _data = new float[_pkg!.Sensors.Count];
                for (int d = 0; d < _data.Length; d++)
                {
                    _data[d] = (float)Convert.ToDecimal(_pkg.Sensors[d].Value, CultureInfo.GetCultureInfo("en-US"));
                }
                TopicHandle = _pkg.DeviceID;
                EntryCount = _pkg.Sensors.Count;
                MessageHandler?.Invoke(this, _data);
            }
            catch
            {
                throw new Exception("InvalidPacket");
            }
            return Task.CompletedTask;
        }
        protected virtual void ClientDisconnected(object sender, EventArgs e)
        {
            ClientHandler?.Invoke(this, e);
        }
        public bool Connected
        {
            get => _mqttClient.IsConnected;
        }
    }
}
