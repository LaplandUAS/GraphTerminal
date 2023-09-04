using ConsoleGUI;
using ConsoleGUI.Controls;
using ConsoleGUI.Data;
using ConsoleGUI.Space;
using ConsoleGUI.UserDefined;

namespace GraphTerminal.TerminalContent.Extensions
{
    //---------------------------------------------------------------
    //GraphControl Luokan prototyyppi
    //---------------------------------------------------------------
    internal sealed class GraphControl : SimpleControl                                                              //LabelControl luokka
    {
        //Vakiot
        private const int _minLeft = 10;                                                                            //Marginaalin minimileveys vakioarvo
        private const int _minBottom = 4;
        //Yksityiset
        private readonly int _minRight;                                                                             //Ikkunan minimileveys
        private readonly TextBlock _titleText;                                                                               //Otsikon teksti
        private Offset _offset;                                                                                     //Marginaali
        private readonly Overlay _window;                                                                           //Ikkunan olio
        private LabelControl[]? _labels;                                                                            //Lämpötilamerkintöjen oliotaulukko
        private readonly VerticalStackPanel _labelStack;                                                            //Lämpötilamerkintöjen oliotaulukon puskuri
        private BarControl[]? _bars;                                                                                //Käyrän palkkien oliotaulukko
        private readonly HorizontalStackPanel _barStack;                                                            //käyrän palkkien oliotaulukon puskuri
        private readonly Overlay _pointers;                                                                         //Korkeimman ja matilimman lämpötilan osoittimet ikkunassa
        //-----------------------------------------------------------
        //GraphControl Luokan muodostin
        //Parametrit:
        //-Offset margin:
        //--Määrittää ikkunan marginaalin, Offset-rakenne,
        //-int count:
        //--Arvojen lukumäärä, integraali
        //-----------------------------------------------------------
        public GraphControl(Offset margin, int width)                                                               //GraphControl muodostin
        {
            _titleText = new TextBlock();                                                                           //Luodaan otsikkotekstin olio
            _offset = margin;                                                                                       //Luodaan ikkunan marginaali
            _minRight = width;                                                                                      //Ikkunan minimileveys
            _barStack = new HorizontalStackPanel();                                                                 //Luodaan puskuripaneeli palkeille
            _window = new Overlay                                                                                   //Luodaan ikkunaan Overlay-olio
            {
                TopContent = new Margin                                                                             //Luodaan otsikko ikkunan päälle
                {
                    Offset = new Offset(1, 0, 0, 0),                                                                //Siirretään tekstiä hieman sisemmälle
                    Content = _titleText                                                                            //Laaditaan otsikon tekstisisältö
                },
                BottomContent = new Border                                                                          //Luodaan ikkunan reunat
                {
                    BorderPlacement = BorderPlacement.All,                                                          //Elementin täyttävä reunus
                    BorderStyle = BorderStyle.Single,                                                               //Yhden viivan paksuinen reunus
                    Content = new Box                                                                               //Luodaan ikkunan sisältö
                    {
                        HorizontalContentPlacement = Box.HorizontalPlacement.Left,                                  //Järjestetään ikkunan sisältö alkamaan vasemmasta reunasta
                        VerticalContentPlacement = Box.VerticalPlacement.Bottom,                                    //Järjestetään ikkunan sisältö alkamaan alemmasta reunasta
                        Content = _barStack                                                                         //Määritetään ikkunan sisällöksi palkkien elementti
                    }
                }
            };
            _labelStack = new VerticalStackPanel();                                                                 //Luodaan lämpötilamerkinnöille puskuri
            _pointers = new Overlay();                                                                              //Tyhjä overlay antureiden ääriarvojen osoittimille
            int adjustedWidth = (ConsoleManager.WindowSize.Width - _minRight) - _minLeft;                           //Säädetään ikkunan marginaalin oikeanpuoleista päätyä käyrään sopivaksi
            Content = new Overlay                                                                                   //Terminaalin puskurin sisältö
            {
                TopContent = new Margin                                                                             //Marginaali, määrittää lämpötilamerkintöjen sijainnin
                {
                    Offset = new Offset(0, _offset.Top + 1, 0, 0),                                                  //Siirrä marginaalia rivin verran ikkunan yläreunaa alemmas
                    Content = new Overlay                                                                           //luodaan marginaali
                    {
                        TopContent = _labelStack,                                                                   //Sisällöksi lämpötilamerkintöjen puskuri
                        BottomContent = new Box                                                                     //Toisio sisällöksi uusi laatikko
                        {
                            VerticalContentPlacement = Box.VerticalPlacement.Bottom,                                //Asetellaan laatikon sisältö pohjalle
                            HorizontalContentPlacement = Box.HorizontalPlacement.Left,                              //Asetellaan laatikon sisältö vasempaan reunaan
                            Content = _pointers                                                                     //Laatikon sisällöksi ääriarvojen osoittimet
                        }
                    }
                },
                BottomContent = new Margin                                                                          //Marginaali, määrittää ikkunan koon suhteessa terminaalin kokoon
                {
                    Offset = new Offset                                                                             //Marginaalin alkupisteen olio
                        (
                            (_offset.Left < _minLeft) ?                                                             //Tarkistetaan onko marginaalin leveys alle minimiarvon
                            _minLeft : _offset.Left,                                                                //Asetetaan leveys minimiarvoksi
                            _offset.Top,                                                                            //Korkeus
                            (_offset.Right != adjustedWidth) ?                                                      //Tarkistetaan onko marginaalin leveys riittävä käyrälle
                            adjustedWidth : _offset.Right,                                                          //Asetetaan leveys aiemmin laskettuun arvoon
                            (_offset.Bottom != _minBottom) ?                                                        //Tarkistetaan onko marginaalin syvyys alle minimiarvon
                            _minBottom : _offset.Bottom                                                             //Asetetaan syvyys minimiarvoksi
                        ),
                    Content = _window                                                                               //Asetetaan ikkuna marginaaliin
                }
            };
        }
        //-----------------------------------------------------------
        //GraphControl Luokan [RefreshRange] funktio
        //Parametrit:
        //-FloatRange labels:
        //--Määrittää lämpötila-alueen, FloatRange-Rakenne,
        //-int spacing:
        //--Ikkunan lämpötilamerkintöjen väli riveissä, integraali
        //-----------------------------------------------------------
        public void RefreshRange(FloatRange labels, int spacing)                                                    //GraphControl RefreshRange funktio
        {
            int maxSize = GraphSize.Height;                                                                         //Haetaan ikkunan syvyys
            int numLabels = (maxSize / spacing) - 1;                                                                //Lasketaan ikkunaan mahtuvien merkintöjen lukumäärä
            _labels = new LabelControl[numLabels];                                                                  //Muodostetaan uusi taulukko merkintöjen tekstisisällölle
            for (int n = 0; n < numLabels; n++)                                                                     //Iteroidaan taulukon läpi ja luodaan uudet merkinnät
            {
                float iterator = (float)(numLabels - n) / numLabels;                                                //tehdään käänteinen iteraattori joka esittää pienenevää arvoa likulukujen 1.0 - 0.0 väliltä
                _labels[n] = new LabelControl                                                                       //Tehdään uusi lämpötilamerkintä _labels taulukkoon kohtaan 'n'
                {
                    Label = TerminalUtils.Lerp(labels, iterator).ToString("000.00"),                                //Formatoidaan lämpötilan liukuluko kahden desimaalin tarkkuuteen
                    Margin = new Offset(0, 0, 0, spacing)                                                           //Luodaan uusi marginaali arvojen väliin
                };
            }
            _labelStack.Children = _labels;                                                                         //Korvataan _labelStack:in jäsentaulukko _labels-taulukolla
        }
        //-----------------------------------------------------------
        //GraphControl Luokan [BuildStack] funktio
        //Parametrit:
        //-FloatRange range:
        //--Määrittää lämpötila-alueen, FloatRange-Rakenne,
        //-float[] val:
        //--Lämpötilat, liukulukutaulukko
        //-----------------------------------------------------------
        public void BuildStack(FloatRange range, float[] val)                                                       //GraphControl BuildStack funktio
        {
            int w = GraphSize.Width;                                                                                //Ikkunan leveys
            int h = GraphSize.Height;                                                                               //Ikkunan korkeus
            int minIndex = Array.IndexOf(val, val.Min());                                                           //Minimiarvon sijainti taulukossa
            int maxIndex = Array.IndexOf(val, val.Max());                                                           //Maksimiarvon sijainti taulukossa
            int count = val.Length;                                                                                 //lämpötila taulukon pituus
            if (count > w) return;                                                                                  //Palataan mikäli lämpötilat ei mahdu ikkunaan
            _bars = new BarControl[count];                                                                          //Muodostetaan uusi taulukko
            for (int n = 0; n < count; n++)                                                                         //Iteroidaan käyrän palkkien koko pituuden läpi
            {
                _bars[n] = new BarControl((int)val[n].Map(range, new FloatRange(1, h)));                            //Luodaan ja asetetaan palkki käyrän taulukkoon, uudelleen sovitetaan lämpötilataulukon lukemat ikkunan kokoon
            }
            _pointers.TopContent = new PointerControl(minIndex, val[minIndex], _minLeft, minIndex > maxIndex);      //Ääriosoittimen minimiarvo
            _pointers.BottomContent = new PointerControl(maxIndex, val[maxIndex], _minLeft, minIndex < maxIndex);   //Ääriosoittimen maksimiarvo
            _barStack.Children = _bars;                                                                             //Korvataan _barStack:in jäsentaulukko _bars-taulukolla
        }
        //-----------------------------------------------------------
        //Otsikko jäsenen kahva
        //-----------------------------------------------------------
        public string Title                                                                                         //Title jäsen
        {
            get => _titleText.Text;                                                                                 //Arvon palautuskahva
            set => _titleText.Text = value;                                                                         //Arvon asetuskahva
        }
        //-----------------------------------------------------------
        //Marginaali jäsenen kahva
        //-----------------------------------------------------------
        public Offset Margin                                                                                        //Margin jäsen
        {
            get => _offset;                                                                                         //Arvon palautuskahva
            set => _offset = value;                                                                                 //Arvon asetuskahva

        }
        //-----------------------------------------------------------
        //Elementin koon jäsenen kahva
        //-----------------------------------------------------------
        public Size GraphSize                                                                                       //GraphSize jäsen
        {
            get => _window.Size;                                                                                    //Arvon palautuskahva
        }
    }
}
