using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG.HelperObjects
{
    class CardReturner
    {
        public string Name;
        public string Cardid;
        public int Damage;

        public CardReturner(string _name, string _cardid, int _damage)
        {
            Name = _name;
            Cardid = _cardid;
            Damage = _damage;
        }
    }
}
