using ConsoleGUI.Controls;
using ConsoleGUI.Space;
using ConsoleGUI.UserDefined;

namespace GraphTerminal.TerminalContent.Extensions
{
    //---------------------------------------------------------------
    //LabelControl Luokan prototyyppi
    //---------------------------------------------------------------
    internal class LabelControl : SimpleControl                                                                     //LabelControl Luokka
    {
        private readonly TextBlock _label;                                                                          //Merkin tekstisisältö
        private readonly Margin _margin;                                                                            //Merkin marginaali
        //-----------------------------------------------------------
        //LabelControl Luokan muodostin
        //-----------------------------------------------------------
        public LabelControl()                                                                                       //LabelControl muodostin
        {
            _label = new TextBlock();                                                                               //Luodaan uusi _label jäsen
            _margin = new Margin                                                                                    //Muodostetaan luokan marginaali ja tekstisisältö
            {
                Offset = new Offset(0, 0, 0, 0),                                                                    //Asetetaan marginaali nolla-arvoon
                Content = _label,                                                                                   //Asetetaan sisällöksi _label
            };
            Content = _margin;                                                                                      //Sisäistetään elementti luokan sisältöön
        }
        //-----------------------------------------------------------
        //Marginaali jäsenen kahva
        //-----------------------------------------------------------
        public Offset Margin                                                                                        //Margin jäsen
        {
            get => _margin.Offset;                                                                                  //Arvon palautuskahva
            set => _margin.Offset = value;                                                                          //Arvon asetuskahva
        }
        //-----------------------------------------------------------
        //Teksti jäsenen kahva
        //-----------------------------------------------------------
        public string Label                                                                                         //Label jäsen
        {
            get => _label.Text;                                                                                     //Arvon palautuskahva
            set => _label.Text = $"[{value}]°C┤";                                                                   //Arvon asetuskahva (Formatoidaan lämpötila-arvo)
        }
    }
}
