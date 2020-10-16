using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class User
    {
        private bool Authenticated = false;
        public int Coins { get; set; }

        public string Username { get; set; }

        public Stash UserStash { get; set; }

        public Deck UserDeck { get; set; }

        public string AuthToken { get; set; }

        public string Password { get; set; }

        public void Authenticate()
        {
            throw new System.NotImplementedException();
        }

        public void BattleRequest()
        {
            throw new System.NotImplementedException();
        }
    }
}