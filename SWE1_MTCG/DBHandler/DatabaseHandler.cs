using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Npgsql;
using SWE1_MTCG.HelperObjects;

namespace SWE1_MTCG.DBHandler
{
    class DatabaseHandler
    {
        static readonly string ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcg";

        // Register Request: /users
        public static int RegRequest(string username, string password)
        {
            long userAlreadyExists = CountOccurrence("Credentials", "username", username);
            if (userAlreadyExists >= 1) return -1;

            InsertIntoCredentials(username, password);
            NewEntries(username);

            return 0;
        }

        // Login Request: /sessions
        public static string LoginRequest(string username, string password)
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            var fromTable = "credentials";

            string sql1 = $"SELECT Count(*) FROM {fromTable} WHERE username = '{username}' AND password = '{password}'";
            using var cmd1 = new NpgsqlCommand(sql1, con);

            Int64 exi = (Int64)cmd1.ExecuteScalar();

            if (exi <= 0) return "-1";

            string sql2 = $"SELECT token FROM {fromTable} WHERE username = '{username}' AND password = '{password}'";
            using var cmd2 = new NpgsqlCommand(sql2, con);

            string token = (string)cmd2.ExecuteScalar();

            return token;
        }
        public static int AddPackage(dynamic data, string admin)
        {
            if (admin != "Basic admin-mtcgToken")
            {
                return -1;
            }

            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            for (int i = 0; i <= 4; i++)
            {
                string sql1 = $"SELECT Count(*) FROM CARDS WHERE name = '{data[i]["Name"]}'";
                using var cmd1 = new NpgsqlCommand(sql1, con);

                Int64 exi = (Int64)cmd1.ExecuteScalar();

                if (exi <= 0) return -1;

                string sql2 = $"SELECT Count(*) FROM T_PACKAGE WHERE cardid = '{data[i]["Id"]}'";
                using var cmd2 = new NpgsqlCommand(sql2, con);

                exi = (Int64)cmd2.ExecuteScalar();

                if (exi > 0) return -1;

                string sql3 = $"SELECT Count(*) FROM STACK WHERE cardid = '{data[i]["Id"]}'";
                using var cmd3 = new NpgsqlCommand(sql2, con);

                exi = (Int64)cmd3.ExecuteScalar();

                if (exi > 0) return -1;

                if (int.Parse((string) data[i]["Damage"]) > 100) return -1;
                if (int.Parse((string)data[i]["Damage"]) < 0) return -1;
            }

            string sql = $"SELECT MAX(packageID) FROM T_PACKAGE";
            using var cmd = new NpgsqlCommand(sql, con);

            int nextID = 0;

            if (cmd.ExecuteScalar() == DBNull.Value) nextID = 0;
            else nextID = (int)cmd.ExecuteScalar();
            nextID += 1;

            for (int i = 0; i <= 4; i++)
            {
                string sql1 = 
                    $"INSERT INTO T_PACKAGE (packageID, cardID, name, damage) VALUES ('{nextID}', '{data[i]["Id"]}' ,'{data[i]["Name"]}', {int.Parse((string)data[i]["Damage"])})";
                using var cmd1 = new NpgsqlCommand(sql1, con);
                cmd1.ExecuteNonQuery();
            }

            return 0;
        }
        public static long CountOccurrence(string fromTable, string columnName, string whereCondition)
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            string sql = $"SELECT Count(*) FROM {fromTable} WHERE {columnName} = '{whereCondition}'";
            using var cmd = new NpgsqlCommand(sql, con);

            Int64 exi = (Int64) cmd.ExecuteScalar();

            return exi;
        }
        public static int NewEntries(string username)
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            string sql = $"SELECT ID FROM CREDENTIALS WHERE username = '{username}'";
            using var cmd1 = new NpgsqlCommand(sql, con);

            Int64 userID = (Int64)cmd1.ExecuteScalar();

            using var cmd2 = new NpgsqlCommand();
            cmd2.Connection = con;
            cmd2.CommandText = $"INSERT INTO T_COINS (personID, coins) VALUES ('{userID}', 20)";
            cmd2.ExecuteNonQuery();

            using var cmd3 = new NpgsqlCommand();
            cmd3.Connection = con;
            cmd3.CommandText = $"INSERT INTO T_ELO (personID, elo) VALUES ('{userID}', 100)";
            cmd3.ExecuteNonQuery();

            using var cmd4 = new NpgsqlCommand();
            cmd4.Connection = con;
            cmd4.CommandText = $"INSERT INTO T_PAGE (personID, name, bio, image) VALUES ('{userID}', '{username}', 'Hey I am new', '8-) Noobie')";
            cmd4.ExecuteNonQuery();

            return 0;
        }
        public static int InsertIntoCredentials(string username, string password)
        {
            var token = "Basic " + username + "-mtcgToken";

            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"INSERT INTO credentials (username, password, token) VALUES ('{username}', '{password}', '{token}')";
            cmd.ExecuteNonQuery();

            return 0;
        }
        public static int Transaction(string token)
        {
            using var con = new NpgsqlConnection(ConnectionString);
            con.Open();

            int coinsOwned = 0;

            string sql1 = $"SELECT coins FROM t_coins INNER JOIN credentials ON personid = id WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con);

            if ((cmd1.ExecuteScalar() == DBNull.Value) ||
                ((int)cmd1.ExecuteScalar() < 5) || (cmd1.ExecuteScalar() == null)) return -1;
            else coinsOwned = (int)cmd1.ExecuteScalar();


            string sql2 = $"SELECT MIN(packageID) FROM T_PACKAGE";
            using var cmd2 = new NpgsqlCommand(sql2, con);

            int packageNum = 0;

            if (cmd2.ExecuteScalar() == DBNull.Value) return -1;
            else packageNum = (int)cmd2.ExecuteScalar();

            string sql3 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd3 = new NpgsqlCommand(sql3, con);

            Int64 personID = 0;

            if (cmd3.ExecuteScalar() == DBNull.Value) return -1;
            else personID = (Int64)cmd3.ExecuteScalar();
            con.Close();

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql4 = $"SELECT * FROM T_PACKAGE WHERE packageID = {packageNum}";
            using var cmd4 = new NpgsqlCommand(sql4, con2);


            using var reader = cmd4.ExecuteReader();

            while (reader.Read())
            {
                
                string cardID = reader.GetString(reader.GetOrdinal("cardid"));
                string name = reader.GetString(reader.GetOrdinal("name"));
                int damage = reader.GetInt32(reader.GetOrdinal("damage"));

                using var con3 = new NpgsqlConnection(ConnectionString);
                con3.Open();
                string sqlDropFromPackage = $"DELETE FROM T_PACKAGE WHERE cardid = '{cardID}'";
                using var cmdDropFromPackage = new NpgsqlCommand(sqlDropFromPackage, con3);
                cmdDropFromPackage.ExecuteNonQuery();
                cmdDropFromPackage.Dispose();
                con3.Close();

                using var con4 = new NpgsqlConnection(ConnectionString);
                con4.Open();
                string sqlInsertIntoStack = $"INSERT INTO STACK (personID, cardID, name, damage) " +
                                             $"VALUES ({personID}, '{cardID}', '{name}', {damage})";
                using var cmdInsertIntoStack = new NpgsqlCommand(sqlInsertIntoStack, con4);
                cmdInsertIntoStack.ExecuteNonQuery();
                cmdInsertIntoStack.Dispose();
                con4.Close();
            }

            using var con5 = new NpgsqlConnection(ConnectionString);
            con5.Open();
            string sqlUpdateCoins = $"UPDATE T_COINS " +
                                    $"SET coins = {coinsOwned - 5}" +
                                    $"WHERE personID = {personID}";
            using var cmdUpdateCoins = new NpgsqlCommand(sqlUpdateCoins, con5);
            cmdUpdateCoins.ExecuteNonQuery();
            cmdUpdateCoins.Dispose();
            con5.Close();

            return 0;
        }
        public static string GetCards(string token)
        {
            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return "-1";
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM STACK WHERE personID = {personID}";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var reader = cmd2.ExecuteReader();

            var cardList = new List<CardReturner>();

            while (reader.Read())
            {

                string name = reader.GetString(reader.GetOrdinal("name"));
                string cardID = reader.GetString(reader.GetOrdinal("cardid"));
                int damage = reader.GetInt32(reader.GetOrdinal("damage"));

                var tmpObject = new CardReturner(name, cardID, damage);

                cardList.Add(tmpObject);
            }

            return JsonConvert.SerializeObject(cardList);
        }
        public static string GetDeck(string token)
        {


            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return "-1";
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM DECK WHERE personID = {personID}";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var deckReader = cmd2.ExecuteReader();

            var cardList = new List<CardReturner>();

            while (deckReader.Read())
            {
                string deckCardID = deckReader.GetString(deckReader.GetOrdinal("cardid"));

                using var con3 = new NpgsqlConnection(ConnectionString);
                con3.Open();

                string sql3 = $"SELECT * FROM STACK WHERE cardID = '{deckCardID}'";
                using var cmd3 = new NpgsqlCommand(sql3, con3);

                using var stackReader = cmd3.ExecuteReader();

                while (stackReader.Read())
                {
                    string name = stackReader.GetString(stackReader.GetOrdinal("name"));
                    string cardID = stackReader.GetString(stackReader.GetOrdinal("cardid"));
                    int damage = stackReader.GetInt32(stackReader.GetOrdinal("damage"));
                    var tmpObject = new CardReturner(name, cardID, damage);
                    cardList.Add(tmpObject);
                }
            }

            return JsonConvert.SerializeObject(cardList);
        }
        public static string ConfigureDeck(dynamic data, string token)
        {


            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return "-1";
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();

            long doTheyExist = 0;
            long alreadyInTrade = 0;

            for (int i = 0; i <= 3; i++)
            {
                using var con2 = new NpgsqlConnection(ConnectionString);
                con2.Open();

                string sql2 = $"SELECT COUNT(*) FROM STACK WHERE personID = {personID} AND cardID = '{data[i]}'";
                using var cmd2 = new NpgsqlCommand(sql2, con2);

                if ((cmd2.ExecuteScalar() == DBNull.Value) || (cmd2.ExecuteScalar() == null)) return "-1";
                else doTheyExist += (Int64)cmd2.ExecuteScalar();
                con2.Close();

            }

            if (doTheyExist != 4)
            {
                return "-1";
            }

            for (int i = 0; i <= 3; i++)
            {
                using var con2 = new NpgsqlConnection(ConnectionString);
                con2.Open();

                string sql2 = $"SELECT COUNT(*) FROM T_TRADE WHERE cardtoID = '{data[i]}'";
                using var cmd2 = new NpgsqlCommand(sql2, con2);

                if ((cmd2.ExecuteScalar() == DBNull.Value) || (cmd2.ExecuteScalar() == null)) return "-1";
                else alreadyInTrade += (Int64)cmd2.ExecuteScalar();
                con2.Close();

            }

            if (alreadyInTrade != 0)
            {
                return "-1";
            }

            using var con3 = new NpgsqlConnection(ConnectionString);
            con3.Open();

            string sql3 = $"DELETE FROM DECK WHERE personID = {personID}";
            using var cmd3 = new NpgsqlCommand(sql3, con3);
            cmd3.ExecuteNonQuery();
            con3.Close();

            using var con4 = new NpgsqlConnection(ConnectionString);
            con4.Open();

            for (int i = 0; i <= 3; i++)
            {
                string sql4 = $"INSERT INTO DECK (personID, cardID) " +
                              $"VALUES ({personID}, '{data[i]}')";
                using var cmd4 = new NpgsqlCommand(sql4, con4);
                cmd4.ExecuteNonQuery();
            }

            con4.Close();

            return "0";
        }
        public static string GetUserdata(string token, string path)
        {
            string[] pathFilter = path.Split("/");

            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return "-1";
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();


            using var conComp = new NpgsqlConnection(ConnectionString);
            conComp.Open();

            string sqlComp = $"SELECT id FROM CREDENTIALS WHERE username = '{pathFilter[2]}'";
            using var cmdComp = new NpgsqlCommand(sqlComp, conComp);

            Int64 personIDComp = 0;

            if ((cmdComp.ExecuteScalar() == DBNull.Value) || (cmdComp.ExecuteScalar() == null)) return "-1";
            else personIDComp = (Int64)cmdComp.ExecuteScalar();
            conComp.Close();

            if (personID != personIDComp)
            {
                return "-1";
            }

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM t_page WHERE personID = {personID}";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var reader = cmd2.ExecuteReader();
            reader.Read();

            string name = reader.GetString(reader.GetOrdinal("name"));
            string bio = reader.GetString(reader.GetOrdinal("bio"));
            string image = reader.GetString(reader.GetOrdinal("image"));

            con2.Close();

            var tmpObject = new PageReturner(name, bio, image);

            return JsonConvert.SerializeObject(tmpObject);
        }
        public static int PutUserdata(string token, string path, dynamic data)
        {
            string[] pathFilter = path.Split("/");

            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return -1;
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();


            using var conComp = new NpgsqlConnection(ConnectionString);
            conComp.Open();

            string sqlComp = $"SELECT id FROM CREDENTIALS WHERE username = '{pathFilter[2]}'";
            using var cmdComp = new NpgsqlCommand(sqlComp, conComp);

            Int64 personIDComp = 0;

            if ((cmdComp.ExecuteScalar() == DBNull.Value) || (cmdComp.ExecuteScalar() == null)) return -1;
            else personIDComp = (Int64)cmdComp.ExecuteScalar();
            conComp.Close();

            if (personID != personIDComp)
            {
                return -1;
            }

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"DELETE FROM T_PAGE WHERE personID = {personID}";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            cmd2.ExecuteNonQuery();
            con2.Close();

            using var con3 = new NpgsqlConnection(ConnectionString);
            con3.Open();

            string sql3 = $"INSERT INTO T_PAGE (personID, name, bio, image) " +
                          $"VALUES ({personID}, '{data["Name"]}', '{data["Bio"]}', '{data["Image"]}')";
            using var cmd3 = new NpgsqlCommand(sql3, con3);

            cmd3.ExecuteNonQuery();
            con3.Close();

            return 0;
        }
        public static string GetStats(string token)
        {
            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null)) return "-1";
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();


            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM t_elo WHERE personID = {personID}";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var reader = cmd2.ExecuteReader();
            reader.Read();

            return $"Your Elo-Score is {reader.GetInt32(reader.GetOrdinal("elo"))}";
        }
        public static string GetScoreboard()
        {
            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM T_ELO INNER JOIN CREDENTIALS ON personID = ID ORDER BY ELO";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var reader = cmd2.ExecuteReader();

            var cardList = new List<ScoreReturner>();

            while (reader.Read())
            {

                string username = reader.GetString(reader.GetOrdinal("username"));
                int elo = reader.GetInt32(reader.GetOrdinal("elo"));

                var tmpObject = new ScoreReturner(username, elo);

                cardList.Add(tmpObject);
            }

            return JsonConvert.SerializeObject(cardList);
        }
        public static string GetDeals()
        {
            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT * FROM T_TRADE INNER JOIN STACK ON T_TRADE.cardtoID = STACK.cardID" +
                          $" INNER JOIN CREDENTIALS ON T_TRADE.traderID = CREDENTIALS.id";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            using var reader = cmd2.ExecuteReader();

            var cardList = new List<DealsReturner>();

            while (reader.Read())
            {

                string username = reader.GetString(reader.GetOrdinal("username"));
                string cardid = reader.GetString(reader.GetOrdinal("cardid"));
                int damage = reader.GetInt32(reader.GetOrdinal("damage"));
                string wantedtype = reader.GetString(reader.GetOrdinal("wantedtyp"));
                int wantedDamage = reader.GetInt32(reader.GetOrdinal("minimumdamage"));
                string dealid = reader.GetString(reader.GetOrdinal("dealid"));

                var tmpObject = new DealsReturner(username, cardid, damage, wantedtype, wantedDamage, dealid);

                cardList.Add(tmpObject);
            }

            return JsonConvert.SerializeObject(cardList);
        }
        public static string AddTrade(dynamic data, string token)
        {


            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();

            long doTheyExist = 0;
            long alreadyInDeck = 0;

            
            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT COUNT(*) FROM STACK WHERE personID = {personID} AND cardID = '{data["CardToTrade"]}'";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            if ((cmd2.ExecuteScalar() == DBNull.Value) || (cmd2.ExecuteScalar() == null))
            {
                return "-1";
            }
            else doTheyExist += (Int64)cmd2.ExecuteScalar();
            con2.Close();

            

            if (doTheyExist != 1)
            {
                return "-1";
            }

            long cardInStack = 0;

            using var conW = new NpgsqlConnection(ConnectionString);
            conW.Open();

            string sqlW = $"SELECT COUNT(*) FROM STACK WHERE personID = {personID} AND cardID = '{data["CardToTrade"]}'";
            using var cmdW = new NpgsqlCommand(sqlW, conW);

            if ((cmdW.ExecuteScalar() == DBNull.Value) || (cmdW.ExecuteScalar() == null))
            {
                return "-1";
            }
            else cardInStack += (Int64)cmdW.ExecuteScalar();
            conW.Close();

            if (cardInStack != 1)
            {
                return "-1";
            }

            long idInTrade = 0;

            using var conTrade = new NpgsqlConnection(ConnectionString);
            conTrade.Open();

            string sqlTrade = $"SELECT COUNT(*) FROM T_TRADE WHERE dealID = '{data["Id"]}'";
            using var cmdTrade = new NpgsqlCommand(sqlTrade, conTrade);

            if ((cmdTrade.ExecuteScalar() == DBNull.Value) || (cmdTrade.ExecuteScalar() == null))
            {
                return "-1";
            }
            else idInTrade += (Int64)cmdTrade.ExecuteScalar();
            conTrade.Close();



            if (idInTrade != 0)
            {
                return "-1";
            }


            using var conD = new NpgsqlConnection(ConnectionString);
            conD.Open();

            string sqlD = $"SELECT COUNT(*) FROM DECK WHERE cardID = '{data["CardToTrade"]}'";
            using var cmdD = new NpgsqlCommand(sqlD, conD);

            if ((cmdD.ExecuteScalar() == DBNull.Value) || (cmdD.ExecuteScalar() == null))
            {
                return "-1";
            }
            else alreadyInDeck += (Int64)cmdD.ExecuteScalar();
            conD.Close();
            

            if (alreadyInDeck != 0)
            {
                return "-1";
            }

            long alreadyInTrade = 0;

            using var conT = new NpgsqlConnection(ConnectionString);
            conT.Open();

            string sqlT = $"SELECT COUNT(*) FROM T_TRADE WHERE cardtoID = '{data["CardToTrade"]}'";
            using var cmdT = new NpgsqlCommand(sqlT, conT);

            if ((cmdT.ExecuteScalar() == DBNull.Value) || (cmdT.ExecuteScalar() == null))
            {
                return "-1";
            }
            else alreadyInTrade += (Int64)cmdT.ExecuteScalar();
            conT.Close();


            if (alreadyInTrade != 0)
            {
                return "-1";
            }

            using var con3 = new NpgsqlConnection(ConnectionString);
            con3.Open();

            string type = data["Type"];
            if (type == "monster") type = "Monster";
            if (type == "spell") type = "Spell";
            if ((type != "Monster") && (type != "Spell"))
            {
                return "-1";
            }

            string sql3 = $"INSERT INTO T_TRADE (traderID, dealid, cardtoid, wantedtyp, minimumDamage) " +
                          $"VALUES ({personID}, '{data["Id"]}', '{data["CardToTrade"]}', '{type}', {data["MinimumDamage"]})";
            using var cmd3 = new NpgsqlCommand(sql3, con3);
            cmd3.ExecuteNonQuery();
            con3.Close();

            return "0";
        }

        public static string DeleteTrade(string token, string path)
        {
            string[] pathFilter = path.Split("/");

            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();

            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT traderID FROM T_TRADE WHERE dealID = '{pathFilter[2]}'";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            Int32 personIDComp = 0;

            if ((cmd2.ExecuteScalar() == DBNull.Value) || (cmd2.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personIDComp = (Int32)cmd2.ExecuteScalar();
            con2.Close();

            if (personIDComp != personID) return "-1";

            using var con3 = new NpgsqlConnection(ConnectionString);
            con3.Open();

            string sql3 = $"DELETE FROM T_TRADE WHERE dealID = '{pathFilter[2]}'";
            using var cmd3 = new NpgsqlCommand(sql3, con3);
            cmd3.ExecuteNonQuery();

            con3.Close();

            return "0";
        }

        public static string Trade(string token, string path, dynamic data)
        {
            string[] pathFilter = path.Split("/");

            using var con1 = new NpgsqlConnection(ConnectionString);
            con1.Open();

            string sql1 = $"SELECT id FROM CREDENTIALS WHERE token = '{token}'";
            using var cmd1 = new NpgsqlCommand(sql1, con1);

            Int64 personID = 0;

            if ((cmd1.ExecuteScalar() == DBNull.Value) || (cmd1.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personID = (Int64)cmd1.ExecuteScalar();
            con1.Close();


            using var conComp = new NpgsqlConnection(ConnectionString);
            conComp.Open();

            string sqlComp = $"SELECT traderID FROM T_TRADE WHERE dealID = '{pathFilter[2]}'";
            using var cmdComp = new NpgsqlCommand(sqlComp, conComp);

            Int32 personIDComp = 0;

            if ((cmdComp.ExecuteScalar() == DBNull.Value) || (cmdComp.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personIDComp = (Int32)cmdComp.ExecuteScalar();
            conComp.Close();

            if (personIDComp == personID)
            {
                return "-1";
            }


            using var con2 = new NpgsqlConnection(ConnectionString);
            con2.Open();

            string sql2 = $"SELECT COUNT(*) FROM T_TRADE WHERE dealID = '{pathFilter[2]}'";
            using var cmd2 = new NpgsqlCommand(sql2, con2);

            Int64 dealExists = 0;

            if ((cmd2.ExecuteScalar() == DBNull.Value) || (cmd2.ExecuteScalar() == null))
            {
                return "-1";
            }
            else dealExists = (Int64)cmd2.ExecuteScalar();
            con2.Close();

            if (dealExists != 1)
            {
                return "-1";
            }


            using var con3 = new NpgsqlConnection(ConnectionString);
            con3.Open();

            Int64 personHasCard = 0;

            string sql3 = $"SELECT COUNT(*) FROM STACK WHERE cardID = '{data}' AND personID = {personID}";
            using var cmd3 = new NpgsqlCommand(sql3, con3);

            if ((cmd3.ExecuteScalar() == DBNull.Value) || (cmd3.ExecuteScalar() == null))
            {
                return "-1";
            }
            else personHasCard = (Int64)cmd3.ExecuteScalar();
            con3.Close();

            if (personHasCard != 1)
            {
                return "-1";
            }


            using var con4 = new NpgsqlConnection(ConnectionString);
            con4.Open();

            string sql4 = $"SELECT * FROM STACK INNER JOIN CARDS ON STACK.name = CARDS.name " +
                          $"WHERE cardid = '{data}'";
            using var cmd4 = new NpgsqlCommand(sql4, con4);

            using var readerGivenCard = cmd4.ExecuteReader();

            readerGivenCard.Read();

            string givenType = readerGivenCard.GetString(readerGivenCard.GetOrdinal("type"));
            int givenDamage = readerGivenCard.GetInt32(readerGivenCard.GetOrdinal("damage"));

            con4.Close();


            using var con5 = new NpgsqlConnection(ConnectionString);
            con5.Open();

            string sql5 = $"SELECT * FROM T_TRADE " +
                          $"WHERE dealid = '{pathFilter[2]}'";
            using var cmd5 = new NpgsqlCommand(sql5, con5);

            using var readerWantedCard = cmd5.ExecuteReader();

            readerWantedCard.Read();

            string wantedType = readerWantedCard.GetString(readerWantedCard.GetOrdinal("wantedtyp"));
            string presentedCard = readerWantedCard.GetString(readerWantedCard.GetOrdinal("cardtoid"));
            int wantedDamage = readerWantedCard.GetInt32(readerWantedCard.GetOrdinal("minimumdamage"));
            int trader = readerWantedCard.GetInt32(readerWantedCard.GetOrdinal("traderid"));

            con5.Close();

            if (wantedType != givenType)
            {
                return "-1";
            }

            if (wantedDamage > givenDamage)
            {
                return "-1";
            }

            using var con6 = new NpgsqlConnection(ConnectionString);
            con6.Open();

            string sql6 = $"DELETE FROM T_TRADE WHERE dealID = '{pathFilter[2]}'";
            using var cmd6 = new NpgsqlCommand(sql6, con6);
            cmd6.ExecuteNonQuery();

            con6.Close();

            using var con7 = new NpgsqlConnection(ConnectionString);
            con7.Open();

            string sql7 = $"UPDATE STACK SET personID = {trader} WHERE cardID = '{data}'";
            using var cmd7 = new NpgsqlCommand(sql7, con7);
            cmd7.ExecuteNonQuery();

            con7.Close();


            using var con8 = new NpgsqlConnection(ConnectionString);
            con8.Open();

            string sql8 = $"UPDATE STACK SET personID = {personID} WHERE cardID = '{presentedCard}'";
            using var cmd8 = new NpgsqlCommand(sql8, con8);
            cmd8.ExecuteNonQuery();

            con8.Close();

            return "0";
        }
        //public static int NewCard(string name, string type, string element)
        //{
        //    using var con = new NpgsqlConnection(ConnectionString);
        //    con.Open();

        //    using var cmd2 = new NpgsqlCommand();
        //    cmd2.Connection = con;
        //    cmd2.CommandText = $"INSERT INTO CARDS (name, type, element) VALUES ('{name}', '{type}', '{element}')";
        //    cmd2.ExecuteNonQuery();

        //    return 0;
        //}
    }
}
