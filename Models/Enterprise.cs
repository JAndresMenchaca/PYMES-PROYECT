using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Enterprise
{
    public short Id { get; set; }

    public string GroupName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public byte[] Image { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public short IdTownShip { get; set; }

    public virtual ICollection<BusinessManager> BusinessManagers { get; set; } = new List<BusinessManager>();

    public virtual TownShip IdTownShipNavigation { get; set; } = null!;

    public virtual ICollection<ProducerCompany> ProducerCompanies { get; set; } = new List<ProducerCompany>();
}
