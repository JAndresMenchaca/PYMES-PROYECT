using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class ProductionDetail
{
    public int IdManufacturing { get; set; }

    public int IdRawMaterial { get; set; }

    public int Quantity { get; set; }

    public virtual Manufacturing? IdManufacturingNavigation { get; set; } = null!;

    public virtual RawMaterial? IdRawMaterialNavigation { get; set; } = null!;
}
