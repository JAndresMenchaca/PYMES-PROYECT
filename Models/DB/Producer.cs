using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Producer
{
    public int Id { get; set; }

    [Display(Name = "Ubicación")]
    [Required(ErrorMessage = "La ubicación es un campo obligatorio. Por favor, seleccione su ubicación.")]
    public string Longitude { get; set; } = null!;


    public string Latitude { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual Person IdNavigation { get; set; } = null!;

    public virtual ICollection<ProducerCompany> ProducerCompanies { get; set; } = new List<ProducerCompany>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
