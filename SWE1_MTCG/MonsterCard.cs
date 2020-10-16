using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class MonsterCard:ICard
    {
        public string Name { get; set; }

        public int Damage { get; set; }

        public EElement Element { get; set; }

        public EMonsterType MonsterType { get; set; }
    }
}