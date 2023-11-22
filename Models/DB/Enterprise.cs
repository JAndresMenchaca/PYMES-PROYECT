using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Proyecto_Pymes.Models.DB;

public partial class Enterprise
{
    public short Id { get; set; }

    [Display(Name = "Nombre de la Agrupación")]
    [Required(ErrorMessage = "El campo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 3)]
    [RegularExpression("^[A-Za-záéíóúüÁÉÍÓÚÜñÑ0-9\\s*]+$", ErrorMessage = "Solo se permiten letras, números.")]
    public string GroupName { get; set; } = null!;


    [Display(Name = "Dirección")]
    [Required(ErrorMessage = "El campo es obligatorio.")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 2)]
    [RegularExpression("^[A-Za-záéíóúüÁÉÍÓÚÜñÑ0-9#,.\\s]+$", ErrorMessage = "Solo se permiten letras, números y los caracteres #,-,.,(,)")]
    public string Address { get; set; } = null!;


    //Acepta Nulos
    public byte[]? Image { get; set; }


    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "El campo es obligatorio.")]
    [StringLength(149, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres.", MinimumLength = 3)]
    [RegularExpression("^[A-Za-záéíóúüÁÉÍÓÚÜñÑ0-9\\s*]+$", ErrorMessage = "Solo se permiten letras, números.")]
    public string Description { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "Por favor, selecciona un Municipio.")]
    public short IdTownShip { get; set; }

    public virtual ICollection<BusinessManager> BusinessManagers { get; set; } = new List<BusinessManager>();

    public virtual TownShip? IdTownShipNavigation { get; set; } = null!;

    public virtual ICollection<ProducerCompany> ProducerCompanies { get; set; } = new List<ProducerCompany>();
}
