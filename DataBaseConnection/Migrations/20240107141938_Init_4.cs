using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropIndex(
                name: "IX_Queue_Id_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistTrack",
                table: "PlaylistTrack");

            migrationBuilder.DropColumn(
                name: "PlaylistType",
                table: "Playlist");

            migrationBuilder.RenameTable(
                name: "PlaylistTrack",
                newName: "PlaylistTracks");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_TrackId",
                table: "PlaylistTracks",
                newName: "IX_PlaylistTracks_TrackId");

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

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Queue",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistTracks",
                table: "PlaylistTracks",
                columns: new[] { "PlaylistId", "TrackId" });

            migrationBuilder.CreateIndex(
                name: "IX_Track_QueueId",
                table: "Track",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Playlist_PlaylistId",
                table: "PlaylistTracks",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Track_TrackId",
                table: "PlaylistTracks",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Track_Queue_QueueId",
                table: "Track",
                column: "QueueId",
                principalTable: "Queue",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Playlist_PlaylistId",
                table: "PlaylistTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Track_TrackId",
                table: "PlaylistTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_Track_Queue_QueueId",
                table: "Track");

            migrationBuilder.DropIndex(
                name: "IX_Track_QueueId",
                table: "Track");

            migrationBuilder.DropIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistTracks",
                table: "PlaylistTracks");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "QueueId",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "TrackIndex",
                table: "Track");

            migrationBuilder.RenameTable(
                name: "PlaylistTracks",
                newName: "PlaylistTrack");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTracks_TrackId",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_TrackId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Queue",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "PlaylistType",
                table: "Playlist",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistTrack",
                table: "PlaylistTrack",
                columns: new[] { "PlaylistId", "TrackId" });

            migrationBuilder.CreateIndex(
                name: "IX_Queue_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                principalTable: "QueueTrack",
                principalColumns: new[] { "QueueId", "TrackId" },
                onDelete: ReferentialAction.SetNull);
        }
    }
}
