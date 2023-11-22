using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Sector
{
    public short Id { get; set; }


    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(55, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras y números.")]
    public string Name { get; set; } = null!;


    [Display(Name = "Capacidad Maxima")]
    [RegularExpression(@"^\d{1,5}$", ErrorMessage = "El campo {0} debe ser un número positivo con hasta 5 dígitos.")]
    public short? CapacityMax { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }


    [Display(Name = "Almacenamiento")]
    [Required(ErrorMessage = "Por favor, selecciona un Almacén.")]
    public short IdWareHouse { get; set; }

    public int UserId { get; set; }

    public virtual Warehouse? IdWareHouseNavigation { get; set; } = null!;

    public virtual ICollection<Product>? Products { get; set; } = new List<Product>();
}
