using System;
using System.Collections.Generic;

namespace SWE1_MTCG
{
    public class Store
    {
        public List<ITradeDeal> TradeDeals { get; set; }

        public static List<ICard> Package()
        {
            List<ICard> generatedCards = new List<ICard>();
            int[] cardArray = new int[4];
            var rand = new Random();

            for (int i = 0; i <= 4; i++)
            {
                generatedCards.Add(new CardSpecs(rand.Next(1, 10)));
            }

            return generatedCards;
        }

        public ICard Trade()
        {
            throw new System.NotImplementedException();
        }
    }
}