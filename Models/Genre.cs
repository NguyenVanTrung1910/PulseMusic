using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Genre
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Music> Musics { get; set; } = new List<Music>();
}
