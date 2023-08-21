using ConsoleGUI.Controls;
using ConsoleGUI.Space;
using ConsoleGUI.UserDefined;
using System.Text;

namespace GraphTerminal.TerminalContent.Extensions
{
    //---------------------------------------------------------------
    //PointerControl Luokan prototyyppi
    //---------------------------------------------------------------
    internal class PointerControl : SimpleControl
    {
        //Vakiot
        private const int _minWidth = 10;                                                                           //Integraali, osoittimen minimileveyden määrittävä vakio
        //Yksityiset
        private Margin? _margin;                                                                                    //Osoittimen marginaali
        private int _startMargin;                                                                                   //Marginaalin aloituspiste
        private bool _textAlignLeft;                                                                                //Totuusarvo: asetetaanko osoitin vasempaan reunaan?
        private int _index;                                                                                         //Integraali, osoitettavan elementin sijainti
        private float _value;                                                                                       //Liukuluku, osoitettavan elementin arvo
        //-----------------------------------------------------------
        //PointerControl Luokan muodostin
        //Parametrit:
        //-int index:
        //--Määrittää osoittimen sijainnin, integraali,
        //-float value:
        //--Osoittimen arvo, liukuluku,
        //-int start:
        //--Määrittää marginaalin vasemman reunan, integraali,
        //-bool align:
        //--Määrittää onko osoitin vasemmanpuoleinen, totuusarvo
        //-----------------------------------------------------------
        public PointerControl(int index, float value, int start, bool align)                                        //PointerControl muodostin
        {
            _margin = new Margin();                                                                                 //Luodaan uusi marginaali
            _startMargin = start;                                                                                   //Asetetaan marginaalin arvo
            _index = index;                                                                                         //Asetetaan osoitettavan elementin lukema
            _value = value;                                                                                         //Asetetaan osoitettavan elementin arvo
            _textAlignLeft = align;                                                                                 //Asetetaan osoíttimen sijaintiarvo
            UpdateContent();                                                                                        //Päivitetään sisältö
            Content = _margin;                                                                                      //Asetetaan elementin sisällöksi marginaali
        }
        //-----------------------------------------------------------
        //PointerControl Luokan [UpdateContent] funktio,
        //Päivittää osoittimen sisällön
        //-----------------------------------------------------------
        public void UpdateContent()                                                                                 //PointerControl UpdateContent funktio
        {
            _margin = new Margin                                                                                    //Luodaan uusi marginaali
            {
                Offset = new Offset(_startMargin + (_index + 1), 0, 0, 0),                                          //Asetetaan marginaali _index arvon osoittamaan sijaintiin
                Content = new Boundary                                                                              //Luodaan uudet rajat
                {
                    Width = _minWidth + 1,                                                                          //Rajojen leveys minimileveydeksi
                    Height = 5,                                                                                     //Vähintään 5-riviä korkeutta
                    Content = new BreakPanel                                                                        //Luodaan uusi paneeli osoittimen sisällölle
                    {
                        Content = new TextBlock                                                                     //Luodaan uusi tekstielementti osoittimelle
                        {
                            Text = BuildContent(),                                                                  //Kootaan osoittimen sisältö
                        }
                    }
                }
            };
        }
        //-----------------------------------------------------------
        //PointerControl Luokan [BuildContent] funktio,
        //Kokoaa osoittimen sisällön
        //-----------------------------------------------------------
        private string BuildContent()                                                                               //PointerControl BuildContent funktio
        {
            StringBuilder? sb = new();                                                                              //Luodaan uusi puskuri merkiijonolle
            if (!AlignLeft)                                                                                         //Onko osoittimen sijainti vasemmalla
                sb.Append($"┬\n│\n│\n└No.{_index.ToString("000")}\n {_value.ToString("000.00")}°C");                //Mikäli ei, piirretään osoittimelle pidempi varsi
            else
                sb.Append($"┬\n└No.{_index.ToString("000")}\n {_value.ToString("000.00")}°C");                      //Mikäli on, piiretään osoittimelle lyhyempi varsi
            return sb.ToString();                                                                                   //Palautetaan valmis puskuri
        }
        //-----------------------------------------------------------
        //Aloituskohta jäsenen kahva
        //-----------------------------------------------------------
        public int Start                                                                                            //Start jäsen
        {
            get => _startMargin;                                                                                    //Arvon palautuskahva
            set => _startMargin = value;                                                                            //Arvon asetuskahva
        }
        //-----------------------------------------------------------
        //Puoli jäsenen kahva
        //-----------------------------------------------------------
        public bool AlignLeft                                                                                       //AlignLeft jäsen
        {
            get => _textAlignLeft;                                                                                  //Arvon palautuskahva
            set => _textAlignLeft = value;                                                                          //Arvon asetuskahva
        }
        //-----------------------------------------------------------
        //lukema jäsenen kahva
        //-----------------------------------------------------------
        public int Index                                                                                            //Index jäsen
        {
            get => _index;                                                                                          //Arvon palautuskahva
            set => _index = value;                                                                                  //Arvon asetuskahva
        }
        //-----------------------------------------------------------
        //Arvo jäsenen kahva
        //-----------------------------------------------------------
        public float Value                                                                                          //Value jäsen
        {
            get => _value;                                                                                          //Arvon palautuskahva
            set => _value = value;                                                                                  //Arvon asetuskahva
        }
    }
}
