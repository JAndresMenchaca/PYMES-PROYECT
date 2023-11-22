using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Product
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

    [Display(Name = "Precio Base")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Range(1, double.MaxValue, ErrorMessage = "El campo {0} debe ser un valor no negativo.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?(,\d{1,2})?$", ErrorMessage = "El campo {0} debe ser un número decimal válido con hasta dos decimales y puede usar punto o coma como separador decimal.")]
    [DataType(DataType.Currency, ErrorMessage = "El campo {0} debe ser un número válido.")]
    public decimal BasePrise { get; set; }


    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser un número no negativo.")]
    public int Stock { get; set; }


    [Display(Name = "Unidad de Medida")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s/]+$", ErrorMessage = "El campo {0} solo debe contener letras, números y el carácter '/'.")]
    public string UnitMeasure { get; set; } = null!;

    /// <summary>
    /// 1 si
    /// 2 no
    /// </summary>
    /// 
    [Display(Name = "Manufactura")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public byte ManufacturingNeed { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int? IdProducer { get; set; }

    [Display(Name = "Categoría")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int IdCategory { get; set; }


    [Display(Name = "Sector")]
    public short? IdSector { get; set; }

    public int UserId { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; } = null!;

    public virtual Producer? IdProducerNavigation { get; set; } = null!;

    public virtual Sector? IdSectorNavigation { get; set; }

    public virtual ICollection<Image>? Images { get; set; } = new List<Image>();

    public virtual ICollection<Manufacturing>? Manufacturings { get; set; } = new List<Manufacturing>();

    public virtual ICollection<Specification>? Specifications { get; set; } = new List<Specification>();
}
