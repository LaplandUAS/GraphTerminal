using System.Globalization;
using System.Text;
using System.Text.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

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

        private readonly string _clientID = Guid.NewGuid().ToString();
        private readonly string _mqttURI;
        private readonly string _mqttUser;
        private readonly string _mqttPassword;
        private readonly string[] _mqttTopic;
        private Packet _pkg;
        private float[]? _data;
        private readonly MqttClient _mqttClient;
        public Client(string uri, string usr, string pwd, string top)
        {
            TopicHandle = string.Empty;
            EntryCount = 0;
            _pkg = new Packet();
            _mqttURI = uri;
            _mqttUser = usr;
            _mqttPassword = pwd;
            _mqttTopic = new string[] { top };
            _mqttClient = new MqttClient(_mqttURI);
            _mqttClient.MqttMsgPublishReceived += ClientReceived;
        }
        public void Connect()
        {
            try
            {
                _mqttClient.Connect(_clientID, _mqttUser, _mqttPassword);
            }
            catch
            {
                ClientDisconnected(this, EventArgs.Empty);
            }
            _mqttClient.Subscribe(_mqttTopic, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        protected virtual void ClientReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                _pkg = JsonSerializer.Deserialize<Packet>(Encoding.UTF8.GetString(e.Message))!;
                _data = new float[_pkg.Sensors.Count];
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
