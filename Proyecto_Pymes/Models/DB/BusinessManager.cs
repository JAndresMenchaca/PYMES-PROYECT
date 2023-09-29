using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class BusinessManager
{
    public int Id { get; set; }

    public string? CorporateNumber { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public short IdEnterprise { get; set; }

    public int UserId { get; set; }

    public virtual Enterprise? IdEnterpriseNavigation { get; set; } = null!;

    public virtual Person? IdNavigation { get; set; } = null!;
}
