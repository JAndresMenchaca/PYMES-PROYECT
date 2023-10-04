using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class Sector
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short CapacityMax { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public short IdWareHouse { get; set; }

    public int UserId { get; set; }

    public virtual Warehouse IdWareHouseNavigation { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
