using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class TownShip
{
    public short Id { get; set; }

    [Display(Name = "Municipio")]
    [Required(ErrorMessage = "El campo es obligatorio.")]
    [StringLength(20, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 3)]
    [RegularExpression("^[a-zA-ZáéíóúüÁÉÍÓÚÜ\\s*]+$", ErrorMessage = "El campo {0} solo debe contener letras.")]
    public string Name { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    [Required(ErrorMessage = "Por favor, selecciona un Departamento.")]
    public byte IdTown { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Enterprise> Enterprises { get; set; } = new List<Enterprise>();

    public virtual Town? IdTownNavigation { get; set; } = null!;
}
