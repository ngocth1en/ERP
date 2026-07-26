using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Asset_Big_Types
{
    public string Big_Type_Name_EN { get; set; } = null!;

    public string Big_Type_Name_VN { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int Big_Type_Id { get; set; }

    public virtual ICollection<Asset_Middle_Types> Asset_Middle_Types { get; set; } = new List<Asset_Middle_Types>();

    public virtual ICollection<Asset_Models> Asset_Models { get; set; } = new List<Asset_Models>();
}
