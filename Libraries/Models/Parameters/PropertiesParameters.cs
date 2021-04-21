using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Properties.Models.Parameters
{
    public class PropertiesParameters: QueryStringParameters
    {
        public uint MinYear { get; set; }
        public uint MaxYear { get; set; } = (uint)DateTime.Now.Year;
        public bool ValidYearRange => MaxYear > MinYear;


        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 99999999999999;
        public bool ValidPriceRange => MaxPrice > MinPrice;
    }
}
