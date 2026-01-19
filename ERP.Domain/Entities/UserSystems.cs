using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class UserSystems
{
    public Guid UserId { get; set; }

    public string SystemCode { get; set; } = null!;

    public bool IsAllowed { get; set; }

    public string PermissionMask { get; set; } = null!;

    public virtual SystemModules SystemCodeNavigation { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
