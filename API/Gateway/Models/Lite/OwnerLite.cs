using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Properties.Models.Lite
{
    public partial class OwnerLite
    {
        public OwnerLite()
        {
        }

        public Guid IdOwner { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public byte[] Photo { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
    }
}
