using System;
using System.Collections.Generic;

namespace Properties.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Properties = new HashSet<Property>();
        }

        public Guid IdOwner { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Address { get; set; }
        public byte[] Photo { get; set; }
        public DateTime Birthday { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
