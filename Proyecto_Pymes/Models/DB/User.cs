using System;
using System.Collections.Generic;

namespace Proyecto_Pymes.Models.DB;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    /// <summary>
    /// administratorUnivalle
    /// BusinessManager
    /// Producer
    /// 
    /// </summary>
    public string Role { get; set; } = null!;

    public byte FirstLogin { get; set; }

    public byte Status { get; set; }

    public DateTime RegisterDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public int UserId { get; set; }

    public virtual Person IdNavigation { get; set; } = null!;
}
