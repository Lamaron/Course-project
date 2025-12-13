using Microsoft.EntityFrameworkCore;
using Domain;
using System.IO;

namespace Data
{
    public class MusicDbContext : DbContext
    {
        public DbSet<AudioFile> AudioFiles { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }

        public MusicDbContext() { }

        public MusicDbContext(DbContextOptions<MusicDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=ALKON\SQLEXPRESS;Database=MusicPlayerDB;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(pt => pt.Playlist_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.AudioFile)
                .WithMany(a => a.PlaylistTracks)
                .HasForeignKey(pt => pt.Audio_File_Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
