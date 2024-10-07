using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public partial class Cart
{
    [Key]
    public Guid CartId { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
