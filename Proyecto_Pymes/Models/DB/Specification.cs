using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class Specification
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdProduct { get; set; }

    public int UserId { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
