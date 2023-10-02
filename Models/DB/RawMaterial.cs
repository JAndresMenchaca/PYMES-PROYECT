using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class RawMaterial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public int Stock { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public short IdUnitType { get; set; }

    public int UserId { get; set; }

    public virtual UnitType IdUnitTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductionDetail> ProductionDetails { get; set; } = new List<ProductionDetail>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
