using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class UnitType
{
    public short Id { get; set; }

    [Display(Name = "Unidad de Medida")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(49, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s/]+$", ErrorMessage = "El campo {0} solo debe contener letras, números y el carácter '/'.")]
    public string Type { get; set; } = null!;



    [Display(Name = "Descripción")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9ÁÉÍÓÚáéíóúÑñüÜ\s]+$", ErrorMessage = "El campo {0} solo debe contener letras y números.")]
    public string? Description { get; set; }


    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<RawMaterial> RawMaterials { get; set; } = new List<RawMaterial>();
}
