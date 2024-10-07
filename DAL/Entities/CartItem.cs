using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public partial class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }

        [ForeignKey("Koi")]
        public Guid KoiId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public virtual Cart Cart { get; set; } = null!;
        
        public virtual Koi Koi { get; set; } = null!;
    }
}
