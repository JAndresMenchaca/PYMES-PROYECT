using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class Manufacturing
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal CostProduction { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdProduct { get; set; }

    public int UserId { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual ICollection<ProductionDetail> ProductionDetails { get; set; } = new List<ProductionDetail>();
}
