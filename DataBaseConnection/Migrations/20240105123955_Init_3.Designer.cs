﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicPlay.Database.DatabaseAccess;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240105123955_Init_3")]
    partial class Init_3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("AudioHandler.Models.EQBand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Band")
                        .HasColumnType("INTEGER");

                    b.Property<double>("BandWidth")
                        .HasColumnType("REAL");

                    b.Property<double>("CenterFrequency")
                        .HasColumnType("REAL");

                    b.Property<int>("Channels")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EQPresetId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Gain")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("EQPresetId");

                    b.ToTable("EQBand");
                });

            modelBuilder.Entity("AudioHandler.Models.EQPreset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EQPreset");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.AlbumTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlbumId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TagId", "AlbumId");

                    b.HasIndex("AlbumId");

                    b.ToTable("AlbumTag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.ArtistGroupMember", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GroupId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("ArtistGroupMember");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.ArtistTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ArtistId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TagId", "ArtistId");

                    b.HasIndex("ArtistId");

                    b.ToTable("ArtistTag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.PlaylistTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlaylistId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TagId", "PlaylistId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("PlaylistTag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.PlaylistTrack", b =>
                {
                    b.Property<int>("PlaylistId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlaylistId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("PlaylistTrack");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.QueueTrack", b =>
                {
                    b.Property<int>("QueueId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("QueueId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("QueueTrack");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.TrackTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TagId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("TrackTag");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AlbumCover")
                        .HasColumnType("TEXT");

                    b.Property<string>("Copyright")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Information")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCompilation")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsVariousArtists")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LabelId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PrimaryArtistId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReleaseDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LabelId");

                    b.HasIndex("PrimaryArtistId");

                    b.ToTable("Album");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Biography")
                        .HasColumnType("TEXT");

                    b.Property<int>("BirthDate")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CountryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cover")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DeathDate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsGroup")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RealName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Artist");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.ArtistRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ArtistId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("RoleId");

                    b.ToTable("ArtistRole");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.DataBaseModels.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.DataBaseModels.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Label");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.DataBaseModels.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMonitored")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TrackImportedCount")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("(SELECT COUNT(*) FROM Tracks WHERE FolderId = Folder.Id)");

                    b.HasKey("Id");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Lyrics", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LyricsText")
                        .HasColumnType("TEXT")
                        .HasColumnName("Lyrics");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Lyrics");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.PlayHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayTime")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayHistory");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.PlayHistoryEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("HistoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayTime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayedLength")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrackId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("HistoryId");

                    b.HasIndex("TrackId");

                    b.ToTable("PlayHistoryEntry");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cover")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlaylistType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Queue", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cover")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsOnRepeat")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsShuffled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayingFromId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayingFromModelType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayingFromName")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayingTrackId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id", "PlayingTrackId")
                        .IsUnique();

                    b.ToTable("Queue");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.TimedLyricsLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Line")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LyricsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimestampMs")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LyricsId");

                    b.ToTable("TimedLyricsLine");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Track", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlbumId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Artwork")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DiscNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FolderId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Length")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0)
                        .HasColumnName("Length");

                    b.Property<int?>("LyricsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayCount")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Rating")
                        .HasColumnType("REAL");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TrackNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("FolderId");

                    b.HasIndex("LyricsId");

                    b.ToTable("Track");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.TrackArtistsRole", b =>
                {
                    b.Property<int>("TrackId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ArtistRoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TrackId", "ArtistRoleId");

                    b.HasIndex("ArtistRoleId");

                    b.ToTable("TrackArtistRole");
                });

            modelBuilder.Entity("AudioHandler.Models.EQBand", b =>
                {
                    b.HasOne("AudioHandler.Models.EQPreset", null)
                        .WithMany("Effects")
                        .HasForeignKey("EQPresetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.AlbumTag", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Album", "Album")
                        .WithMany("AlbumTags")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Tag", "Tag")
                        .WithMany("AlbumTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.ArtistGroupMember", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Artist", "Group")
                        .WithMany("GroupMembers")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Artist", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.ArtistTag", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Artist", "Artist")
                        .WithMany("ArtistTags")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Tag", "Tag")
                        .WithMany("ArtistTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.PlaylistTag", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Playlist", "Playlist")
                        .WithMany("PlaylistTags")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Tag", "Tag")
                        .WithMany("PlaylistTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.PlaylistTrack", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Playlist", "Playlist")
                        .WithMany("PlaylistTracks")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Track", "Track")
                        .WithMany("PlaylistTracks")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.QueueTrack", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Queue", "Queue")
                        .WithMany("Tracks")
                        .HasForeignKey("QueueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Queue");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("DataBaseConnection.Models.DataBaseModels.TrackTag", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Tag", "Tag")
                        .WithMany("TrackTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Track", "Track")
                        .WithMany("TrackTags")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Album", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.DataBaseModels.Label", "Label")
                        .WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MusicPlay.Database.Models.Artist", "PrimaryArtist")
                        .WithMany("Albums")
                        .HasForeignKey("PrimaryArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Label");

                    b.Navigation("PrimaryArtist");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Artist", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.DataBaseModels.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Country");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.ArtistRole", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Artist", "Artist")
                        .WithMany("ArtistRoles")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.DataBaseModels.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.PlayHistoryEntry", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.PlayHistory", null)
                        .WithMany("Entries")
                        .HasForeignKey("HistoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Track");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Queue", b =>
                {
                    b.HasOne("DataBaseConnection.Models.DataBaseModels.QueueTrack", "PlayingTrack")
                        .WithOne()
                        .HasForeignKey("MusicPlay.Database.Models.Queue", "Id", "PlayingTrackId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("PlayingTrack");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.TimedLyricsLine", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Lyrics", null)
                        .WithMany("TimedLines")
                        .HasForeignKey("LyricsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Track", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.Album", "Album")
                        .WithMany("Tracks")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Folder", "Folder")
                        .WithMany()
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Lyrics", "Lyrics")
                        .WithMany()
                        .HasForeignKey("LyricsId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Album");

                    b.Navigation("Folder");

                    b.Navigation("Lyrics");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.TrackArtistsRole", b =>
                {
                    b.HasOne("MusicPlay.Database.Models.ArtistRole", "ArtistRole")
                        .WithMany()
                        .HasForeignKey("ArtistRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlay.Database.Models.Track", "Track")
                        .WithMany("TrackArtistRole")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ArtistRole");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("AudioHandler.Models.EQPreset", b =>
                {
                    b.Navigation("Effects");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Album", b =>
                {
                    b.Navigation("AlbumTags");

                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Artist", b =>
                {
                    b.Navigation("Albums");

                    b.Navigation("ArtistRoles");

                    b.Navigation("ArtistTags");

                    b.Navigation("GroupMembers");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Lyrics", b =>
                {
                    b.Navigation("TimedLines");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.PlayHistory", b =>
                {
                    b.Navigation("Entries");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Playlist", b =>
                {
                    b.Navigation("PlaylistTags");

                    b.Navigation("PlaylistTracks");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Queue", b =>
                {
                    b.Navigation("Tracks");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Tag", b =>
                {
                    b.Navigation("AlbumTags");

                    b.Navigation("ArtistTags");

                    b.Navigation("PlaylistTags");

                    b.Navigation("TrackTags");
                });

            modelBuilder.Entity("MusicPlay.Database.Models.Track", b =>
                {
                    b.Navigation("PlaylistTracks");

                    b.Navigation("TrackArtistRole");

                    b.Navigation("TrackTags");
                });
#pragma warning restore 612, 618
        }
    }
}
