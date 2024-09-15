using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class ListDetail
{
    public string Id { get; set; } = null!;

    public string MusicId { get; set; } = null!;

    public string PlayListId { get; set; } = null!;

    public virtual Music Music { get; set; } = null!;

    public virtual PlayList PlayList { get; set; } = null!;
}
