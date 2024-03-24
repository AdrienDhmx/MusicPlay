using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EQPreset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQPreset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsMonitored = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrackImportedCount = table.Column<int>(type: "INTEGER", nullable: false, computedColumnSql: "(SELECT COUNT(*) FROM Tracks WHERE FolderId = Folder.Id)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lyrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Lyrics = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lyrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayTime = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Cover = table.Column<string>(type: "TEXT", nullable: false),
                    PlaylistType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Cover = table.Column<string>(type: "TEXT", nullable: false),
                    Biography = table.Column<string>(type: "TEXT", nullable: false),
                    RealName = table.Column<string>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<int>(type: "INTEGER", nullable: false),
                    DeathDate = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGroup = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artist_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EQBand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EQPresetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Band = table.Column<int>(type: "INTEGER", nullable: false),
                    Channels = table.Column<int>(type: "INTEGER", nullable: false),
                    CenterFrequency = table.Column<double>(type: "REAL", nullable: false),
                    BandWidth = table.Column<double>(type: "REAL", nullable: false),
                    Gain = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EQBand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EQBand_EQPreset_EQPresetId",
                        column: x => x.EQPresetId,
                        principalTable: "EQPreset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimedLyricsLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LyricsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimestampMs = table.Column<int>(type: "INTEGER", nullable: false),
                    Line = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimedLyricsLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimedLyricsLine_Lyrics_LyricsId",
                        column: x => x.LyricsId,
                        principalTable: "Lyrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistTag",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTag", x => new { x.Id2, x.Id1 });
                    table.ForeignKey(
                        name: "FK_PlaylistTag_Playlist_Id2",
                        column: x => x.Id2,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTag_Tag_Id1",
                        column: x => x.Id1,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Album",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LabelId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrimaryArtistId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Copyright = table.Column<string>(type: "TEXT", nullable: false),
                    AlbumCover = table.Column<string>(type: "TEXT", nullable: false),
                    IsVariousArtists = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReleaseDate = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCompilation = table.Column<bool>(type: "INTEGER", nullable: false),
                    Information = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Album_Artist_PrimaryArtistId",
                        column: x => x.PrimaryArtistId,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Album_Label_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Label",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ArtistGroupMember",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistGroupMember", x => new { x.Id1, x.Id2 });
                    table.ForeignKey(
                        name: "FK_ArtistGroupMember_Artist_Id1",
                        column: x => x.Id1,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistGroupMember_Artist_Id2",
                        column: x => x.Id2,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArtistId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistRole_Artist_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistTag",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTag", x => new { x.Id2, x.Id1 });
                    table.ForeignKey(
                        name: "FK_ArtistTag_Artist_Id2",
                        column: x => x.Id2,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistTag_Tag_Id1",
                        column: x => x.Id1,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumTag",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTag", x => new { x.Id2, x.Id1 });
                    table.ForeignKey(
                        name: "FK_AlbumTag_Album_Id2",
                        column: x => x.Id2,
                        principalTable: "Album",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumTag_Tag_Id1",
                        column: x => x.Id1,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlbumId = table.Column<int>(type: "INTEGER", nullable: false),
                    FolderId = table.Column<int>(type: "INTEGER", nullable: false),
                    LyricsId = table.Column<int>(type: "INTEGER", nullable: true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Artwork = table.Column<string>(type: "TEXT", nullable: false),
                    TrackNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    DiscNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rating = table.Column<double>(type: "REAL", nullable: false),
                    IsLive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Track_Album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Album",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Track_Folder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Track_Lyrics_LyricsId",
                        column: x => x.LyricsId,
                        principalTable: "Lyrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlayHistoryEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HistoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayedLength = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayTime = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayHistoryEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayHistoryEntry_PlayHistory_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "PlayHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayHistoryEntry_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistTrack",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTrack", x => new { x.Id1, x.Id2 });
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Playlist_Id1",
                        column: x => x.Id1,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Track_Id2",
                        column: x => x.Id2,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackArtistRole",
                columns: table => new
                {
                    TrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArtistRoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackArtistRole", x => new { x.TrackId, x.ArtistRoleId });
                    table.ForeignKey(
                        name: "FK_TrackArtistRole_ArtistRole_ArtistRoleId",
                        column: x => x.ArtistRoleId,
                        principalTable: "ArtistRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackArtistRole_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackTag",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTag", x => new { x.Id2, x.Id1 });
                    table.ForeignKey(
                        name: "FK_TrackTag_Tag_Id1",
                        column: x => x.Id1,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackTag_Track_Id2",
                        column: x => x.Id2,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Queue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    IsShuffled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOnRepeat = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlayingFromName = table.Column<string>(type: "TEXT", nullable: false),
                    PlayingFromId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cover = table.Column<string>(type: "TEXT", nullable: false),
                    PlayingTrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayingFromModelType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "INTEGER", nullable: false),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPlayed = table.Column<DateTime>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueueTrack",
                columns: table => new
                {
                    Id1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Id2 = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPlaying = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueTrack", x => new { x.Id1, x.Id2 });
                    table.ForeignKey(
                        name: "FK_QueueTrack_Queue_Id1",
                        column: x => x.Id1,
                        principalTable: "Queue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueTrack_Track_Id2",
                        column: x => x.Id2,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Album_LabelId",
                table: "Album",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Album_PrimaryArtistId",
                table: "Album",
                column: "PrimaryArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTag_Id1",
                table: "AlbumTag",
                column: "Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Artist_CountryId",
                table: "Artist",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistGroupMember_Id2",
                table: "ArtistGroupMember",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRole_ArtistId",
                table: "ArtistRole",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRole_RoleId",
                table: "ArtistRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTag_Id1",
                table: "ArtistTag",
                column: "Id1");

            migrationBuilder.CreateIndex(
                name: "IX_EQBand_EQPresetId",
                table: "EQBand",
                column: "EQPresetId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayHistoryEntry_HistoryId",
                table: "PlayHistoryEntry",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayHistoryEntry_TrackId",
                table: "PlayHistoryEntry",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTag_Id1",
                table: "PlaylistTag",
                column: "Id1");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTrack_Id2",
                table: "PlaylistTrack",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Queue_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueueTrack_Id2",
                table: "QueueTrack",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_TimedLyricsLine_LyricsId",
                table: "TimedLyricsLine",
                column: "LyricsId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_AlbumId",
                table: "Track",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_FolderId",
                table: "Track",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Track_LyricsId",
                table: "Track",
                column: "LyricsId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackArtistRole_ArtistRoleId",
                table: "TrackArtistRole",
                column: "ArtistRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTag_Id1",
                table: "TrackTag",
                column: "Id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                principalTable: "QueueTrack",
                principalColumns: new[] { "Id1", "Id2" },
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Album_Artist_PrimaryArtistId",
                table: "Album");

            migrationBuilder.DropForeignKey(
                name: "FK_Album_Label_LabelId",
                table: "Album");

            migrationBuilder.DropForeignKey(
                name: "FK_Track_Album_AlbumId",
                table: "Track");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Track_Id2",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropTable(
                name: "AlbumTag");

            migrationBuilder.DropTable(
                name: "ArtistGroupMember");

            migrationBuilder.DropTable(
                name: "ArtistTag");

            migrationBuilder.DropTable(
                name: "EQBand");

            migrationBuilder.DropTable(
                name: "PlayHistoryEntry");

            migrationBuilder.DropTable(
                name: "PlaylistTag");

            migrationBuilder.DropTable(
                name: "PlaylistTrack");

            migrationBuilder.DropTable(
                name: "TimedLyricsLine");

            migrationBuilder.DropTable(
                name: "TrackArtistRole");

            migrationBuilder.DropTable(
                name: "TrackTag");

            migrationBuilder.DropTable(
                name: "EQPreset");

            migrationBuilder.DropTable(
                name: "PlayHistory");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "ArtistRole");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Artist");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Label");

            migrationBuilder.DropTable(
                name: "Album");

            migrationBuilder.DropTable(
                name: "Track");

            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.DropTable(
                name: "Lyrics");

            migrationBuilder.DropTable(
                name: "QueueTrack");

            migrationBuilder.DropTable(
                name: "Queue");
        }
    }
}
