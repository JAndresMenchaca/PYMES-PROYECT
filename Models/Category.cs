using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
