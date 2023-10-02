using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class TownShip
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public byte IdTown { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Enterprise> Enterprises { get; set; } = new List<Enterprise>();

    public virtual Town? IdTownNavigation { get; set; } = null!;
}
