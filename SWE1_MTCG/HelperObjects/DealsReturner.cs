using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG.HelperObjects
{
    class DealsReturner
    {
        public string Username;
        public string CardId;
        public int Damage;
        public string WantedType;
        public int WantedDamage;
        public string DealId;

        public DealsReturner(string _username, string _cardid, int _damage, string _wantedtype, int _wanteddamage, string _dealid)
        {
            Username = _username;
            CardId = _cardid;
            Damage = _damage;
            WantedType = _wantedtype;
            WantedDamage = _wanteddamage;
            DealId = _dealid;
        }
    }
}
