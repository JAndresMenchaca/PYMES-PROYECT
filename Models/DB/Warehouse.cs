using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Warehouse
{
    public short Id { get; set; }


    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras y números.")]
    public string Name { get; set; } = null!;


    [Display(Name = "Localidad")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras y números.")]
    public string? Location { get; set; }


    [Display(Name = "Capacidad Maxima")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^\d{1,5}$", ErrorMessage = "El campo {0} debe ser un número positivo con hasta 5 dígitos.")]
    public short CapacityMax { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
