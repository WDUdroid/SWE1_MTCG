using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG.HelperObjects
{
    class ScoreReturner
    {
        public string Username;
        public int Elo;

        public ScoreReturner(string _username, int _elo)
        {
            Username = _username;
            Elo = _elo;
        }
    }
}
