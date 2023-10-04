using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal BasePrise { get; set; }

    public int Stock { get; set; }

    public string UnitMeasure { get; set; } = null!;

    /// <summary>
    /// 1 si
    /// 2 no
    /// </summary>
    public byte ManufacturingNeed { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdProducer { get; set; }

    public int IdCategory { get; set; }

    public short? IdSector { get; set; }

    public int UserId { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual Producer IdProducerNavigation { get; set; } = null!;

    public virtual Sector? IdSectorNavigation { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Manufacturing> Manufacturings { get; set; } = new List<Manufacturing>();

    public virtual ICollection<Specification> Specifications { get; set; } = new List<Specification>();
}
