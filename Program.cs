using ConsoleGUI;
using ConsoleGUI.Api;
using ConsoleGUI.Controls;
using ConsoleGUI.Space;
using GraphTerminal.TerminalContent;

namespace GraphTerminal
{
    //---------------------------------------------------------------
    //Program luokan staattinen prototyyppi:
    //Sisältää ohjelman entrypointin
    //---------------------------------------------------------------
    static class Program                                                                                            //Ohjelman Program luokka
    {
        //Vakiot
        private static readonly Offset TerminalWindowMargin = new(2, 2, 2, 2);                                      //Ohjelman ikkunan marginaali
        //Yksityiset
        private static TerminalWindow? _terminal;                                                                   //Ohjelman ikkuna
        private static Client? _client;                                                                             //MQTT yhteys
        private static VerticalStackPanel? _messagePanel;                                                           //Esitettävien käynnistysviestien puskuripaneeli
        private static bool _display = false;                                                                       //Onko palvelun aihe esitettävä
        private static string? _url, _usr, _pwd, _top;                                                              //Parametrien merkkijonot
        //-----------------------------------------------------------
        //Program luokan [Main] funktio:
        //Ohjelman entrypoint.
        //
        //Luodaan ohjelmalle oma ikkuna tyyppiä [TerminalWindow],
        //Aloitetaan ohjelman pääsilmukka.
        //
        //Parametrit:
        //-string[] args:
        //--Ohjelman käynnistysparametrit, merkkijonotaulukko
        //-----------------------------------------------------------
        static void Main(string[] args)                                                                             //Ohjelman entrypoint
        {
            for (int i = 0; i < args.Length; i++)                                                                   //Iteroidaan käyynistysparametrien läpi
            {
                switch (args[i])                                                                                    //Setvitään käynnistysparametrit                                                                          
                {
                    case "-url":                                                                                    //Palvelun osoite
                        _url = args[i + 1];
                        break;
                    case "-usr":                                                                                    //Käyttäjän nimi
                        _usr = args[i + 1];
                        break;
                    case "-pwd":                                                                                    //Käyttäjän salasana
                        _pwd = args[i + 1];
                        break;
                    case "-top":                                                                                    //Palvelun aihe
                        _top = args[i + 1];
                        break;
                    default:
                        break;
                }
            }
            _messagePanel = new VerticalStackPanel();                                                               //Luodaan käynnistysviestien puskuripaneeli
            Console.CursorVisible = false;                                                                          //Piilotetaan vilkkuva kursori
            _client = new Client(_url!, _usr!, _pwd!, _top!);                                                       //Luodaan uusi yhteys MQTT palvelimeen
            _client.MessageHandler += MQTTReceived!;                                                                //Kiinnitetään vastaanotettu viesti uuteen tapahtumaan
            _client.ClientHandler += MQTTDisconnected!;                                                             //Kiinnitetään yhteyden menetys uuteen tapahtumaan
            ConsoleManager.Console = new SimplifiedConsole();                                                       //Luodaan yksinkertaistettu ympäristö mikäli yhteensopivuustilassa(FIXME)!!!
            ConsoleManager.Setup();                                                                                 //Käynnistetään ympäristö
            ConsoleManager.Content = _messagePanel;                                                                 //Kiinnitetään viestit terminaaliin
            _client.Connect();                                                                                      //Yhdistetään palveluun
            _messagePanel.Add(new TextBlock { Text = "[WAIT] Connecting to broker..." });                           //Ilmoitetaan että palveluun yhdistäminen on alkanut
            while (!_client.Connected) {}                                                                           //Jäädään kiertämään kunnes palveluun on muodotettu yhteys
            _messagePanel.Add(new TextBlock { Text = "[OK] Connected!" });                                          //Ilmoitetaan yhteyden muodostuneen
            _messagePanel.Add(new TextBlock { Text = "[WAIT] Waiting for topic response..." });                     //Ilmoitetaan että palvelun aihetta odotetaan
            _messagePanel.Add(new TextBlock { Text = "[WAIT] (This may take several minutes)" });                   //Ilmoitetaan että aikaa voi kulua
            while (!_display) {}                                                                                    //Jäädään kiertämään kunnes palvelun aihe on esitettävissä
            MainLoop(true);                                                                                         //Aloitetaan ohjelman pääsilmukka
        }
        //-----------------------------------------------------------
        //Program luokan [MQTTDIsconnected] funktio:
        //Yhteyden katkeamisen tapahtuma.
        //
        //
        //Parametrit:
        //-object sender:
        //--Viite alkuperäobjektiin, olio,
        //-EventArgs e:
        //--Tapahtuman parametrit (ei käytetty), Tapahtumaparametri
        //-----------------------------------------------------------
        private static void MQTTDisconnected(object sender, EventArgs e)
        {
            _messagePanel?.Add(new TextBlock { Text = "[ERROR] Failed to connect!" });                              //Ilmoitetaan yhteyden muodostamisen epäonnistuneen
            _messagePanel?.Add(new TextBlock { Text = "[ERROR] Press any key to continue..." });                    //Pyydetään käyttäjää painamaan näppäintä
            Console.ReadKey();                                                                                      //Ohjelma roikkuu tässä kunnes käyttäjä painaa näppäintä
            Environment.Exit(0);                                                                                    //Poistutaan ohjelmaympäristöstä, suljetaan ohjelma
        }
        //-----------------------------------------------------------
        //Program luokan [MQTTReceived] funktio:
        //Datan vastaanotto tapahtuma.
        //
        //Ajetaan [_client]-olion MessageHandler-tapahtuman lauetessa
        //
        //Parametrit:
        //-object sender:
        //--Viite alkuperäobjektiin, olio
        //-float[] data:
        //--lämpötila-arvot, liukulukutaulukko
        //-----------------------------------------------------------
        private static void MQTTReceived(object sender, float[] data)                                               //MQTT viestin vastaanottofunktio
        {
            if (!_display)                                                                                          //Onko palvelun aihe esitettävä?
            {
                Console.Clear();                                                                                    //Tyhjennetään terminaali
                _terminal ??= new TerminalWindow(_client!.TopicHandle, TerminalWindowMargin, data.Length + 2);      //Luodaan uusi ikkuna terminaaliin otsikolla esimääritettyyn marginaaliin sopivaksi, sekä legacy-tuella
                _display = true;                                                                                    //Asetetaan palvelun aihe esitettäväksi
            }
            ConsoleManager.AdjustBufferSize();                                                                      //Päivitetään terminaalin ikkunaa
            _terminal!.Update(data);                                                                                //Päivitetään ikkunan sisältö
        }
        //-----------------------------------------------------------
        //Program luokan [MainLoop] funktio:
        //Ohjelman pääsilmukka.
        //
        //Ajetaan [while]-silmukkaa parametrin _run ollessa tosi
        //
        //Parametrit:
        //-bool _run:
        //--[while]-silmukan käyntiehto, totuusarvo
        //-----------------------------------------------------------
        static void MainLoop(bool _run)                                                                             //Ohjelman pääsilmukka
        {
            while (_run)                                                                                            //Toistetaan ohjelman _run totuusarvon ollessa tosi
            {
                ConsoleManager.AdjustBufferSize();                                                                  //Päivitetään terminaalin ikkunaa
                Thread.Sleep(5000);                                                                                 //Prosessin ei tarvitse pyöriä jatkuvasti
            }
        }
    }
}