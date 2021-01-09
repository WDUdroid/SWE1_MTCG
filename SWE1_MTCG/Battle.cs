using System;
using System.Collections.Generic;
using System.Threading;
using Npgsql;
using SWE1_MTCG.HelperObjects;

namespace SWE1_MTCG
{
    public class Battle
    {
        public List<User> Challengers;
        public List<string> Result;
        public int Views;
        static readonly string ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcg";

        public Battle()
        {
            Challengers = new List<User>();
            Result = new List<string>();
        }
        public void BattleFunc()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (Challengers.Count == 2)
                {
                    Fight();
                }
            }
        }

        public int RegForBattle(string token)
        {
            if (Challengers.Count > 1)
            {
                return -100;
            }

            var cardList = new List<CardInBattle>();

            using var conFetchDeck = new NpgsqlConnection(ConnectionString);
            conFetchDeck.Open();

            string sqlFetchDeck = $"SELECT * FROM STACK INNER JOIN DECK ON STACK.cardid = DECK.cardid " +
                                  $"INNER JOIN CARDS ON STACK.name = CARDS.name " +
                                  $"INNER JOIN CREDENTIALS ON STACK.personID = CREDENTIALS.ID " +
                                  $"WHERE CREDENTIALS.token = '{token}'";

            using var cmdFetchDeck = new NpgsqlCommand(sqlFetchDeck, conFetchDeck);

            using var fetchReader = cmdFetchDeck.ExecuteReader();

            while (fetchReader.Read())
            {
                string name = fetchReader.GetString(fetchReader.GetOrdinal("name"));
                string type = fetchReader.GetString(fetchReader.GetOrdinal("type"));
                string element = fetchReader.GetString(fetchReader.GetOrdinal("element"));
                int damage = fetchReader.GetInt32(fetchReader.GetOrdinal("damage"));
                var tmpObject = new CardInBattle(name, type, element, damage);
                cardList.Add(tmpObject);
            }

            if (cardList.Count != 4)
            {
                Console.WriteLine(cardList.Count);
                return -101;
            }

            var newChallenger = new User(cardList, token);
            Challengers.Add(newChallenger);

            return 0;
        }

        public string CheckOnBattle()
        {
            while (true)
            {
                Thread.Sleep(10);
                if (Result.Count > 0)
                {
                    var resultBite = Result[0];
                    Views++;
                    if (Views == 2)
                    {
                        Views = 0;
                        Result.Clear();
                    }
                    return resultBite;
                }
            }
        }

        public void Fight()
        {
            Random randomPlayer0 = new Random();
            Random randomPlayer1 = new Random();

            var rounds = 1;
            string battleLog = "";

            while ((rounds < 100) && (Challengers[0].ChallengerDeck.Count != 0) && (Challengers[0].ChallengerDeck.Count != 8))
            {
                rounds++;

                double player0x = 1;
                double player1x = 1;
                string bonusMessage = "";

                var chosenCardPlayer0 = randomPlayer0.Next(0, Challengers[0].ChallengerDeck.Count);
                var chosenCardPlayer1 = randomPlayer1.Next(0, Challengers[1].ChallengerDeck.Count);

                var player0Card = Challengers[0].ChallengerDeck[chosenCardPlayer0];
                var player1Card = Challengers[1].ChallengerDeck[chosenCardPlayer1];

                // Monster vs Monster
                if ((player0Card.Type == "Monster") && (player1Card.Type == "Monster"))
                {
                    // Goblin against Dragon
                    if ((player0Card.Name.Contains("Goblin")) && (player1Card.Name.Contains("Dragon")))
                    {
                        player0x = 0;

                        //Chosen Goblin
                        Random player0ChosenGoblin = new Random();
                        if (player0ChosenGoblin.Next(0, 50) == 50)
                        {
                            player0x = 10;
                            bonusMessage = "THE CHOSEN GOBIN HAS RISEN AND GAINED 10 TIMES ITS STRENGTH AGAINST ITS COLDBLOODED NIGHTMARE!!!\r\n";
                        }


                    }
                    if ((player1Card.Name.Contains("Goblin")) && (player0Card.Name.Contains("Dragon")))
                    {
                        player1x = 0;

                        //Chosen Goblin
                        Random player1ChosenGoblin = new Random();
                        if (player1ChosenGoblin.Next(0, 50) == 50)
                        {
                            player1x = 10;
                            bonusMessage = "THE CHOSEN GOBIN HAS RISEN AND GAINED 10 TIMES ITS STRENGTH AGAINST ITS COLDBLOODED NIGHTMARE!!!\r\n";
                        }
                    }

                    //Wizzard against Ork
                    if ((player0Card.Name.Contains("Ork")) && (player1Card.Name.Contains("Wizzard")))
                    {
                        player0x = 0;

                        //Wizzard died of old age
                        Random player0WizzardDied = new Random();
                        if (player0WizzardDied.Next(0, 10) == 10)
                        {
                            player0x = 1;
                            player1x = 0;
                            bonusMessage = $"You know how it is :( Wizzards are old, they die sometimes, " +
                                           $"but at least they get reborn in the next round, just in the other deck... Muuuhaaaahaaaa\r\n";
                        }
                    }
                    if ((player1Card.Name.Contains("Ork")) && (player0Card.Name.Contains("Wizzard")))
                    {
                        player1x = 0;

                        //Wizzard died of old age
                        Random player1WizzardDied = new Random();
                        if (player1WizzardDied.Next(0, 10) == 10)
                        {
                            player0x = 0;
                            player1x = 1;
                            bonusMessage = $"You know how it is :( Wizzards are old, they die sometimes, " +
                                           $"but at least they get reborn in the next round, just in the other deck... Muuuhaaaahaaaa\r\n";
                        }
                    }

                    //FireElf against Dragon
                    if ((player0Card.Name.Contains("FireElf")) && (player1Card.Name.Contains("Dragon")))
                    {
                        player1x = 0;
                    }
                    if ((player1Card.Name.Contains("FireElf")) && (player0Card.Name.Contains("Dragon")))
                    {
                        player0x = 0;
                    }
                }

                //Monster vs Spell
                if ((player0Card.Type == "Spell") && (player1Card.Type == "Monster"))
                {
                    // Water Spell against Fire Monster
                    if ((player0Card.Element == "Water") && (player1Card.Element == "Fire"))
                    {
                        player0x = 2;
                    }

                    // Fire Spell against Normal Monster
                    if ((player0Card.Element == "Fire") && (player1Card.Element == "Normal"))
                    {
                        player0x = 2;
                    }

                    // Normal Spell against Water Monster
                    if ((player0Card.Element == "Normal") && (player1Card.Element == "Water"))
                    {
                        player0x = 2;
                    }

                    // Fire Spell against Water Monster
                    if ((player0Card.Element == "Fire") && (player1Card.Element == "Water"))
                    {
                        player0x = 0.5;
                    }

                    // Normal Spell against Fire Monster
                    if ((player0Card.Element == "Normal") && (player1Card.Element == "Fire"))
                    {
                        player0x = 0.5;
                    }

                    // Water Spell against Normal Monster
                    if ((player0Card.Element == "Water") && (player1Card.Element == "Normal"))
                    {
                        player0x = 0.5;
                    }

                    // Water Spell against Knight
                    if ((player0Card.Element == "Water") && (player1Card.Name.Contains("Knight")))
                    {
                        player1x = 0;
                    }

                    // Spell against Kraken
                    if (player1Card.Name.Contains("Kraken"))
                    {
                        player0x = 0;
                    }
                }

                if ((player1Card.Type == "Spell") && (player0Card.Type == "Monster"))
                {
                    // Water Spell against Fire Monster
                    if ((player1Card.Element == "Water") && (player0Card.Element == "Fire"))
                    {
                        player1x = 2;
                    }

                    // Fire Spell against Normal Monster
                    if ((player1Card.Element == "Fire") && (player0Card.Element == "Normal"))
                    {
                        player1x = 2;
                    }

                    // Normal Spell against Water Monster
                    if ((player1Card.Element == "Normal") && (player0Card.Element == "Water"))
                    {
                        player1x = 2;
                    }

                    // Fire Spell against Water Monster
                    if ((player1Card.Element == "Fire") && (player0Card.Element == "Water"))
                    {
                        player1x = 0.5;
                    }

                    // Normal Spell against Fire Monster
                    if ((player1Card.Element == "Normal") && (player0Card.Element == "Fire"))
                    {
                        player1x = 0.5;
                    }

                    // Water Spell against Normal Monster
                    if ((player1Card.Element == "Water") && (player0Card.Element == "Normal"))
                    {
                        player1x = 0.5;
                    }

                    // Water Spell against Knight
                    if ((player1Card.Element == "Water") && (player0Card.Name.Contains("Knight")))
                    {
                        player0x = 0;
                    }

                    // Spell against Kraken
                    if (player0Card.Name.Contains("Kraken"))
                    {
                        player1x = 0;
                    }
                }

                if ((player0Card.Type == "Spell") && (player1Card.Type == "Spell"))
                {
                    // Water Spell against Fire Spell
                    if ((player0Card.Element == "Water") && (player1Card.Element == "Fire"))
                    {
                        player0x = 2;
                    }

                    // Fire Spell against Normal Spell
                    if ((player0Card.Element == "Fire") && (player1Card.Element == "Normal"))
                    {
                        player0x = 2;
                    }

                    // Normal Spell against Water Spell
                    if ((player0Card.Element == "Normal") && (player1Card.Element == "Water"))
                    {
                        player0x = 2;
                    }

                    // Fire Spell against Water Spell
                    if ((player0Card.Element == "Fire") && (player1Card.Element == "Water"))
                    {
                        player0x = 0.5;
                    }

                    // Normal Spell against Fire Spell
                    if ((player0Card.Element == "Normal") && (player1Card.Element == "Fire"))
                    {
                        player0x = 0.5;
                    }

                    // Water Spell against Normal Spell
                    if ((player0Card.Element == "Water") && (player1Card.Element == "Normal"))
                    {
                        player0x = 0.5;
                    }

                    // Water Spell against Fire Spell
                    if ((player1Card.Element == "Water") && (player0Card.Element == "Fire"))
                    {
                        player1x = 2;
                    }

                    // Fire Spell against Normal Spell
                    if ((player1Card.Element == "Fire") && (player0Card.Element == "Normal"))
                    {
                        player1x = 2;
                    }

                    // Normal Spell against Water Spell
                    if ((player1Card.Element == "Normal") && (player0Card.Element == "Water"))
                    {
                        player1x = 2;
                    }

                    // Fire Spell against Water Spell
                    if ((player1Card.Element == "Fire") && (player0Card.Element == "Water"))
                    {
                        player1x = 0.5;
                    }

                    // Normal Spell against Fire Spell
                    if ((player1Card.Element == "Normal") && (player0Card.Element == "Fire"))
                    {
                        player1x = 0.5;
                    }

                    // Water Spell against Normal Spell
                    if ((player1Card.Element == "Water") && (player0Card.Element == "Normal"))
                    {
                        player1x = 0.5;
                    }

                    //Hides your Type wildcard
                    Random stupidGameMaster = new Random();
                    if (stupidGameMaster.Next(0, 2) == 1)
                    {
                        player0x = 1;
                        player1x = 1;
                        bonusMessage = $"The Game Master forgot which type your cards were. Ah who cares, just compare their damage stats.\r\n";
                    }

                }

                var roundString = $"{bonusMessage} {player0Card.Name} with a Dmg-Level of (Dmg * multi) {player0Card.Damage * player0x} " +
                                  $"fought against the almighty {player1Card.Name}, Dmg-Level (Dmg * multi) {player1Card.Damage * player1x} and ";

                if (player0Card.Damage * player0x > player1Card.Damage * player1x)
                {
                    roundString += "won this round!\r\n\r\n";
                    Challengers[1].ChallengerDeck.RemoveAt(chosenCardPlayer1);
                    Challengers[0].ChallengerDeck.Add(player1Card);
                }

                if (player1Card.Damage * player1x > player0Card.Damage * player0x)
                {
                    roundString += "lost this round!\r\n\r\n";
                    Challengers[0].ChallengerDeck.RemoveAt(chosenCardPlayer0);
                    Challengers[1].ChallengerDeck.Add(player0Card);
                }

                if (player0Card.Damage * player0x == player1Card.Damage * player1x)
                {
                    roundString += "it was a draw... :(\r\n\r\n";
                }

                battleLog += roundString;
            }

            using var conFetchPlayer0Username = new NpgsqlConnection(ConnectionString);
            conFetchPlayer0Username.Open();

            string sqlFetchPlayer0Username = $"SELECT username FROM CREDENTIALS WHERE token = '{Challengers[0].Token}'";
            using var cmdFetchPlayer0Username = new NpgsqlCommand(sqlFetchPlayer0Username, conFetchPlayer0Username);
            using var fetchPlayer0UsernameReader = cmdFetchPlayer0Username.ExecuteReader();

            fetchPlayer0UsernameReader.Read();
            string player0Username = fetchPlayer0UsernameReader.GetString(fetchPlayer0UsernameReader.GetOrdinal("username"));

            conFetchPlayer0Username.Close();


            using var conFetchPlayer1Username = new NpgsqlConnection(ConnectionString);
            conFetchPlayer1Username.Open();

            string sqlFetchPlayer1Username = $"SELECT username FROM CREDENTIALS WHERE token = '{Challengers[1].Token}'";
            using var cmdFetchPlayer1Username = new NpgsqlCommand(sqlFetchPlayer1Username, conFetchPlayer1Username);
            using var fetchPlayer1UsernameReader = cmdFetchPlayer1Username.ExecuteReader();

            fetchPlayer1UsernameReader.Read();
            string player1Username = fetchPlayer1UsernameReader.GetString(fetchPlayer1UsernameReader.GetOrdinal("username"));

            conFetchPlayer1Username.Close();


            using var conEloRead1 = new NpgsqlConnection(ConnectionString);
            conEloRead1.Open();

            string sqlEloRead1 = $"SELECT elo, id FROM CREDENTIALS INNER JOIN T_ELO " +
                                 $"ON CREDENTIALS.ID = T_ELO.personID " +
                                 $"WHERE token = '{Challengers[1].Token}'";

            using var cmdEloRead1 = new NpgsqlCommand(sqlEloRead1, conEloRead1);
            using var eloRead1Reader = cmdEloRead1.ExecuteReader();

            eloRead1Reader.Read();
            int player1Elo = eloRead1Reader.GetInt32(eloRead1Reader.GetOrdinal("elo"));
            int player1Id = eloRead1Reader.GetInt32(eloRead1Reader.GetOrdinal("id"));

            conEloRead1.Close();


            using var conEloRead0 = new NpgsqlConnection(ConnectionString);
            conEloRead0.Open();

            string sqlEloRead0 = $"SELECT elo, id FROM CREDENTIALS INNER JOIN T_ELO " +
                                 $"ON CREDENTIALS.ID = T_ELO.personID " +
                                 $"WHERE token = '{Challengers[0].Token}'";

            using var cmdEloRead0 = new NpgsqlCommand(sqlEloRead0, conEloRead0);
            using var eloRead0Reader = cmdEloRead0.ExecuteReader();

            eloRead0Reader.Read();
            int player0Elo = eloRead0Reader.GetInt32(eloRead0Reader.GetOrdinal("elo"));
            int player0Id = eloRead0Reader.GetInt32(eloRead0Reader.GetOrdinal("id"));

            conEloRead0.Close();

            if (Challengers[0].ChallengerDeck.Count == 0)
            {
                battleLog += $"{player1Username} won / {player0Username} lost with {rounds} rounds played\r\n";
                player0Elo = player0Elo - 5;
                player1Elo = player1Elo + 3;
            }

            if (Challengers[0].ChallengerDeck.Count == 8)
            {
                battleLog += $"{player0Username} won / {player1Username} lost with {rounds} rounds played\r\n";
                player1Elo = player1Elo - 5;
                player0Elo = player0Elo + 3;
            }
            
            else
            {
                battleLog += $"Draw between {player0Username} and {player1Username} with {rounds} rounds played\r\n";
            }

            battleLog += $"{player0Username} Elo {player0Elo} / {player1Username} Elo {player1Elo}\r\n";


            using var conEloWrite0 = new NpgsqlConnection(ConnectionString);
            conEloWrite0.Open();

            string sqlEloWrite0 = $"UPDATE T_ELO SET elo = {player0Elo} " +
                                  $"WHERE personid = {player0Id}";

            using var cmdEloWrite0 = new NpgsqlCommand(sqlEloWrite0, conEloWrite0);
            cmdEloWrite0.ExecuteNonQuery();

            conEloWrite0.Close();


            using var conEloWrite1 = new NpgsqlConnection(ConnectionString);
            conEloWrite1.Open();

            string sqlEloWrite1 = $"UPDATE T_ELO SET elo = {player1Elo} " +
                                  $"WHERE personid = {player1Id}";

            using var cmdEloWrite1 = new NpgsqlCommand(sqlEloWrite1, conEloWrite1);
            cmdEloWrite1.ExecuteNonQuery();

            conEloWrite1.Close();

            Result.Add(battleLog);
            Result.Add(battleLog);

            Challengers.Clear();
        }
    }
}