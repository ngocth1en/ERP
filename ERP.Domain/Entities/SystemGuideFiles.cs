using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class SystemGuideFiles
{
    public int Id { get; set; }

    public string SystemCode { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public virtual SystemModules SystemCodeNavigation { get; set; } = null!;
}
