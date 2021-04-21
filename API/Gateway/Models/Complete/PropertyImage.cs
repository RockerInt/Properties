using Properties.Models.Lite;
using System;
using System.Collections.Generic;

namespace Properties.Models.Complete
{
    public partial class PropertyImage
    {
        public Guid IdPropertyImage { get; set; }
        public byte[] File { get; set; }
        public bool Enabled { get; set; }

        public virtual PropertyLite Property { get; set; }
    }
}
