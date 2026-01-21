using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Asset_Small_Types
{
    public int Small_Type_Id { get; set; }

    public string Small_Type_Name_EN { get; set; } = null!;

    public string Small_Type_Name_VN { get; set; } = null!;

    public int Middle_Type_Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Asset_Models> Asset_Models { get; set; } = new List<Asset_Models>();

    public virtual Asset_Middle_Types Middle_Type { get; set; } = null!;
}
