using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrackImportedCount",
                table: "Folder",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComputedColumnSql: "(SELECT COUNT(*) FROM Track WHERE FolderId = Folder.Id)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrackImportedCount",
                table: "Folder",
                type: "INTEGER",
                nullable: false,
                computedColumnSql: "(SELECT COUNT(*) FROM Track WHERE FolderId = Folder.Id)",
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
