using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class Category
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    [StringLength(30, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    public string Name { get; set; } = null!;

    [Display(Name = "Descripción")]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras y números.")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    public string? Description { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
