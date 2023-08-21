using ConsoleGUI;
using ConsoleGUI.Space;
using GraphTerminal.TerminalContent.Extensions;

namespace GraphTerminal.TerminalContent
{
    //---------------------------------------------------------------
    //TerminalWindow Luokan prototyyppi
    //---------------------------------------------------------------
    internal sealed class TerminalWindow                                                                            //TerminalWindow luokka
    {
        //Yksityiset 
        private string TerminalTitle { get; set; }                                                                  //Ikkunan otsikko, merkkijono
        private int ElementCount { get; set; }                                                                      //Lukemien määrä
        private GraphControl Window { get; set; }                                                                   //Ikkunan puskuriobjekti
        private Offset WindowMargin { get; set; }                                                                   //Ikkunan marginaali
        private FloatRange TemperatureRange { get; set; }                                                           //Lämpötilojen minimi-maksimiarvot
        //-----------------------------------------------------------
        //TerminalWindow Luokan muodostin
        //Parametrit:
        //-string _title:
        //--Määrittää ikkunan otsikon, merkkijono
        //-bool _simple:
        //--Ajetaanko terminaalia yhteensopivuustilassa, totuusarvo
        //-----------------------------------------------------------
        public TerminalWindow(string _title, Offset _offset, int count)                                             //TerminalWindow Muodostin
        {
            TerminalTitle = _title;                                                                                 //Sisäistetään _title parametri otsikkojäseneen
            WindowMargin = _offset;                                                                                 //Sisäistetään _offset parametri ikkunan marginaaliin
            ElementCount = count;                                                                                   //Sisäistetään lukemien määrä

            Window = new GraphControl(WindowMargin, ElementCount)                                                   //Luodaan uusi ikkunaolio
            {
                Title = TerminalTitle                                                                               //Otsikolla
            };
        }
        //-----------------------------------------------------------
        //TerminalWindow Luokan [Update] funktio,
        //Päivittää ikkunan sisällön
        //Parametrit:
        //-float[] data,
        //--Päivitettävien lukemien arvotaulukko, likulukutaulukko
        //-----------------------------------------------------------
        public void Update(float[] data)                                                                            //TerminalWindow Update funktio
        {
            if (ConsoleManager.Content != Window) ConsoleManager.Content = Window;                                  //Varmistetaan että ikkuna kuuluu terminaalin sisältöön
            TemperatureRange = new FloatRange(data.Min(), data.Max());                                              //Luetaan ääriarvot lämpötilataulukosta
            Window.RefreshRange(TemperatureRange, 4);                                                               //Päivitetään ikkunan merkinnät raja-arvojen välille 4:n rivin tarkkuudella
            Window.BuildStack(TemperatureRange, data);                                                              //Päivitetään Käyrä lämpötilataulukon sisällöllä
            Window.Title = $"Topic:[{TerminalTitle}], TIME:[{DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss}]";         //Päivitetään otsikko & aikaleima
        }
        //---------------------------------------------------------------
        //Otsikko jäsenen kahva
        //---------------------------------------------------------------
        public string Title                                                                                         //Otsikon kahva
        {
            get => TerminalTitle;                                                                                   //Arvon palautuskahva
            set => TerminalTitle = value;                                                                           //Arvon asetuskahva
        }
    }
}
