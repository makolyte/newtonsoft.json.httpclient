using System;
using System.Collections.Generic;
using System.Text;

namespace NewtonsoftHttpClientExample
{
    public class Stock
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset QuoteTime { get; set; }
        public FundTypes FundType { get; set; }
    }
    public enum FundTypes
    {
        Stock,
        MutualFund,
        ETF
    }
}
