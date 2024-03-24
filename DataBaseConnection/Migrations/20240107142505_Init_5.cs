using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue");

            migrationBuilder.AlterColumn<int>(
                name: "TrackImportedCount",
                table: "Folder",
                type: "INTEGER",
                nullable: false,
                computedColumnSql: "(SELECT COUNT(*) FROM Track WHERE FolderId = Folder.Id)",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComputedColumnSql: "(SELECT COUNT(*) FROM Tracks WHERE FolderId = Folder.Id)");

            migrationBuilder.CreateIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue");

            migrationBuilder.AlterColumn<int>(
                name: "TrackImportedCount",
                table: "Folder",
                type: "INTEGER",
                nullable: false,
                computedColumnSql: "(SELECT COUNT(*) FROM Tracks WHERE FolderId = Folder.Id)",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComputedColumnSql: "(SELECT COUNT(*) FROM Track WHERE FolderId = Folder.Id)");

            migrationBuilder.CreateIndex(
                name: "IX_Queue_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId",
                unique: true);
        }
    }
}
