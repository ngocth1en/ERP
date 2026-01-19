using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class UserMenuPermissions
{
    public Guid UserId { get; set; }

    public string MenuCode { get; set; } = null!;

    public string PermissionMask { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SystemMenus MenuCodeNavigation { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
