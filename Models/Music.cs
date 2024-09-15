using System;
using System.Collections.Generic;

namespace PulseMusic.Models;

public partial class Music
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LinkAudio { get; set; } = null!;

    public int? Player { get; set; }

    public int? Likes { get; set; }

    public string Image { get; set; } = null!;

    public DateTime PostingTime { get; set; }

    public string? AlbumId { get; set; }

    public string GenreId { get; set; } = null!;

    public string ArtistId { get; set; } = null!;

    public virtual Album? Album { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<Like> LikesNavigation { get; set; } = new List<Like>();

    public virtual ICollection<ListDetail> ListDetails { get; set; } = new List<ListDetail>();
}
