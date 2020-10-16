using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public interface ITradeDeal
    {
        String Username { get; set; }
        ICard TradeCard { get; set; }
        string ReqSpellOrMonster { get; set; }
        int ReqMinDmg { get; set; }
    }
}