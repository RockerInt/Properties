using Properties.Models.Lite;
using System;
using System.Collections.Generic;

namespace Properties.Models.Complete
{
    public partial class PropertyTrace
    {
        public Guid IdPropertyTrace { get; set; }
        public DateTime DateSale { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Tax { get; set; }

        public virtual PropertyLite Property { get; set; }
    }
}
