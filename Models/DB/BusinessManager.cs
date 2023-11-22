using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Pymes.Models.DB;

public partial class BusinessManager
{
    public int Id { get; set; }


    [Display(Name = "Número de Corporación")]
    [StringLength(8, ErrorMessage = "El campo {0} debe tener 8 caracteres.", MinimumLength = 7)]
    public string? CorporateNumber { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }


    [Display(Name = "Asociación")]
    [Required(ErrorMessage = "Por favor, selecciona una Asociación.")]
    public short IdEnterprise { get; set; }

    public int UserId { get; set; }

    public virtual Enterprise? IdEnterpriseNavigation { get; set; } = null!;

    public virtual Person? IdNavigation { get; set; } = null!;
}
