using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG.HelperObjects
{
    public class CardInBattle
    {
        public string Name;
        public string Type;
        public string Element;
        public int Damage;

        public CardInBattle(string _name, string _type, string _element, int _damage)
        {
            Name = _name;
            Type = _type;
            Element = _element;
            Damage = _damage;
        }
    }
}
