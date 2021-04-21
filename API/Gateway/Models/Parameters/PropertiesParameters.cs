using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Models.Parameters
{
    public class PropertiesParameters: QueryStringParameters
    {
        public uint MinYear { get; set; }
        public uint MaxYear { get; set; } = (uint)DateTime.Now.Year;
        internal bool ValidYearRange => MaxYear > MinYear;


        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 99999999999999;
        internal bool ValidPriceRange => MaxPrice > MinPrice;

        public string GetQueryString() => $"{GetPaginingQueryString()}&MinYear={MinYear}&MaxYear={MaxYear}&MinPrice={MinPrice}&MaxPrice={MaxPrice}";
    }
}
