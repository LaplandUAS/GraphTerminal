using ConsoleGUI.Controls;
using ConsoleGUI.Data;
using ConsoleGUI.UserDefined;
using System.Text;

namespace GraphTerminal.TerminalContent.Extensions
{
    //---------------------------------------------------------------
    //BarControl Luokan prototyyppi
    //---------------------------------------------------------------
    internal class BarControl : SimpleControl                                                                       //BarControl luokka
    {
        private char[] barItems = { '░', '▒', '▓', '█' };                                                           //Palkin merkit
        //-----------------------------------------------------------
        //BarControl Luokan muodostin
        //Parametrit:
        //-int height:
        //--Määrittää palkin pituuden, integraali,
        //-Color color:
        //--Palkin väri, Color rakenne
        //-----------------------------------------------------------
        public BarControl(int height, Color? color = null)                                                          //BarControl luokan muodostin
        {
            StringBuilder sb = new StringBuilder();                                                                 //Luodaan uusi puskuri merkkijonolle
            for(int h = 0; h < height; h++)                                                                         //Iteroidaan palkin korkeuden läpi
            {
                sb.Append(barItems[(int)(((float)h/height)*4)]);                                                    //Lisätään sopiva merkki pituudelle taulukosta "barItems"
            }

            Content = new Box                                                                                       //Luodaan elementtiin suorakulmion muotoinen alue
            {
                VerticalContentPlacement = Box.VerticalPlacement.Bottom,                                            //Asetetaan sisältö suorakulmion pohjaan
                Content = new Boundary                                                                              //Luodaan palkin kokoa esittävä raja suorakulmioon
                {
                    Width = 1,                                                                                      //Asetetaan rajan leveys yhden merkin kokoiseksi
                    Height = height,                                                                                //asetetaan rajan korkeus palkin pituudeksi
                    Content = new Background                                                                        //Luodaan rajan sisälle tekstityyli
                    {
                        Color = color ?? Color.Black,                                                               //Asetetaan palkin taustaväri mustaksi, mikäli palkin vakioarvo on tyhjä.
                        Content = new WrapPanel                                                                     //Tehdään tekstityylistä monirivinen jotta merkkijno jatkuisi koko palkin pituudelta
                        {
                            Content = new TextBlock                                                                 //Täytetään paneeli tekstillä
                            {
                                Text = sb.ToString()                                                                //asetetaan merkkijonopuskuri sisällöksi
                            }
                        }
                    }
                }
            };
        }
    }
}
