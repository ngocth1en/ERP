using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Asset_Middle_Types
{
    public string Middle_Type_Name_EN { get; set; } = null!;

    public string Middle_Type_Name_VN { get; set; } = null!;

    public string Machine_Type { get; set; } = null!;

    public int Big_Type_Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public int Middle_Type_Id { get; set; }

    public virtual ICollection<Asset_Small_Types> Asset_Small_Types { get; set; } = new List<Asset_Small_Types>();
    public virtual ICollection<Asset_Models> Asset_Models { get; set; } = new List<Asset_Models>();
    public virtual Asset_Big_Types Big_Type { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public string Inventory_Check_Type { get; set; } = "1";
}
