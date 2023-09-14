using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ManufacturingBatch
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public string Description { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdProduct { get; set; }

    public int IdManufacturing { get; set; }

    public virtual Manufacturing IdManufacturingNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual ICollection<Manufacturing> Manufacturings { get; set; } = new List<Manufacturing>();
}
