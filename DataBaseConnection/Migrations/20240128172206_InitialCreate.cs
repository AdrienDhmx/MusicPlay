using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Track_Queue_QueueId",
                table: "Track");

            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.DropIndex(
                name: "IX_Track_QueueId",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "QueueId",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "TrackIndex",
                table: "Track");

            migrationBuilder.AddColumn<int>(
                name: "AlbumId",
                table: "Tag",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlaylistTrack",
                columns: table => new
                {
                    TrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlaylistId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTrack", x => new { x.PlaylistId, x.TrackId });
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTrack_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_AlbumId",
                table: "Tag",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTrack_TrackId",
                table: "PlaylistTrack",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Album_AlbumId",
                table: "Tag",
                column: "AlbumId",
                principalTable: "Album",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Album_AlbumId",
                table: "Tag");

            migrationBuilder.DropTable(
                name: "PlaylistTrack");

            migrationBuilder.DropIndex(
                name: "IX_Tag_AlbumId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "AlbumId",
                table: "Tag");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Track",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QueueId",
                table: "Track",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackIndex",
                table: "Track",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlaylistTracks",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTracks", x => new { x.PlaylistId, x.TrackId });
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Track_QueueId",
                table: "Track",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Track_Queue_QueueId",
                table: "Track",
                column: "QueueId",
                principalTable: "Queue",
                principalColumn: "Id");
        }
    }
}
