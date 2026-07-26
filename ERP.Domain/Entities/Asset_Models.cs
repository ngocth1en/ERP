using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Asset_Models
{
    public int Model_Id { get; set; }

    public string Model_Name { get; set; } = null!;

    public int Big_Type_Id { get; set; }

    public int Middle_Type_Id { get; set; }

    public int Small_Type_Id { get; set; }

    public byte[] ImageData { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool Is_Deleted { get; set; }

    public virtual ICollection<Assets_Information> Assets_Information { get; set; } = new List<Assets_Information>();
    public virtual ICollection<Asset_Inventories> Asset_Inventories { get; set; } = new List<Asset_Inventories>();
    public virtual Asset_Big_Types Big_Type { get; set; } = null!;

    public virtual Asset_Middle_Types Middle_Type { get; set; } = null!;

    public virtual Asset_Small_Types Small_Type { get; set; } = null!;
}
