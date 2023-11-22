using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Manufacturing
{
    public int Id { get; set; }


    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[1-9]\d{0,8}$", ErrorMessage = "Por favor, ingrese un valor positivo con un máximo de 9 dígitos.")]
    public int Quantity { get; set; }


    [Display(Name = "Costo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^\d{1,6}$", ErrorMessage = "El {0} debe tener entre 1 y 6 dígitos enteros.")]
    [Range(1, int.MaxValue, ErrorMessage = "El {0} debe ser un número entero positivo.")]
    public decimal CostProduction { get; set; }


    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    [Required(ErrorMessage = "Por favor, selecciona un Producto.")]
    public int IdProduct { get; set; }

    public int UserId { get; set; }

    public virtual Product? IdProductNavigation { get; set; } = null!;


    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MinLength(1, ErrorMessage = "Debe ingresar al menos un detalle de producción en {0}.")]
    public virtual ICollection<ProductionDetail> ProductionDetails { get; set; } = new List<ProductionDetail>();
}
