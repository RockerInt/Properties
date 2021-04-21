using Properties.Models.Lite;
using System;
using System.Collections.Generic;

namespace Properties.Models.Complete
{
    public partial class Property
    {
        public Property()
        {
            PropertyImages = new HashSet<PropertyImageLite>();
            PropertyTraces = new HashSet<PropertyTraceLite>();
        }

        public Guid IdProperty { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int CodeInternal { get; set; }
        public short Year { get; set; }

        public virtual OwnerLite Owner { get; set; }
        public virtual ICollection<PropertyImageLite> PropertyImages { get; set; }
        public virtual ICollection<PropertyTraceLite> PropertyTraces { get; set; }
    }
}
