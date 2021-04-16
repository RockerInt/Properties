using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Properties.Models.Lite
{
    public partial class PropertyTraceLite
    {
        public Guid IdPropertyTrace { get; set; } = Guid.NewGuid();
        public DateTime DateSale { get; set; } = DateTime.Now;
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        public decimal Tax { get; set; }
        [Required]
        public Guid IdProperty { get; set; }
    }
}
