using System.Configuration;
using AudioHandler.Models;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.DatabaseAccess
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Lyrics> Lyrics { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Queue> Queues { get; set; }

        public DbSet<PlayHistory> PlayHistories { get; set; }

        public DbSet<PlayHistoryEntry> PlayHistoryEntries { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<ArtistRole> ArtistRoles { get; set; }

        public DbSet<TrackArtistsRole> TrackArtistRoles { get; set; }

        public DbSet<EQPreset> EQPresetModels { get; set; }

        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }

        public DbSet<AlbumTag> AlbumTags { get; set; }
        public DbSet<ArtistTag> ArtistTags { get; set; }
        public DbSet<PlaylistTag> PlaylistTags { get; set; }
        public DbSet<TrackTag> TrackTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=.\\musicdb.db;"); // ConfigurationManager.ConnectionStrings["default"].ConnectionString
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The required fields are decorated with [Required] in the classes
            // The unique indexes are decorated with [Index(isUnique = true)] in the classes

            // Artists (and ArtistRole)
            OnArtistCreated(modelBuilder);

            // Albums
            OnAlbumCreated(modelBuilder);

            // Tracks (and TrackArtistRole)
            OnTrackCreated(modelBuilder);

            // Playlists (and PlaylistTrack)
            OnPlaylistCreated(modelBuilder);

            // Lyrics (and TimedLyricsLine)
            OnLyricsCreated(modelBuilder);

            // Tags (and TrackTag, ArtistTag, AlbumTag, PlaylistTag)
            OnTagCreated(modelBuilder);

            // Queue (and Tracks)
            OnQueueCreated(modelBuilder);

            // PlayHistory (and PlayHistoryEntry)
            OnHistoryCreated(modelBuilder);

            // Folders
            //modelBuilder.Entity<Folder>()
            //    .Property(f => f.TrackImportedCount)
            //    .HasComputedColumnSql("(SELECT COUNT(*) FROM Track WHERE FolderId = Folder.Id)");

            OnEQPresetCreated(modelBuilder);

            // Call the base implementation
            base.OnModelCreating(modelBuilder);
        }

        private static void OnTrackCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Track>()
                .Property<int>(t => t.Length)
                .HasColumnName("Length")
                .IsRequired()
                .HasDefaultValue(0);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Lyrics)
                .WithMany()
                .HasForeignKey(t => t.LyricsId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Folder)
                .WithMany()
                .HasForeignKey(t => t.FolderId);

            modelBuilder.Entity<Track>()
                .HasMany(t => t.TrackArtistRole)
                .WithOne(tar => tar.Track)
                .HasForeignKey(tar => tar.TrackId);

            modelBuilder.Entity<Track>()
                .HasMany(t => t.TrackTags)
                .WithOne(tar => tar.Track)
                .HasForeignKey(tar => tar.TrackId);

            modelBuilder.Entity<Track>()
                .HasMany(t => t.PlaylistTracks)
                .WithOne(tar => tar.Track)
                .HasForeignKey(tar => tar.TrackId);

            modelBuilder.Entity<TrackArtistsRole>()
                .HasKey(tar => new { tar.TrackId, tar.ArtistRoleId });

            modelBuilder.Entity<TrackArtistsRole>()
                .HasOne(tar => tar.Track)
                .WithMany(t => t.TrackArtistRole)
                .HasForeignKey(tar => tar.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrackArtistsRole>()
                .HasOne(tar => tar.ArtistRole)
                .WithMany()
                .HasForeignKey(tar => tar.ArtistRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void OnArtistCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .HasOne(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Artist>()
                .HasMany(a => a.GroupMembers)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId);

            modelBuilder.Entity<Artist>()
                .HasMany(a => a.ArtistTags)
                .WithOne(at => at.Artist)
                .HasForeignKey(at => at.ArtistId);

            // GroupGroupMembers
            modelBuilder.Entity<ArtistGroupMember>()
                .HasKey(gm => new {gm.GroupId, gm.MemberId });

            modelBuilder.Entity<ArtistGroupMember>()
                 .HasOne(agm => agm.Group)
                 .WithMany(a => a.GroupMembers)
                 .HasForeignKey(agm => agm.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtistGroupMember>()
                .HasOne(agm => agm.Member)
                .WithMany()
                .HasForeignKey(agm => agm.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // ArtistRoles
            modelBuilder.Entity<ArtistRole>()
                .HasKey(ar => ar.Id);

            modelBuilder.Entity<ArtistRole>()
                .HasOne(ar => ar.Artist)
                .WithMany(a => a.ArtistRoles)
                .HasForeignKey(ar => ar.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArtistRole>()
                .HasOne(ar => ar.Role)
                .WithMany()
                .HasForeignKey(ar => ar.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void OnAlbumCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>()
                .HasOne(a => a.PrimaryArtist)
                .WithMany(a => a.Albums)
                .HasForeignKey(a => a.PrimaryArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Label)
                .WithMany()
                .HasForeignKey(a => a.LabelId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Album>()
                .HasMany(a => a.AlbumTags)
                .WithOne(at => at.Album)
                .HasForeignKey(at => at.AlbumId);
        }

        private static void OnPlaylistCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Tracks)
                .WithOne(pt => pt.Playlist)
                .HasForeignKey(pt => pt.PlaylistId);

            modelBuilder.Entity<Playlist>()
                .HasMany(t => t.PlaylistTags)
                .WithOne(pt => pt.Playlist)
                .HasForeignKey(tar => tar.PlaylistId);

            // Tracks
            modelBuilder.Entity<PlaylistTrack>()
                .HasKey(pt => new { pt.PlaylistId, pt.TrackId });

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Track)
                .WithMany(t => t.PlaylistTracks)
                .HasForeignKey(pt => pt.TrackId);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.Tracks)
                .HasForeignKey(pt => pt.PlaylistId);
        }

        private static void OnLyricsCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lyrics>()
                .HasMany(l => l.TimedLines)
                .WithOne()
                .HasForeignKey(tl => tl.LyricsId);
        }

        private static void OnTagCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
                .HasMany(t => t.TrackTags)
                .WithOne(tt => tt.Tag)
                .HasForeignKey(tt => tt.TagId);

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.AlbumTags)
                .WithOne(at => at.Tag)
                .HasForeignKey(at => at.TagId);

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.ArtistTags)
                .WithOne(at => at.Tag)
                .HasForeignKey(at => at.TagId);

            modelBuilder.Entity<Tag>()
                .HasMany(t => t.PlaylistTags)
                .WithOne(pt => pt.Tag)
                .HasForeignKey(pt => pt.TagId);


            // TrackTags
            modelBuilder.Entity<TrackTag>()
                .HasKey(tt => new { tt.TagId, tt.TrackId });

            modelBuilder.Entity<TrackTag>()
                .HasOne(tt => tt.Track)
                .WithMany(t => t.TrackTags)
                .HasForeignKey(tt => tt.TrackId);

            modelBuilder.Entity<TrackTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TrackTags)
                .HasForeignKey(tt => tt.TagId);

            // ArtistTags
            modelBuilder.Entity<ArtistTag>()
                .HasKey(tt => new { tt.TagId, tt.ArtistId });

            modelBuilder.Entity<ArtistTag>()
                .HasOne(tt => tt.Artist)
                .WithMany(t => t.ArtistTags)
                .HasForeignKey(tt => tt.ArtistId);

            modelBuilder.Entity<ArtistTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.ArtistTags)
                .HasForeignKey(tt => tt.TagId);

            // AlbumTags
            modelBuilder.Entity<AlbumTag>()
                .HasKey(tt => new { tt.TagId, tt.AlbumId });

            modelBuilder.Entity<AlbumTag>()
                .HasOne(tt => tt.Album)
                .WithMany(t => t.AlbumTags)
                .HasForeignKey(tt => tt.AlbumId);

            modelBuilder.Entity<AlbumTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.AlbumTags)
                .HasForeignKey(tt => tt.TagId);

            // PlaylistTags
            modelBuilder.Entity<PlaylistTag>()
                .HasKey(tt => new { tt.TagId, tt.PlaylistId });

            modelBuilder.Entity<PlaylistTag>()
                .HasOne(tt => tt.Playlist)
                .WithMany(t => t.PlaylistTags)
                .HasForeignKey(tt => tt.PlaylistId);

            modelBuilder.Entity<PlaylistTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.PlaylistTags)
                .HasForeignKey(tt => tt.TagId);

        }

        private static void OnQueueCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Queue>()
                .HasMany(q => q.Tracks)
                .WithOne(qt => qt.Queue)
                .HasForeignKey(qt => qt.QueueId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tracks
            modelBuilder.Entity<QueueTrack>()
                .HasKey(pt => new { pt.QueueId, pt.TrackId });

            modelBuilder.Entity<QueueTrack>()
                .HasOne(pt => pt.Track)
                .WithMany()
                .HasForeignKey(pt => pt.TrackId);

            modelBuilder.Entity<QueueTrack>()
                .HasOne(pt => pt.Queue)
                .WithMany(p => p.Tracks)
                .HasForeignKey(pt => pt.QueueId);
        }

        private static void OnHistoryCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayHistory>()
                .HasMany(ph => ph.Entries)
                .WithOne()
                .HasForeignKey(phe => phe.HistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayHistoryEntry>()
                .HasOne(phe => phe.Track)
                .WithMany()
                .HasForeignKey(phe => phe.TrackId);
        }

        private static void OnEQPresetCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EQPreset>()
                .HasMany(eqp => eqp.Effects)
                .WithOne()
                .HasForeignKey(e => e.EQPresetId);
        }
    }
}
