using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class SpellCard:ICard
    {
        public int Damage { get; set; }

        public string Name { get; set; }

        public EElement Element { get; set; }
    }
}