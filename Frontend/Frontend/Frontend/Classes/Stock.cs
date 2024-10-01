using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Classes
{
    public class Stock
    {
        public Stock(string stockName, int amount)
        {
            StockName = stockName;
            this.Amount = amount;
        }

        public string StockName { get; set; }
        public int Amount { get; set; }


    }
}
