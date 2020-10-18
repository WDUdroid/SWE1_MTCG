using System;

namespace SWE1_MTCG
{
    public class CardSpecs : ICard
    {
        public CardSpecs(int select)
        {
            var rand = new Random();

            if (select == 1)
            {
                this.Name = "KnightCard";
                //Expecting Knight
                this.MonsterType = EMonsterType.Knight;
                this.Element = EElement.normal;
                this.Damage = 30;
            }
            else if (select == 2)
            {
                this.Name = "DragonCard";
                //Expecting Dragon
                this.MonsterType = EMonsterType.Dragon;
                this.Element = EElement.fire;
                this.Damage = 100;
            }
            else if (select == 3)
            {
                this.Name = "GoblinCard";
                //Expecting Goblin
                this.MonsterType = EMonsterType.Goblin;
                this.Element = EElement.normal;
                this.Damage = 20;
            }
            else if (select == 4)
            {
                this.Name = "OrkCard";
                //Expecting Ork
                this.MonsterType = EMonsterType.Ork;
                this.Element = EElement.normal;
                this.Damage = 50;
            }
            else if (select == 5)
            {
                this.Name = "FireElveCard";
                //Expecting FireElve
                this.MonsterType = EMonsterType.FireElve;
                this.Element = EElement.fire;
                this.Damage = 15;
            }
            else if (select == 6)
            {
                this.Name = "KrakenCard";
                //Expecting Kraken
                this.MonsterType = EMonsterType.Kraken;
                this.Element = EElement.water;
                this.Damage = 100;
            }
            else if (select == 7)
            {
                this.Name = "WizardCard";
                //Expecting Wizard
                this.MonsterType = EMonsterType.Wizard;
                this.Element = EElement.normal;
                this.Damage = 60;
            }
            else if (select == 8)
            {
                var randDamage = rand.Next(1, 3);
                this.Name = "FireSpell" + randDamage + "Card";
                //Expecting Spell with element fire
                this.MonsterType = EMonsterType.Spell;
                this.Element = EElement.fire;
                this.Damage = randDamage * randDamage * 10;
            }
            else if (select == 9)
            {
                var randDamage = rand.Next(1, 3);
                this.Name = "WaterSpell" + randDamage + "Card";
                //Expecting spell with element water
                this.MonsterType = EMonsterType.Spell;
                this.Element = EElement.water;
                this.Damage = randDamage * randDamage * 10;
            }
            else if (select == 10)
            {
                var randDamage = rand.Next(1, 3);
                this.Name = "NormalSpell" + randDamage + "Card";
                //Expecting Spell with element normal
                this.MonsterType = EMonsterType.Spell;
                this.Element = EElement.normal;
                this.Damage = randDamage * randDamage * 10;
            }
            else
            {
                this.Name = "UnknownCard";
                //Expecting unknown
                this.MonsterType = EMonsterType.Unknown;
                this.Element = EElement.Unknown;
                this.Damage = 0;
            }
        }

        public CardSpecs()
        {
            this.Name = "UnknownCard";
            //Expecting unknown
            this.MonsterType = EMonsterType.Unknown;
            this.Element = EElement.Unknown;
            this.Damage = 0;
        }

        public string Name { get; set; }

        public int Damage { get; set; }

        public EElement Element { get; set; }

        public EMonsterType MonsterType { get; set; }
    }
}