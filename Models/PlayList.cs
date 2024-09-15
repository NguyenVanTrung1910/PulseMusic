using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class PlayList
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<ListDetail> ListDetails { get; set; } = new List<ListDetail>();
}
