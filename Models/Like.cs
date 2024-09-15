using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Like
{
    public string Id { get; set; } = null!;

    public string MusicId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Music Music { get; set; } = null!;
}
