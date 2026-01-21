using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Assets_Information
{
    public string AssetNo { get; set; } = null!;

    public string QRCode { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public string UseDepartmentId { get; set; } = null!;

    public int Model_Id { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public string StorageLocationNo { get; set; } = null!;

    public virtual Departments Department { get; set; } = null!;

    public virtual Asset_Models Model { get; set; } = null!;

    public virtual StorageLocations StorageLocationNoNavigation { get; set; } = null!;

    public virtual Departments UseDepartment { get; set; } = null!;
}
