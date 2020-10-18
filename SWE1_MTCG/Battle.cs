using System;
using System.Collections.Generic;

namespace SWE1_MTCG
{
    public class Battle
    {
        public List<User> Challengers { get; set; }

        public List<string> NewBattle(User playerOne, User playerTwo)
        {
            if (playerOne == null) throw new ArgumentNullException(nameof(playerOne));
            if (playerTwo == null) throw new ArgumentNullException(nameof(playerTwo));

            var rand = new Random();

            var playerOneFighters = new List<ICard>();
            var playerTwoFighters = new List<ICard>();

            var playerOneDuplicate = playerOne;
            var playerTwoDuplicate = playerTwo;

            for (int i = 0; i <= 4; i++)
            {
                int chooserOne = rand.Next(4 - i);
                playerOneFighters.Add(playerOneDuplicate.UserDeck.CardsInDeck[chooserOne]);
                playerOneDuplicate.UserDeck.CardsInDeck.RemoveAt(chooserOne);
                int chooserTwo = rand.Next(4 - i);
                playerTwoFighters.Add(playerTwoDuplicate.UserDeck.CardsInDeck[chooserTwo]);
                playerTwoDuplicate.UserDeck.CardsInDeck.RemoveAt(chooserTwo);
            }

            var playerOneDeckPosition = 1;
            var playerTwoDeckPosition = 1;
            var attackingPlayer = 1;

            var fightLog = new List<string>();

            for (int i = 0; i <= 100; i++)
            {
                var playerOneBonus = 1;
                var playerTwoBonus = 1;

                if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Spell) ^
                    (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Spell))
                {
                    if ((playerOneFighters[playerOneDeckPosition].Element == EElement.water) &&
                        (playerTwoFighters[playerTwoDeckPosition].Element == EElement.fire))
                    {
                        playerOneBonus = 2;
                    }
                    else if ((playerOneFighters[playerOneDeckPosition].Element == EElement.fire) &&
                        (playerTwoFighters[playerTwoDeckPosition].Element == EElement.water))
                    {
                        playerTwoBonus = 2;
                    }
                    else if ((playerOneFighters[playerOneDeckPosition].Element == EElement.fire) &&
                             (playerTwoFighters[playerTwoDeckPosition].Element == EElement.normal))
                    {
                        playerOneBonus = 2;
                    }
                    else if ((playerOneFighters[playerOneDeckPosition].Element == EElement.normal) &&
                             (playerTwoFighters[playerTwoDeckPosition].Element == EElement.fire))
                    {
                        playerTwoBonus = 2;
                    }
                    else if ((playerOneFighters[playerOneDeckPosition].Element == EElement.water) &&
                             (playerTwoFighters[playerTwoDeckPosition].Element == EElement.normal))
                    {
                        playerOneBonus = 2;
                    }
                    else if ((playerOneFighters[playerOneDeckPosition].Element == EElement.normal) &&
                             (playerTwoFighters[playerTwoDeckPosition].Element == EElement.water))
                    {
                        playerTwoBonus = 2;
                    }
                    else
                    {
                        throw new ArgumentException("Fighter Element not clear.");
                    }
                }

                if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Goblin) &&
                    (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Dragon))
                {
                    playerOneBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Dragon) &&
                    (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Goblin))
                {
                    playerTwoBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Ork) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Wizard))
                {
                    playerOneBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Wizard) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Ork))
                {
                    playerTwoBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Knight) &&
                         (playerTwoFighters[playerTwoDeckPosition].Element == EElement.water))
                {
                    playerOneBonus = 0;
                }
                else if ((playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Knight) &&
                         (playerOneFighters[playerOneDeckPosition].Element == EElement.water))
                {
                    playerTwoBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Spell) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Kraken))
                {
                    playerOneBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Kraken) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Spell))
                {
                    playerTwoBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.Dragon) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.FireElve))
                {
                    playerOneBonus = 0;
                }
                else if ((playerOneFighters[playerOneDeckPosition].MonsterType == EMonsterType.FireElve) &&
                         (playerTwoFighters[playerTwoDeckPosition].MonsterType == EMonsterType.Dragon))
                {
                    playerTwoBonus = 0;
                }

                if (playerOneFighters[playerOneDeckPosition].Damage * playerOneBonus >
                    playerTwoFighters[playerTwoDeckPosition].Damage * playerTwoBonus)
                {
                    playerOneFighters.Add(playerTwoFighters[playerTwoDeckPosition]);
                    playerTwoFighters.RemoveAt(playerTwoDeckPosition);

                    if (attackingPlayer == 1)
                    {
                        fightLog.Add("Player 1 attacked with " +
                                     playerOneFighters[playerOneDeckPosition].Name + " and bested Player 2s " +
                                     playerTwoFighters[playerTwoDeckPosition].Name);
                    }
                    else
                    {
                        fightLog.Add("Player 2 attacked with " +
                                     playerTwoFighters[playerTwoDeckPosition].Name + " and was defeated by Player 1s " +
                                     playerOneFighters[playerOneDeckPosition].Name);
                    }

                    if (playerTwoFighters.Count == 0) break;
                    attackingPlayer = attackingPlayer % 2 + 1;
                }
                
                else if ((playerOneFighters[playerOneDeckPosition].Damage * playerOneBonus ==
                          playerTwoFighters[playerTwoDeckPosition].Damage * playerTwoBonus) && attackingPlayer == 1)
                {
                    playerTwoFighters.Add(playerOneFighters[playerOneDeckPosition]);
                    playerOneFighters.RemoveAt(playerOneDeckPosition);

                    fightLog.Add("Player 1 attacked with " +
                                 playerOneFighters[playerOneDeckPosition].Name + " and bested Player 2s " +
                                 playerTwoFighters[playerTwoDeckPosition].Name);

                    if (playerOneFighters.Count == 0) break;
                    attackingPlayer = attackingPlayer % 2 + 1;
                }
                
                else if ((playerOneFighters[playerOneDeckPosition].Damage * playerOneBonus ==
                          playerTwoFighters[playerTwoDeckPosition].Damage * playerTwoBonus) && attackingPlayer == 2)
                {
                    playerTwoFighters.Add(playerOneFighters[playerOneDeckPosition]);
                    playerOneFighters.RemoveAt(playerOneDeckPosition);

                    fightLog.Add("Player 2 attacked with " +
                                 playerTwoFighters[playerTwoDeckPosition].Name + " and bested Player 1s " +
                                 playerOneFighters[playerOneDeckPosition].Name);

                    if (playerTwoFighters.Count == 0) break;
                    attackingPlayer = attackingPlayer % 2 + 1;
                }
                
                else if (playerOneFighters[playerOneDeckPosition].Damage * playerOneBonus <
                         playerTwoFighters[playerTwoDeckPosition].Damage * playerTwoBonus)
                {
                    playerTwoFighters.Add(playerOneFighters[playerOneDeckPosition]);
                    playerOneFighters.RemoveAt(playerOneDeckPosition);

                    if (attackingPlayer == 1)
                    {
                        fightLog.Add("Player 1 attacked with " +
                                     playerOneFighters[playerOneDeckPosition].Name + " and was defeated by Player 2s " +
                                     playerTwoFighters[playerTwoDeckPosition].Name);
                    }
                    else
                    {
                        fightLog.Add("Player 2 attacked with " +
                                     playerTwoFighters[playerTwoDeckPosition].Name + " and bested Player 1s " +
                                     playerOneFighters[playerOneDeckPosition].Name);
                    }

                    if (playerOneFighters.Count == 0) break;
                    attackingPlayer = attackingPlayer % 2 + 1;
                }
                
                else
                {
                    throw new ArgumentException("Fighter Damage not clear.");
                }
            }

            if (playerOneFighters.Count != 0 && playerTwoFighters.Count != 0)
            {
                fightLog.Add("Player 1 and Player 2 tied");
            }
            else if (playerOneFighters.Count != 0 && playerTwoFighters.Count == 0)
            {
                fightLog.Add("Player 1 won");
            }
            else if (playerOneFighters.Count == 0 && playerTwoFighters.Count != 0)
            {
                fightLog.Add("Player 2 won");
            }

            return fightLog;
        }
    }
}