using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class RawMaterial
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    public string Description { get; set; } = null!;


    [Display(Name = "Precio Unitario")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^\d{1,6}(\.\d{1,2})?(,\d{1,2})?$", ErrorMessage = "El campo {0} debe ser un número decimal válido con hasta seis dígitos antes del punto y puede usar punto o coma como separador decimal.")]
    public decimal UnitPrice { get; set; }



    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser un número no negativo.")]
    public int Stock { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }


    [Display(Name = "Unidad de Medida")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public short IdUnitType { get; set; }

    public int UserId { get; set; }

    public virtual UnitType? IdUnitTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductionDetail> ProductionDetails { get; set; } = new List<ProductionDetail>();

    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
