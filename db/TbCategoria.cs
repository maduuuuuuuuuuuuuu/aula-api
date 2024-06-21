using System;
using System.Collections.Generic;

namespace aula_api.db;

public partial class TbCategoria
{
    public int IdCategoria { get; set; }

    public string? NmCategoria { get; set; }

    public string? DsCategoria { get; set; }

    public virtual ICollection<TbLivro> TbLivro { get; set; } = new List<TbLivro>();
}
