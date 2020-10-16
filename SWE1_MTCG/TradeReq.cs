using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class TradeReq:ITradeDeal
    {
        public ICard TradeCard { get; set; }

        public string Username { get; set; }

        public string ReqSpellOrMonster { get; set; }

        public int ReqMinDmg { get; set; }

        public EMonsterType ReqMonsterType { get; set; }
    }
}