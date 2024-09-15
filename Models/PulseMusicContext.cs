using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PulseMusic.Models;

public partial class PulseMusicContext : DbContext
{
    public PulseMusicContext()
    {
    }

    public PulseMusicContext(DbContextOptions<PulseMusicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<ListDetail> ListDetails { get; set; }

    public virtual DbSet<Music> Musics { get; set; }

    public virtual DbSet<PlayList> PlayLists { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.HasIndex(e => e.Id, "IX_Account").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Album>(entity =>
        {
            entity.ToTable("Album");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ArtistId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Album_Artist");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.ToTable("Artist");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genre");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Like");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MusicId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Account).WithMany(p => p.Likes)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_Account");

            entity.HasOne(d => d.Music).WithMany(p => p.LikesNavigation)
                .HasForeignKey(d => d.MusicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_Music");
        });

        modelBuilder.Entity<ListDetail>(entity =>
        {
            entity.ToTable("ListDetail");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MusicId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PlayListId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Music).WithMany(p => p.ListDetails)
                .HasForeignKey(d => d.MusicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ListDetail_Music");

            entity.HasOne(d => d.PlayList).WithMany(p => p.ListDetails)
                .HasForeignKey(d => d.PlayListId)
                .HasConstraintName("FK_ListDetail_PlayList");
        });

        modelBuilder.Entity<Music>(entity =>
        {
            entity.ToTable("Music");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.AlbumId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ArtistId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.GenreId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PostingTime).HasColumnType("datetime");

            entity.HasOne(d => d.Album).WithMany(p => p.Musics)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Music_Album1");

            entity.HasOne(d => d.Artist).WithMany(p => p.Musics)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Music_Artist");

            entity.HasOne(d => d.Genre).WithMany(p => p.Musics)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Music_Genre");
        });

        modelBuilder.Entity<PlayList>(entity =>
        {
            entity.ToTable("PlayList");

            entity.HasIndex(e => e.Id, "IX_PlayList").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.PlayLists)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayList_Account");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.ToTable("Track");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ArtistId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Account).WithMany(p => p.Tracks)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Track_Account");

            entity.HasOne(d => d.Artist).WithMany(p => p.Tracks)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Track_Artist");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
