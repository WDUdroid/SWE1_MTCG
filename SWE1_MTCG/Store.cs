using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_MTCG
{
    public class Store
    {
        public List<ITradeDeal> TradeDeals { get; set; }

        public List<ICard> Package()
        {
            throw new System.NotImplementedException();
        }

        public ICard Trade()
        {
            throw new System.NotImplementedException();
        }
    }
}