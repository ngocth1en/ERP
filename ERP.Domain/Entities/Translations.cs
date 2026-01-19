using System;
using System.Collections.Generic;

namespace ERP.Infrastructure.ERP.Domain.Entities;

public partial class Translations
{
    public Guid Id { get; set; }

    public string LanguageCode { get; set; } = null!;

    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual Languages LanguageCodeNavigation { get; set; } = null!;
}
