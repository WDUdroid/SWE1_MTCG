using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class Server
    {
        public List<ScoreboardEntry> Scoreboard { get; set; }

        public Battle Battle { get; set; }

        public Store Store { get; set; }

        public void ClientReception()
        {
            throw new System.NotImplementedException();
        }
    }
}