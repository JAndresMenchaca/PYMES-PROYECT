using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Manufacturing
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal CostProduction { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdBatch { get; set; }

    public virtual ManufacturingBatch IdBatchNavigation { get; set; } = null!;

    public virtual ICollection<ManufacturingBatch> ManufacturingBatches { get; set; } = new List<ManufacturingBatch>();

    public virtual ICollection<ProductionDetail> ProductionDetails { get; set; } = new List<ProductionDetail>();
}
