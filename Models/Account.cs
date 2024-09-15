using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<PlayList> PlayLists { get; set; } = new List<PlayList>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
