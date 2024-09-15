using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Artist
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<Music> Musics { get; set; } = new List<Music>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
