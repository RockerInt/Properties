using Properties.Models.Lite;
using System;
using System.Collections.Generic;

namespace Properties.Models.Complete
{
    public partial class Owner
    {
        public Owner()
        {
            Properties = new HashSet<PropertyLite>();
        }

        public Guid IdOwner { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public byte[] Photo { get; set; }
        public DateTime Birthday { get; set; }

        public virtual ICollection<PropertyLite> Properties { get; set; }
    }
}
