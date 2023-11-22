using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Supply
{
    public int Id { get; set; }
 
    [Display(Name = "Precio Base")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Range(0, double.MaxValue, ErrorMessage = "El campo {0} debe ser un valor no negativo.")]
    [RegularExpression(@"^\d+(\.\d+)?(,\d+)?$", ErrorMessage = "El campo {0} debe ser un número decimal válido.")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser un número no negativo.")]
    public short Quantity { get; set; }


    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int IdRawMaterial { get; set; }

    public int IdSupplier { get; set; }

    public int UserId { get; set; }

    public virtual RawMaterial IdRawMaterialNavigation { get; set; } = null!;

    public virtual Supplier IdSupplierNavigation { get; set; } = null!;
}
