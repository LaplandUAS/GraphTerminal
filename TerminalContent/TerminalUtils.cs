namespace GraphTerminal.TerminalContent
{
    //---------------------------------------------------------------
    //FloatRange rakenne
    //---------------------------------------------------------------
    public readonly struct FloatRange                                                                               //FloatRange rakenteen otsikko
    {
        public FloatRange(float min, float max)                                                                     //Muodostin FloatRange rakenteelle
        {
            Minimum = min;                                                                                          //Rakenteen sisällön minimiarvo
            Maximum = max;                                                                                          //Rakenteen sisällön maksimiarvo
        }
        public float Minimum { get; }                                                                               //FloatRange minimiarvon kahva
        public float Maximum { get; }                                                                               //FloatRange maksimiarvon kahva
        public override string ToString() => $"[{Minimum} - {Maximum}]";                                            //FloatRange rakenteen sisällön formatointi merkkijonoon (DEBUG)
    }
    //---------------------------------------------------------------
    //TerminalUtils luokan prototyyppi
    //---------------------------------------------------------------
    public static class TerminalUtils                                                                               //TerminalUtils luokka
    {
        //-----------------------------------------------------------
        //TerminalUtils Luokan [Map] funktio
        //Parametrit:
        //-float v:
        //--Sovitettava luku, liukuluku,
        //-FloatRange f:
        //--Sovitettavan luvun ääriarvot, FloatRange-Rakenne,
        //-FloatRange t:
        //--Tuloksen ääriarvot joiden välille luku v sovitetaan, FloatRange-rakenne,
        //Palautusarvo:
        //-parametri v sovitettuna, liukuluku,
        //-----------------------------------------------------------
        public static float Map(this float v, FloatRange f, FloatRange t)                                           //TerminalUtils Map funktio
        {
            return (v - f.Minimum) / (f.Maximum - f.Minimum) * (t.Maximum - t.Minimum) + t.Minimum;                 //Palautetaan kahden liukulukualueen välille sovitettu liukuluku
        }
        //-----------------------------------------------------------
        //TerminalUtils Luokan [Lerp] funktio
        //Parametrit:
        //-FloatRange f:
        //--Interpoloitavien liukulukujen arvot, FloatRange-Rakenne,
        //-float by:
        //--Interpolaation määrä, liukuluku,
        //Palautusarvo:
        //-Interpoloitu luku, liukuluku,
        //-----------------------------------------------------------
        public static float Lerp(FloatRange f, float by)                                                            //TerminalUtils Lerp funktio
        {
            return f.Minimum * (1 - by) + f.Maximum * by;                                                           //Palautetaan f-rakenteen raja-arvojen välillä interpoloitu liukuluku
        }
        //-----------------------------------------------------------
        //TerminalUtils Luokan [NextFloat] funktio
        //Parametrit:
        //-FloatRange f:
        //--Satunaisen luvun ääriarvot, FloatRange-rakenne,
        //Palautusarvo:
        //-Satunainen luku annettujen ääriarvojen rajoissa, liukuluku,
        //-----------------------------------------------------------
        public static float NextFloat(FloatRange f)                                                                 //TerminalUtils NextFloat funktio
        {
            Random random = new();                                                                                  //Luodaan uusi satunaislukugeneraattori
            double val = (random.NextDouble() * (f.Maximum - f.Minimum) + f.Minimum);                               //Haetaan seuraava satunaisluku ja sovitetaan min - max lukujen välille
            return (float)val;                                                                                      //Palautetaan tulos liukulukuna
        }
    }
}
