using System;
using System.Collections.Generic;
using System.Text;
using SWE1_MTCG.HelperObjects;

namespace SWE1_MTCG
{
    public class User
    {
        public List<CardInBattle> ChallengerDeck = new List<CardInBattle>();
        public string Token { get; set; }

        public User(List<CardInBattle> _cards, string _token)
        {
            ChallengerDeck = _cards;
            Token = _token;
        }
    }
}