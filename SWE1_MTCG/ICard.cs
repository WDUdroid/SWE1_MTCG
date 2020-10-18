using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public interface ICard
    {
        int Damage { get; set; }
        string Name { get; set; }

        EElement Element { get; set; }

        EMonsterType MonsterType { get; set; }
    }
}