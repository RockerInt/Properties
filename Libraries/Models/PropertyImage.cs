using System;
using System.Collections.Generic;

namespace Properties.Models
{
    public partial class PropertyImage
    {
        public Guid IdPropertyImage { get; set; } = Guid.NewGuid();
        public Guid IdProperty { get; set; }
        public byte[] File { get; set; }
        public bool Enabled { get; set; }

        public virtual Property Property { get; set; }
    }
}
