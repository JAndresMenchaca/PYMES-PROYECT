using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class Warehouse
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public short CapacityMax { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
