using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Enterprise
{
    public short Id { get; set; }

    [Required(ErrorMessage = "Campo Obligatorio")]
    public string GroupName { get; set; } = null!;

    [Required(ErrorMessage = "Campo Obligatorio")]
    public string Address { get; set; } = null!;

    [DataType(DataType.Upload)]
    public byte[]? Image { get; set; } = null!;

    [Required(ErrorMessage = "Campo Obligatorio")]
    public string Description { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Campo Obligatorio")]
    public short IdTownShip { get; set; }

    public virtual ICollection<BusinessManager> BusinessManagers { get; set; } = new List<BusinessManager>();

    public virtual TownShip? IdTownShipNavigation { get; set; } = null!;

    public virtual ICollection<ProducerCompany> ProducerCompanies { get; set; } = new List<ProducerCompany>();
}
