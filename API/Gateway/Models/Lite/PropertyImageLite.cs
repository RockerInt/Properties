using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Properties.Models.Lite
{
    public partial class PropertyImageLite
    {
        [Required]
        public Guid IdPropertyImage { get; set; } = Guid.NewGuid();
        [Required]
        public Guid IdProperty { get; set; }
        public byte[] File { get; set; }
        [Required]
        public bool Enabled { get; set; }
    }
}
