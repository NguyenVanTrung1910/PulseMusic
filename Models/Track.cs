using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Track
{
    public string Id { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string ArtistId { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Artist Artist { get; set; } = null!;
}
