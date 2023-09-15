using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ProducerCompany
{
    public int IdProducer { get; set; }

    public short IdEnterprise { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual Enterprise IdEnterpriseNavigation { get; set; } = null!;

    public virtual Producer IdProducerNavigation { get; set; } = null!;
}
