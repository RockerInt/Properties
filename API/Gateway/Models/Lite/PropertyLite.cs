using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Properties.Models.Lite
{
    public partial class PropertyLite
    {
        public PropertyLite()
        {
        }

        public Guid IdProperty { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int CodeInternal { get; set; }
        [Required]
        public short Year { get; set; }
        public Guid IdOwner { get; set; }
    }
}
